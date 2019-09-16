//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Messages.Extensions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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
        public class QNetMessagePointer
        {
            public QNetMessageAttribute Attribute;
            public MethodInfo Method;
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

        /// <summary>
        ///     OnNetworkSpawned method is called when the network on local machine has been activated.
        ///     Called instantly on object initialize if network is already active.
        /// </summary>
        private JEMSmartMethod _onNetworkSpawned;

        /// <inheritdoc />
        protected override void LoadMethods()
        {
            // Invoke base method.
            base.LoadMethods();

            // Collect local methods.
            _onNetworkSpawned = new JEMSmartMethod(this, "OnNetworkSpawned");

            // Collect simulation methods.
            LoadSimulationMethods();

            // Collect network methods.
            LoadNetworkMethods();
        }

        private void LoadNetworkMethods()
        {
            var allMethods = JEMSmartMethod.ExtractAllMethods(GetType());
            for (var index = 0; index < allMethods.Count; index++)
            {
                var m = allMethods[index];
                var attributes = m.GetCustomAttributes(typeof(QNetMessageAttribute), false);
                if (attributes.Length <= 0)
                    continue;

                var attribute = (QNetMessageAttribute) attributes[0];
                var newPointer = new QNetMessagePointer
                {
                    Attribute = attribute,
                    Method = m,
                    Index = (byte) MessagePointers.Count
                };

                MessagePointers.Add(newPointer);
            }

#if DEBUG
            QNetManager.PrintLogMsc($"We found {MessagePointers.Count} message pointers " +
                                    $"in QNetBehaviour based component of type {GetType().Name}", this);
#endif
        }

        /// <summary>
        ///     Gets a message pointer by method name.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal QNetMessagePointer GetMessagePointer([NotNull] string methodName)
        {
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            for (var index = 0; index < MessagePointers.Count; index++)
            {
                var pointer = MessagePointers[index];
                if (pointer.Method.Name == methodName)
                    return pointer;
            }

            return null;
        }

        /// <summary>
        ///     Gets a message pointer by index.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal QNetMessagePointer GetMessagePointer(byte methodIndex)
        {
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
            var pointer = GetMessagePointer(messageIndex);
            if (pointer == null)
                throw new NullReferenceException($"There is no message pointer registered at index {messageIndex}");

            // Check if method is generic.
            MethodInfo method = pointer.Method;
            if (method.IsGenericMethod)
            {
                // Read generic types.
                var genericArguments = method.GetGenericArguments();
                var genericTypes = new Type[genericArguments.Length];
                for (var index = 0; index < genericArguments.Length; index++)
                {
                    var typeIndex = reader.ReadByte();
                    if (GetSerializedMessageType(typeIndex, out var genericType))
                    {
                        genericTypes[index] = genericType;
                    }
                    else throw new NullReferenceException($"Method execute {method.Name} failed because of generic method error. " +
                                                          $"Unable to find serialized message type of index {typeIndex}.");
                }

                method = method.MakeGenericMethod(genericTypes);
            }

            // Read message parameters.
            var parameters = method.GetParameters();
            object[] args = new object[parameters.Length];
            for (var index = 0; index < parameters.Length; index++)
            {
                var param = parameters[index];
                var t = param.ParameterType;
                if (t == typeof(int))
                    args[index] = reader.ReadInt32();
                else if (t == typeof(uint))
                    args[index] = reader.ReadUInt32();
                else if (t == typeof(short))
                    args[index] = reader.ReadInt16();
                else if (t == typeof(ushort))
                    args[index] = reader.ReadUInt16();
                else if (t == typeof(long))
                    args[index] = reader.ReadInt64();
                else if (t == typeof(ulong))
                    args[index] = reader.ReadUInt64();
                else if (t == typeof(float))
                    args[index] = reader.ReadSingle();
                else if (t == typeof(double))
                    args[index] = reader.ReadDouble();
                else if (t == typeof(byte))
                    args[index] = reader.ReadByte();
                else if (t == typeof(string))
                    args[index] = reader.ReadString();
                else if (t == typeof(bool))
                    args[index] = reader.ReadBool();
                else if (t == typeof(byte[]))
                    args[index] = reader.ReadBytes();
                else if (t.IsSubclassOf(typeof(QNetSerializedMessage)))
                    args[index] = reader.ReadMessage(t);
                // Special types.
                else if (t == typeof(Vector3))
                    args[index] = reader.ReadVector3();
                else if (t == typeof(Vector2))
                    args[index] = reader.ReadVector2();
                else if (t == typeof(Vector4))
                    args[index] = reader.ReadVector4();
                else if (t == typeof(Color))
                    args[index] = reader.ReadColor4();
                else
                {
                    throw new NotSupportedException($"Not supported message parameter type {t.Name}.");
                }
            }

            try
            {
                // Invoke.
                method.Invoke(this, args);
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                    e = e.InnerException;

                QNetManager.PrintLogException(e);
            }
        }

        internal void CallOnNetworkSpawned() => _onNetworkSpawned.Invoke();       
    }
}
