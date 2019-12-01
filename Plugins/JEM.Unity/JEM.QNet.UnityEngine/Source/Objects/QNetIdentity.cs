//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// #define DEEP_DEBUG

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Messages;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

namespace JEM.QNet.UnityEngine.Objects
{
    /// <inheritdoc />
    /// <summary>
    ///     QNet Identity component.
    ///     Defines a network object in scene and enables communication between peers.
    ///     Every QNetIdentity have guaranteed to has unique identity (ushort).
    /// </summary>
    [DisallowMultipleComponent, DefaultExecutionOrder(-5)]
    public sealed class QNetIdentity : QNetObject
    {
        /// <summary>
        ///     A custom Identity defined from editor inspector.
        ///     Utilized only on predefined objects.
        /// </summary>
        [HideInInspector]
        public int CustomIdentity;

        /// <summary>
        ///     Unique identity of this object.
        /// </summary>
        public new ushort Identity { get; private set; }

        /// <summary>
        ///     QNet connection reference of the owner.
        /// </summary>
        public QNetConnection OwnerConnection { get; private set; }

        /// <summary>
        ///     The connection identity of the owner.
        /// </summary>
        public ushort Owner { get; private set; }

        /// <summary>
        ///     False, if this object does not has any owner.
        ///     Objects without owner still has server as a pseudo owner.
        /// </summary>
        public bool HasOwner { get; private set; }

        /// <summary>
        ///     True if local client is a owner of this QNetObject.
        ///     NOTE: Server will always have this property set to false. Use IsServerObject instead.
        /// </summary>
        public new bool IsOwner() => base.IsOwner;

        /// <summary>
        ///     Checks if given connection is a owner of this object.
        /// </summary>
        public new bool IsOwner(QNetConnection connection) =>
            OwnerConnection.ConnectionIdentity == connection.ConnectionIdentity;

        /// <summary>
        ///     Checks if given connection is a owner of this object.
        /// </summary>
        public new bool IsOwner(ushort connectionIdentity) =>
            Owner == connectionIdentity;

        /// <summary>
        ///     List of all QNetObject based component object of this Identity has.
        /// </summary>
        public new List<QNetObject> Objects { get; private set; } = new List<QNetObject>();

        /// <summary>
        ///     List of all custom components added to this QNetIdentity object.
        ///     NOTE: Custom components are only ones added via AddComponent method runtime.
        /// </summary>
        internal Dictionary<byte, byte> CustomObjects { get; } = new Dictionary<byte, byte>();

        /// <summary>
        ///     True, if object is spawned (networked).
        /// </summary>
        public bool IsSpawned { get; private set; }

        /// <summary>
        ///     True, if the object was predefined on current scene.
        /// </summary>
        public bool IsPredefined { get; private set; }

        /// <summary>
        ///     True, if this object is currently pooled.
        /// </summary>
        public bool IsPooled { get; private set; }

        /// <summary>
        ///     True if entity was in pool at least once.
        /// </summary>
        public bool WasPooled { get; private set; }

        /// <summary>
        ///     Defines a network active state of object of this <see cref="QNetIdentity"/> component.
        ///     NOTE: Active state of root objects that contains <see cref="QNetIdentity"/> component should only be changed via
        ///           <see cref="SetNetworkActive"/> method only on the server.
        ///     Disabled objects does not receive any simulation from manager and are completely disabled.
        /// </summary>
        public new bool IsNetworkActive { get; private set; }

        /// <summary>
        ///     Invoke action for each object added to this Identity.
        /// </summary>
        internal void ForEach(Action<QNetObject> e) => Objects.ForEach(e);

        /// <summary>
        ///     Invoke action for every QNetBehaviour based object in this Identity.
        /// </summary>
        internal void ForEachBehaviour(Action<QNetBehaviour> e)
        {
            // NOTE: Because of runtime component add/remove, keep last Objects.Count
            //         value to not call the action on not yet initialized component.
            int c = Objects.Count;
            for (var index = 0; index < c; index++)
            {
                if (index >= Objects.Count)
                    return;

                var o = Objects[index];
                if (o is QNetBehaviour beh)
                    e.Invoke(beh);
            }
        }

#if DEBUG
        /// <summary>
        ///     Returns a string that represents list of <see cref="Objects"/> collection.
        /// </summary>
        internal string GetStringOfCurrentObjects()
        {
            var sb = new StringBuilder();
            for (var index = 0; index < Objects.Count; index++)
            {
                var obj = Objects[index];
                sb.AppendLine($"\t{obj.GetType().Name}: {obj.ComponentIndex}");
            }

            return sb.ToString();
        }
#endif

