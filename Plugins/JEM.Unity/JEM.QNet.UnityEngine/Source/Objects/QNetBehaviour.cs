//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

#define DEEP_DEBUG

using JEM.Core.Common;
using JEM.Core.Debugging;
using JEM.QNet.Messages;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace JEM.QNet.UnityEngine.Objects
{
    /// <inheritdoc />
    /// <summary>
    ///     Attribute that tells what method is a network message
    ///      that could receive data from another connection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class QNetMessageAttribute : Attribute
    {
        /// <summary>
        ///     A channel this method is using.
        /// </summary>
        public Enum Channel = QNetBaseChannel.DEFAULT;

        /// <summary>
        ///     A delivery method this method is using.
        /// </summary>
        public QNetMessageMethod Method = QNetMessageMethod.Unreliable;

        public QNetMessageAttribute() { }
        public QNetMessageAttribute(Enum channel) => Channel = channel;     
        public QNetMessageAttribute(Enum channel, QNetMessageMethod method)
        {
            Channel = channel;
            Method = method;
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Base class of every network (QNet) behaviour.
    /// </summary>
    /// <remarks>
    ///     NOTE: It's recommended to use UnsafeSimulate/UnsafeLateSimulate and Simulate methods
    ///      of unity's Update/LateUpdate/FixedUpdate for the best simulation results.
    /// </remarks>
    [RequireComponent(typeof(QNetIdentity))]
    public abstract partial class QNetBehaviour : QNetObject
    {
        public delegate void BehaviourInternalMessage(QNetMessageReader reader);

        public class CachedQNetMessagePointer
        {
            public QNetMessageAttribute Attribute;
            public MethodInfo Method;
            public byte Index;
        }

        public class QNetMessagePointer
        {
            public QNetMessageAttribute Attribute;
            public BehaviourInternalMessage Delegate;
            public byte Index;
        }

        /// <summary>
        ///     Defines if the server is currently active.
        /// </summary>
        public new bool IsServer => QNetManager.Instance.IsServerActive;

        /// <summary>
        ///     Defines if the client is currently active.
        /// </summary>
        public new bool IsClient => QNetManager.Instance.IsClientActive;

        /// <summary>
        ///     List of all registered message pointers.
        /// </summary>
        public List<QNetMessagePointer> MessagePointers { get; } = new List<QNetMessagePointer>();
        private bool _messagePointersCollected;

        /// <summary>
        ///     OnNetworkSpawned method is called when the network on local machine has been activated.
        ///     Called instantly on object initialize if network is already active.
        /// </summary>
        private JEMSmartMethodS _onNetworkSpawned;

        /// <summary>
        ///     OnLateNetworkSpawned method is called after initialize OnDeserializeServerState call.
        ///     If <see cref="QNetManager.IsServerActive"/> is true, this method will be always called right after the OnNetworkSpawned
        ///      cuz server will never de-serialize state of local objects.
        /// </summary>
        private JEMSmartMethodS _onLateNetworkSpawned;

        /// <summary>
        ///     OnPooled method is called when this QNetObject is being pooled by local pooling system.
        ///     Entities are pooled to reduce time on destroying and spawning new entity of same prefab.
        /// </summary>
        private JEMSmartMethodS _onPooled;

        /// <summary>
        ///     OnNetworkActiveChange is called when the <see cref="QNetIdentity.IsNetworkActive"/> changes.
        ///     Always called by <see cref="QNetIdentity.Spawn"/> to activate or <see cref="QNetIdentity.Pool"/> to de-activate the object.
        /// </summary>
        private JEMSmartMethodS<bool> _onNetworkActiveChange;
    
        /// <inheritdoc />
        protected override void LoadMethods()
        {
            // Invoke base method.
            base.LoadMethods();

            Profiler.BeginSample("QNetBehaviour.LoadMethods");

            // Collect local methods.
            _onNetworkSpawned = new JEMSmartMethodS(this, "OnNetworkSpawned");
            _onLateNetworkSpawned = new JEMSmartMethodS(this, "OnLateNetworkSpawned");
            _onPooled = new JEMSmartMethodS(this, "OnPooled");
            _onNetworkActiveChange = new JEMSmartMethodS<bool>(this, "OnNetworkActiveChange");

            // Collect network methods.
            LoadNetworkMethods();

            // Collect simulation methods.
            LoadSimulationMethods();

            Profiler.EndSample();
        }

        private void LoadNetworkMethods()
        {
            for (var index = 0; index < CachedMessagePointers.Count; index++)
            {
                var p = CachedMessagePointers[index];
                if (p.Item1 == GetType())
                {
                    for (var index1 = 0; index1 < p.Item2.Count; index1++)
                    {
                        var i = p.Item2[index1];
                        try
                        {
                            var @delegate = (BehaviourInternalMessage)Delegate.CreateDelegate(typeof(BehaviourInternalMessage), this, i.Method);
                            MessagePointers.Add(new QNetMessagePointer
                            {
                                Attribute = i.Attribute,
                                Delegate = @delegate,
                                Index = i.Index
                            });
                        }
                        catch (Exception e)
                        {
                            JEMLogger.LogException(e, $"Failed to register network method of name {GetType().FullName}.{i.Method.Name}", "GAME");
                            throw;
                        }
                    }

                    _messagePointersCollected = true;
                    return;
                }
            }

            throw new NullReferenceException($"Failed to find list of network message pointers for {GetType().Name} type.");
        }

        /// <summary>
        ///     Gets a message pointer by method name.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public QNetMessagePointer GetMessagePointer([NotNull] string methodName)
        {
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            if (!_messagePointersCollected)
                throw new InvalidOperationException($"You are trying to collect message pointer of name '{methodName}' while list of pointer has been not collected yet.");

            for (var index = 0; index < MessagePointers.Count; index++)
            {
                var pointer = MessagePointers[index];
                if (pointer.Delegate.Method.Name == methodName)
                    return pointer;
            }

            return null;
        }

        /// <summary>
        ///     Gets a message pointer by index.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public QNetMessagePointer GetMessagePointer(byte methodIndex)
        {
            if (!_messagePointersCollected)
                throw new InvalidOperationException($"You are trying to collect message pointer of index '{methodIndex}' while list of pointer has been not collected yet.");

            for (var index = 0; index < MessagePointers.Count; index++)
            {
                var pointer = MessagePointers[index];
                if (pointer.Index == methodIndex)
                    return pointer;
            }

            return null;
        }

        /// <summary>
        ///     Executes the network message at given index.
        /// </summary>
        internal void ExecuteMessage(byte messageIndex, QNetMessageReader reader)
        { 
            // Profiler.BeginSample($"QNetBehaviour.ExecuteMessage {GetType().Name}.({messageIndex})");

            var pointer = GetMessagePointer(messageIndex);
            if (pointer == null)
                throw new NullReferenceException($"There is no message pointer registered at index {messageIndex}");

            try
            {
                // Invoke.
                pointer.Delegate.Invoke(reader);
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                    e = e.InnerException;

                QNetManager.PrintLogException(e);
            }

            // Profiler.EndSample();
        }

        internal void CallOnNetworkSpawned() => _onNetworkSpawned.Invoke();
        internal void CallOnLateNetworkSpawned() => _onLateNetworkSpawned.Invoke();
        internal void CallOnPooled() => _onPooled.Invoke();
        internal void CallOnNetworkActiveChange(bool activeState) => _onNetworkActiveChange.Invoke(activeState);

        /// <summary>
        ///     Search for all <see cref="QNetBehaviour"/> based types and extract all methods from them.
        /// </summary>
        internal static void PrepareAllMethodsTypes()
        {
            Profiler.BeginSample("QNetBehaviour.PrepareAllMethodsTypes");

            int i = 0;
            int t = 0;

            var baseType = typeof(QNetBehaviour);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var index1 = 0; index1 < assemblies.Length; index1++)
            {
                var assembly = assemblies[index1];
                var allTypes = assembly.GetTypes();
                for (var index2 = 0; index2 < allTypes.Length; index2++)
                {
                    var type = allTypes[index2];
                    if (!type.IsClass && !(type.IsValueType && !type.IsPrimitive))
                    {
                        continue;
                    }

                    if (!baseType.IsAssignableFrom(type)) continue;
                    i += JEMSmartMethod.ExtractAllMethods(type).Count;
                    t++;
                }
            }

            Profiler.EndSample();

#if DEBUG
            QNetManager.PrintLogMsc($"Total of {i} methods extracted from {t} types.");
#endif
        }

        internal static void LoadAllNetworkMethods()
        {
            Profiler.BeginSample("QNetBehaviour.LoadNetworkMethods");

            int t = 0;
            int i = 0;

            var baseType = typeof(QNetBehaviour);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var index1 = 0; index1 < assemblies.Length; index1++)
            {
                var assembly = assemblies[index1];
                var allTypes = assembly.GetTypes();
                for (var index2 = 0; index2 < allTypes.Length; index2++)
                {
                    var type = allTypes[index2];
                    if (!type.IsClass && !(type.IsValueType && !type.IsPrimitive))
                    {
                        continue;
                    }

                    if (!baseType.IsAssignableFrom(type)) continue;
                    i += LoadNetworkMethods(type);
                    t++;
                }
            }

            Profiler.EndSample();

#if DEBUG
            QNetManager.PrintLogMsc($"Total of {t} types has extracted {i} network types.");
#endif

        }

        private static int LoadNetworkMethods(Type type)
        {
            Profiler.BeginSample("QNetBehaviour.LoadNetworkMethodsType");

            Profiler.BeginSample("QNetBehaviour.LoadNetworkMethodsType extract all methods");
            var allMethods = JEMSmartMethod.ExtractAllMethods(type);
            Profiler.EndSample();

            var messagePointers = new List<CachedQNetMessagePointer>();

            for (var index = 0; index < allMethods.Count; index++)
            {
                var m = allMethods[index];
                var attributes = m.GetCustomAttributes(typeof(QNetMessageAttribute), false);
                if (attributes.Length <= 0)
                    continue;

                var attribute = (QNetMessageAttribute)attributes[0];
                var newPointer = new CachedQNetMessagePointer
                {
                    Attribute = attribute,
                    Method = m,
                    Index = (byte) messagePointers.Count
                };

                messagePointers.Add(newPointer);
            }

            CachedMessagePointers.Add(new Tuple<Type, List<CachedQNetMessagePointer>>(type, messagePointers));

            Profiler.EndSample();

#if DEBUG && DEEP_DEBUG
            if (messagePointers.Count == 0)
                QNetManager.PrintLogMsc($"No messages pointers found in class of type {type.Name}.");
            else
            {
                var sb = new StringBuilder();
                for (var index = 0; index < messagePointers.Count; index++)
                {
                    var msg = messagePointers[index];
                    sb.AppendLine($"\t{msg.Method.Name}: {msg.Index}");
                }

                QNetManager.PrintLogMsc($"{messagePointers.Count} message pointers found in " +
                                        $"QNetBehaviour based component of type {type.Name}\n{sb}");
            }
#endif

            return messagePointers.Count;
        }

        private static List<Tuple<Type, List<CachedQNetMessagePointer>>> CachedMessagePointers { get; } = new List<Tuple<Type, List<CachedQNetMessagePointer>>>();
    }
}
