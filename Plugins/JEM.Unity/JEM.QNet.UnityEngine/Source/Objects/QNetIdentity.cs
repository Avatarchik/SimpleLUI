//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Objects
{
    /// <inheritdoc />
    /// <summary>
    ///     QNet Identity component.
    ///     Defines a network object in scene and enables communication between peers.
    ///     Every QNetIdentity have guaranteed to has unique identity (ushort).
    /// </summary>
    [DisallowMultipleComponent]
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
        public new List<QNetObject> Objects { get; } = new List<QNetObject>();

        /// <summary>
        ///     True, if object is spawned (networked).
        /// </summary>
        public bool IsSpawned { get; private set; }

        /// <summary>
        ///     True, if the object was predefined on current scene.
        /// </summary>
        public bool IsPredefined { get; private set; }

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

        /// <summary>
        ///     List of all custom components added to this QNetIdentity object.
        ///     NOTE: Custom components are only ones added via AddComponent method runtime.
        /// </summary>
        internal Dictionary<byte, byte> CustomComponentsAdded { get; } = new Dictionary<byte, byte>();

        /// <summary>
        ///     Returns a byte array of types of all custom components added to this Identity.
        /// </summary>
        internal byte[] GetArrayOfCustomComponentsTypes()
        {
            byte[] array = new byte[CustomComponentsAdded.Count];
            int index = 0;
            foreach (var c in CustomComponentsAdded)
            {
                array[index] = c.Value;
            }

            return array;
        }

        private void Awake()
        {
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
        internal override void InterpolateFrame()
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
            Identity = identity;
            OwnerConnection = ownerConnection;
            Owner = owner;
            HasOwner = hasOwner;

            // Run a Test Gather for all objects.
            TestGatherObjects();

            // Initialize objects
            InitializeObjects();
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
            for (var index = 0; index < allObjects.Length; index++)
            {
                var obj = allObjects[index];
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

                obj.Initialize(Prefab, PrefabIdentity, Identity, OwnerConnection, Owner);
            }
        }

        /// <summary>
        ///     Generates new index for new object.
        /// </summary>
        private byte GenerateObjectIndex()
        {
            byte index = 0;
            while (GetObjectByIndex(index) != null && index <= 250)
            {
                index++;
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
            obj.ComponentIndex = GenerateObjectIndex();
            // Add new object.
            Objects.Add(obj);

#if DEBUG
            QNetManager.PrintLogMsc("New component added to QNetIdentity. " +
                                   $"(Type: {obj.GetType().Name}) We have total of {Objects.Count} now.", this);
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
        internal QNetObject GetObjectByIndex(byte index) => Objects.FirstOrDefault(obj => obj.ComponentIndex == index);    

        /// <summary>
        ///     Spawn the entity.
        /// </summary>
        internal void Spawn(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = scale;

            IsSpawned = true;

            // call onNetworkSpawned.
            ForEachBehaviour(e => e.CallOnNetworkSpawned());
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
            foreach (var pre in PredefinedObjects)
            {
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