        /// <inheritdoc />
        protected override void Awake()
        {
            // Invoke base method.
            base.Awake();

            var isNetworkActive = QNetManager.Instance != null && QNetManager.Instance.IsNetworkActive;
            if (!isNetworkActive || !QNetNetworkScene.CanSpawnNetworkObjects)
            {
                // The network is not currently active.
                // Disable this object and wait for network activation.
                gameObject.SetActive(false);

                // Set as predefined.
                IsPredefined = true;

                // Add to the list.
                PredefinedObjects.Add(this);
            }
        }

        /// <inheritdoc />
        public override void InterpolateFrame()
        {
            
        }

        /// <inheritdoc />
        internal override void SimulateFrame()
        {
            // ignore 
        }

        /// <summary>
        ///     Load the identity.
        /// </summary>
        /// <remarks>
        ///     Called only once at <see cref="QNetObject.ServerSpawn"/>
        /// </remarks>
        internal void LoadIdentity(ushort identity, QNetConnection ownerConnection, ushort owner, bool hasOwner)
        {
            Profiler.BeginSample("QNetIdentity.LoadIdentity");

            Identity = identity;
            OwnerConnection = ownerConnection;
            Owner = owner;
            HasOwner = hasOwner;

            // Run a Test Gather for all objects.
            TestGatherObjects();

            // Initialize objects
            InitializeObjects();

            Profiler.EndSample();
        }

        /// <summary>
        ///     Gets all QNetObjects to try initialize them.
        /// </summary>
        /// <remarks>
        ///     Every object is always initializing in Awake method but
        ///      this method is just to make sure that everything is ok.
        /// </remarks>
        private void TestGatherObjects()
        {
            var allObjects = GetComponents<QNetObject>();
            var sortedList = allObjects.OrderBy(o => o.GetType().FullName).ToList();
            for (var index = 0; index < sortedList.Count; index++)
            {
                var obj = sortedList[index];
                InitializeObject(obj);
            }
        }

        /// <summary>
        ///     Initialize all objects added to this Identity.
        /// </summary>
        private void InitializeObjects()
        {
            for (int index = 0; index < Objects.Count; index++)
            {
                var obj = Objects[index];
                if (obj.IsInitialized)
                    continue;

                //// We have problems with objects baked in to the prefab.
                //// We will force componentIndex by identity in sorted list.
                if (!obj.IsCustomComponent)
                {
                    // NOTE: Only for non custom components
                    obj.ComponentIndex = (byte) index;
                }

                // Initialize target object.
                obj.Initialize(Prefab, PrefabIdentity, Identity, OwnerConnection, Owner);
            }
        }

        /// <summary>
        ///     Generates new unique index for new object.
        /// </summary>
        private byte GenerateObjectIndex(bool random)
        {
            byte index = 0;
            while (GetObjectByIndex(index) != null && index <= 250)
            {
                index = !random ? (byte) (index + 1) : (byte) Random.Range(0, byte.MaxValue);
            }

            return index;
        }

        /// <summary>
        ///     Initialize given object as a child of this Identity.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal void InitializeObject([NotNull] QNetObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (Objects.Contains(obj)) return; // object already initialized, dont do anything

            // Update component index.
            // NOTE: For all objects before initialization, componentIndex should be predictable for both client&server peer.
            obj.ComponentIndex = GenerateObjectIndex(IsInitialized);

            // Add new object.
            Objects.Add(obj);

            // Sort list of objects.
            Objects = Objects.OrderBy(o => o.GetType().FullName).ToList();
#if DEBUG && DEEP_DEBUG
            QNetManager.PrintLogMsc("New component added to QNetIdentity. " +
                                   $"(Type: {obj.GetType().Name}, Index: {obj.ComponentIndex}) We have total of {Objects.Count} now.", this);
#endif

            // Initialize new object if not already.
            if (IsInitialized && !obj.IsInitialized)
            {
                obj.Initialize(Prefab, PrefabIdentity, Identity, OwnerConnection, Owner);
            }
        }

