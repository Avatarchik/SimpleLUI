//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using JEM.QNet.UnityEngine.Handlers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JEM.QNet.UnityEngine.Objects
{
    public abstract partial class QNetObject
    {
        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefabPair prefab, Vector3 position, Quaternion rotation) =>
            ServerSpawn(prefab, position, rotation, Vector3.one);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefabPair prefab, Vector3 position, Quaternion rotation, Vector3 scale) => 
            ServerSpawn(prefab.Prefab, prefab.PrefabIdentity, position, rotation, scale);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity,
            Vector3 position, Quaternion rotation) => ServerSpawn(prefab, prefabIdentity, position, rotation, Vector3.one);
        
        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity,
            Vector3 position, Quaternion rotation, Vector3 scale)
        {
            var obj = LocalSpawn(prefab, prefabIdentity);
            obj.Spawn(position, rotation, scale);
            QNetNetworkScene.SendObjectToAllConnections(obj);
            return obj;
        }

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefabPair prefab, QNetConnection ownerConnection,
            Vector3 position, Quaternion rotation) => ServerSpawn(prefab, ownerConnection, position, rotation, Vector3.one);
        
        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefabPair prefab, QNetConnection ownerConnection,
            Vector3 position, Quaternion rotation, Vector3 scale) =>
            ServerSpawn(prefab.Prefab, prefab.PrefabIdentity, ownerConnection, position, rotation, scale);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity, 
            QNetConnection ownerConnection, Vector3 position, Quaternion rotation) => ServerSpawn(prefab, prefabIdentity, ownerConnection, position, rotation, Vector3.one);
        
        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity, 
            QNetConnection ownerConnection, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            var obj = LocalSpawn(prefab, prefabIdentity, ownerConnection);
            obj.Spawn(position, rotation, scale);
            QNetNetworkScene.SendObjectToAllConnections(obj);
            return obj;
        }

        /// <summary>
        ///     Spawn a new network based object only in local scene.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity LocalSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity) => LocalSpawn(prefab, prefabIdentity, default);
        
        /// <summary>
        ///     Spawn a new network based object only in local scene.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity LocalSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity, QNetConnection ownerConnection) => 
            LocalSpawn(prefab, prefabIdentity, ownerConnection, ownerConnection.ConnectionIdentity);

        /// <summary>
        ///     Spawn a new network based object only in local scene.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity LocalSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity,
            QNetConnection ownerConnection, ushort owner)
        {
            return LocalSpawn(prefab, prefabIdentity, ResolveUniqueObjectIdentity(), ownerConnection, owner);
        }

        /// <summary>
        ///     Spawn a new network based object only in local scene.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity LocalSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity, ushort objectIdentity,
            QNetConnection ownerConnection, ushort owner)
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            if (prefabIdentity == 0) throw new ArgumentException("Prefab identity can't be set to zero.", nameof(prefabIdentity));
            if (objectIdentity == 0) throw new ArgumentException("Object identity can't be set to zero.", nameof(objectIdentity));
            if (!QNetNetworkScene.CanSpawnNetworkObjects)
                throw new InvalidOperationException("You can't currently spawn QNetObject based object. Is the network active?");
            if (GetObject(objectIdentity) != null)
                throw new InvalidOperationException($"Failed to spawn new local QNetObject. Identity {objectIdentity} is already in use.");

            // Check if prefab identity exists
            if (QNetManager.Instance.DatabaseReference.GetPrefab(prefabIdentity) == null)
                throw new ArgumentException("Unable to spawn networked object. The prefab identity of value " +
                                            $"{prefabIdentity} not exist or has been not registered.",
                    nameof(prefabIdentity));

            var obj = Instantiate(prefab.gameObject);
            var entity = obj.GetComponent<QNetIdentity>();
            entity.Initialize(prefab, prefabIdentity, objectIdentity, ownerConnection, owner);
            Objects.Add(entity);

#if DEBUG
            QNetManager.PrintLogMsc("New QNetObject spawned localy. " +
                                    $"Prefab: {prefabIdentity}, Object: {objectIdentity}, Owner: {owner}", obj);
#endif
            return entity;
        }

        /// <summary>
        ///     Destroy given QNetObject from server so it will also destroy this object instance on all connected clients.
        /// </summary>
        public static void ServerDestroy(QNetObject obj)
        {
            if (obj == null) return;
            if (!QNetManager.Instance.IsServerActive)
                throw new InvalidOperationException("You can only destroy QNetObjects from server.");

            // Send destroy message to all connections.
            QNetNetworkScene.DestroyObjectOnAllConnections(obj);

            // And then destroy this object in local scene.
            LocalDestroy(obj);
        }

        /// <summary>
        ///     Destroy given QNetObject only in local scene.
        /// </summary>
        /// <param name="obj"></param>
        public static void LocalDestroy(QNetObject obj)
        {
            if (obj == null) return;
            // TODO: Disable back predefined objects instead of destroying them.
            obj.Identity.ForEach(o => o.DestroyObject());
            Objects.Remove(obj.Identity);
            Destroy(obj.gameObject);
        }

        /// <summary>
        ///     Activates the predefined object.
        ///     Called by both <see cref="SpawnPredefinedObjects"/> and <see cref="QNetHandlerObject.FromSerializedObject"/>
        /// </summary>
        /// <remarks>
        ///     Object activation includes Objects.Add();
        /// </remarks>
        internal static void ActivatePredefinedObject([NotNull] QNetIdentity obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            obj.gameObject.SetActive(true);

            obj.Initialize(null, 0, (ushort) obj.CustomIdentity, default, 0);
            Objects.Add(obj);
        }

        /// <summary>
        ///     Activates all the predefined objects in scene.
        /// </summary>
        internal static void SpawnPredefinedObjects()
        {
            for (var index = 0; index < QNetIdentity.PredefinedObjects.Count; index++)
            {
                var obj = QNetIdentity.PredefinedObjects[index];
                if (GetObject(obj) != null)
                    throw new NotSupportedException("Failed to spawn predefined object. " +
                                                    $"QNetObject of identity {obj.Identity} already exists in network scene.");

                // Activate object.
                ActivatePredefinedObject(obj);

                // Spawn
                obj.Spawn(obj.transform.position, obj.transform.rotation, obj.transform.localScale);
            }

            QNetIdentity.PredefinedObjects.Clear();
        }

        /// <summary>
        ///     Returns an always unique Identity for QNetIdentity component.
        /// </summary>
        public static ushort ResolveUniqueObjectIdentity()
        {
            ushort identity = 0;
            while (identity == 0 || GetObject(identity) != null)
            {
                identity = RandomizedUnsignedShort();
            }

            return identity;
        }

        /// <summary>
        ///     Returns a pseudo random unsigned short value.
        /// </summary>
        internal static ushort RandomizedUnsignedShort() => (ushort) Random.Range(ushort.MinValue, ushort.MaxValue);

        /// <summary>
        ///     Returns a entity of given identity.
        /// </summary>
        [CanBeNull]
        internal static QNetIdentity GetObject(ushort entityIdentity)
        {
            if (entityIdentity == 0) return null;
            for (var index = 0; index < Objects.Count; index++)
            {
                var e = Objects[index];
                if (e.Identity == entityIdentity)
                    return e;
            }

            return null;
        }

        /// <summary>
        ///     List of all spawned QNetIdentities in scene.
        /// </summary>
        internal static List<QNetIdentity> Objects { get; } = new List<QNetIdentity>();
    }
}