        /// <summary>
        ///     Returns the QNetObject based component added to this QNetIdentity by index in local array.
        /// </summary>
        [CanBeNull]
        internal QNetObject GetObjectByIndex(byte index)
        {
            for (var i = 0; i < Objects.Count; i++)
            {
                var obj = Objects[i];
                if (obj.ComponentIndex != index) continue;
                return obj;
            }

            return null;
        }

        /// <summary>
        ///     Spawn the entity.
        /// </summary>
        internal void Spawn(Vector3 position, Quaternion rotation, Vector3 scale, bool activated)
        {
#if DEBUG && DEEP_DEBUG
            QNetManager.PrintLogMsc($"QNetIdentity.Spawn({position}, {rotation}, {scale}, {activated})", this);
#endif

            // Apply transform.
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = scale;

            // Update the state.
            IsPooled = false;
            IsSpawned = true;

            // Activate object.
            SetNetworkActive(activated);

            // call onNetworkSpawned.
            ForEachBehaviour(b => b.CallOnNetworkSpawned());

            if (IsServer)
            {
                // Call late spawn here cuz server will newer de-serialize objects :)
                ForEachBehaviour(b => b.CallOnLateNetworkSpawned());
            }
        }

        /// <summary>
        ///     Instead of destroying entity, disable it and return to pool.
        /// </summary>
        internal void Pool()
        {
            if (IsPooled)
            {
                // Already pooled!
                return;
            }

            WasPooled = true;
            IsPooled = true;

            // Disable object.
            SetNetworkActive(false);

            // call onPooled.
            ForEachBehaviour(b => b.CallOnPooled());
        }

        /// <summary>
        ///     Updates the active state of the object.
        ///     NOTE: This method is always called by <see cref="Spawn"/> to activate or <see cref="Pool"/> to de-activate the object.
        /// </summary>
        public void SetNetworkActive(bool activeState)
        {
#if DEBUG && DEEP_DEBUG
            QNetManager.PrintLogMsc($"QNetIdentity.SetNetworkActive({activeState})", this);
#endif

            if (IsNetworkActive == activeState && gameObject.activeSelf == activeState)
            {
                return;
            }

            // Set the state.
            IsNetworkActive = activeState;

            // Update object active state.
            gameObject.SetActive(activeState);

            // call onNetworkActiveChange.
            ForEachBehaviour(b => b.CallOnNetworkActiveChange(activeState));

            if (IsServer)
            {
                // Generate outgoing server message.
                var outgoingMessage = QNetManager.GenerateServerMessage(QNetUnityHeader.OBJECT_ACTIVATION);

                // Write activation state.
                outgoingMessage.WriteUInt16(Identity);
                outgoingMessage.WriteBool(activeState);

                // Send server message to all clients.
                QNetManager.SendToAll(QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered, outgoingMessage);
            }
        }

        /// <summary>
        ///     Serialize all objects of this Identity.
        /// </summary>
        internal void SerializeAllObjects(QNetMessageWriter writer)
        {
            for (var index = 0; index < Objects.Count; index++)
            {
                var obj = Objects[index];
                obj.CallOnSerializeServerState(writer);
            }
        }

        /// <summary>
        ///     De-serialize state of all objects of this Identity.
        /// </summary>
        internal void DeserializeAllObjects(QNetMessageReader reader)
        {
            for (var index = 0; index < Objects.Count; index++)
            {
                var obj = Objects[index];
                obj.CallOnDeserializeServerState(reader);
            }
        }

        /// <summary>
        ///     Gets the predefined object of given identity.
        /// </summary>
        internal static QNetIdentity GetPredefinedObject(ushort objectIdentity)
        {
            for (var index = 0; index < PredefinedObjects.Count; index++)
            {
                var pre = PredefinedObjects[index];
                if (pre.CustomIdentity == objectIdentity)
                    return pre;
            }

            return null;
        }

        /// <summary>
        ///     List of all predefined objects in current scene that need to be activated on network activation.
        /// </summary>
        internal static List<QNetIdentity> PredefinedObjects { get; } = new List<QNetIdentity>();

        public static implicit operator ushort(QNetIdentity identity) => identity.Identity;
    }
}
