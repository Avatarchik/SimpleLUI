//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine.Handlers;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
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
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefab prefab, Vector2 position, float angle, bool networkActive = true) =>
            ServerSpawn(prefab, position, angle, Vector2.one, networkActive);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefab prefab, Vector2 position, float angle, Vector2 scale, bool networkActive = true) =>
            ServerSpawn(prefab.Prefab, prefab.PrefabIdentity, position, Quaternion.Euler(0f, 0f, angle), scale, networkActive);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity,
            Vector2 position, float angle, bool networkActive = true) => ServerSpawn(prefab, prefabIdentity, position, Quaternion.Euler(0f, 0f, angle), Vector3.one, networkActive);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefab prefab, Vector3 position, Quaternion rotation, bool networkActive = true) =>
            ServerSpawn(prefab, position, rotation, Vector3.one, networkActive);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefab prefab, Vector3 position, Quaternion rotation, Vector3 scale, bool networkActive = true) => 
            ServerSpawn(prefab.Prefab, prefab.PrefabIdentity, position, rotation, scale, networkActive);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity,
            Vector3 position, Quaternion rotation, bool networkActive = true) => ServerSpawn(prefab, prefabIdentity, position, rotation, Vector3.one, networkActive);
        
        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity,
            Vector3 position, Quaternion rotation, Vector3 scale, bool networkActive = true)
        {
            var obj = LocalSpawn(prefab, prefabIdentity);
            ServerSpawn(obj, position, rotation, scale, networkActive);
            return obj;
        }

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefab prefab, QNetConnection ownerConnection,
            Vector2 position, float angle, bool networkActive = true) => ServerSpawn(prefab, ownerConnection, position, angle, Vector2.one, networkActive);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefab prefab, QNetConnection ownerConnection,
            Vector2 position, float angle, Vector2 scale, bool networkActive = true) =>
            ServerSpawn(prefab.Prefab, prefab.PrefabIdentity, ownerConnection, position, Quaternion.Euler(0f, 0f, angle), scale, networkActive);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity,
            QNetConnection ownerConnection, Vector2 position, float angle, bool networkActive = true) => 
            ServerSpawn(prefab, prefabIdentity, ownerConnection, position, Quaternion.Euler(0f, 0f, angle), Vector3.one, networkActive);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefab prefab, QNetConnection ownerConnection,
            Vector3 position, Quaternion rotation, bool networkActive = true) => ServerSpawn(prefab, ownerConnection, position, rotation, Vector3.one, networkActive);
        
        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetObjectPrefab prefab, QNetConnection ownerConnection,
            Vector3 position, Quaternion rotation, Vector3 scale, bool networkActive = true) =>
            ServerSpawn(prefab.Prefab, prefab.PrefabIdentity, ownerConnection, position, rotation, scale, networkActive);

        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity, 
            QNetConnection ownerConnection, Vector3 position, Quaternion rotation, bool networkActive = true) => 
            ServerSpawn(prefab, prefabIdentity, ownerConnection, position, rotation, Vector3.one, networkActive);
        
        /// <summary>
        ///     Spawn a new network based object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity ServerSpawn([NotNull] QNetIdentity prefab, ushort prefabIdentity, 
            QNetConnection ownerConnection, Vector3 position, Quaternion rotation, Vector3 scale, bool networkActive = true)
        {
            var obj = LocalSpawn(prefab, prefabIdentity, ownerConnection);
            ServerSpawn(obj, position, rotation, scale, networkActive);
            return obj;
        }

        /// <summary>
        ///     Spawns given <paramref name="identity"/> on active network.
        ///     NOTE: This only spawns a already existing object on the scene.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void ServerSpawn([NotNull] QNetIdentity identity, Vector3 position, Quaternion rotation, Vector3 scale,
            bool networkActive = true)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));
            identity.Spawn(position, rotation, scale, networkActive);
            QNetNetworkScene.SendObjectToAllConnections(identity);
        }

        /// <summary>
        ///     Spawns given <paramref name="identity"/> on active network.
        ///     NOTE: This only spawns a already existing object on the scene.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void ServerSpawn([NotNull] QNetIdentity identity, Vector2 position, float angle,
            bool networkActive = true)
        {
            if (identity == null) throw new ArgumentNullException(nameof(identity));
            identity.Spawn(position, Quaternion.Euler(0f, 0f, angle), Vector3.one, networkActive);
            QNetNetworkScene.SendObjectToAllConnections(identity);
        }

        /// <summary>
        ///     Spawn a new network based object only in local scene.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity LocalSpawn([NotNull] QNetObjectPrefab prefab) => LocalSpawn(prefab.Prefab, prefab.PrefabIdentity);

        /// <summary>
        ///     Spawn a new network based object only in local scene.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static QNetIdentity LocalSpawn([NotNull] QNetObjectPrefab prefab, QNetConnection ownerConnection) =>
            LocalSpawn(prefab.Prefab, prefab.PrefabIdentity, ownerConnection, ownerConnection.ConnectionIdentity);

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
            QNetConnection ownerConnection, ushort owner) =>
            LocalSpawn(prefab, prefabIdentity, ResolveUniqueObjectIdentity(), ownerConnection, owner);
        
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

            Profiler.BeginSample("QNetObject.LocalSpawn");
            var entity = GetObject(objectIdentity);
            if (entity != null && !entity.IsPooled)
            {
                Profiler.EndSample();

                if (IsServer)
                {
                    throw new InvalidOperationException("Failed to spawn new local QNetObject based object. " +
                                                        $"Identity '{objectIdentity}' is already in use.");
                }

                // Debug.Log($"Trying to force local initialization on {objectIdentity}");
                // As non server peers have zero control over spawned object localy,
                //  we may force local peer to reinitialize entity of this identity instead of throwing error.
                LocalDestroy(entity);
            }

            // Check if prefab identity exists.
            if (!QNetManager.Instance.DatabaseReference.GetPrefab(prefabIdentity).IsValid)
            {
                Profiler.EndSample();
                throw new ArgumentException("Unable to spawn networked object. The prefab identity of value " +
                                            $"{prefabIdentity} not exist or has been not registered.",
                    nameof(prefabIdentity));
            }

            if (entity == null)
            {
                entity = GetPooledObject(prefabIdentity);
                if (entity == null)
                {
                    var obj = Instantiate(prefab.gameObject);
                    entity = obj.GetComponent<QNetIdentity>();
                }
            }

            entity.Initialize(prefab, prefabIdentity, objectIdentity, ownerConnection, owner);
            Objects.Add(entity);

#if DEBUG && DEEP_DEBUG
            QNetManager.PrintLogMsc("New QNetObject spawned localy. " +
                                    $"Prefab: {prefabIdentity}, Object: {objectIdentity}, Owner: {owner}", obj);
#endif

            Profiler.EndSample();
            return entity;
        }

        /// <summary>
        ///     Destroy given QNetObject from server so it will also destroy this object instance on all connected clients.
        /// </summary>
        /// <param name="obj"/>
        /// <param name="canPool">If false, instead of pooling the object, it will be permanently destroyed from scene.</param>
        public static void ServerDestroy(QNetObject obj, bool canPool = true)
        {
            if (obj == null) return;
            if (!QNetManager.Instance.IsServerActive)
                throw new InvalidOperationException("You can only destroy QNetObjects from server.");

            // Send destroy message to all connections.
            QNetNetworkScene.DestroyObjectOnAllConnections(obj, canPool);

            // And then destroy this object in local scene.
            LocalDestroy(obj, canPool);
        }

        /// <summary>
        ///     Destroy given <see cref="QNetObject"/> only in local scene.
        /// </summary>
        /// <param name="obj"/>
        /// <param name="canPool">If false, instead of pooling the object, it will be permanently destroyed from scene.</param>
        public static void LocalDestroy(QNetObject obj, bool canPool = true)
        {
            if (obj == null) return;
            Profiler.BeginSample("QNetObject.LocalDestroy");
            if (AlwaysReturnToPool && canPool)
            {
                obj.Identity.Pool();
                Profiler.EndSample();
                return;
            }

            obj.Identity.ForEach(o => o.DestroyObject());
            Objects.Remove(obj.Identity);
            if (obj.Identity.IsPredefined)
                obj.gameObject.SetActive(false); // Instead of destroying predefined object, we should just disable them.
            else
            {
                Destroy(obj.gameObject);
            }
            Profiler.EndSample();
        }

        /// <summary>
        ///     Destroy all local objects on the active scene.
        /// </summary>
        /// <remarks>
        ///     It's most likely that this method has been called just by <see cref="QNetNetworkScene.UnloadNetworkScene"/>
        ///     to clear the objects.
        /// </remarks>
        internal static void LocalDestroyAll()
        {
            for (int index = 0; index < Objects.Count; index++)
            {
                var obj = Objects[index];
                if (obj == null)
                {
                    continue;
                }

                obj.ForEach(o => o.DestroyObject());
                Destroy(obj.gameObject);
            }

            Objects.Clear();
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
            Profiler.BeginSample("QNetObject.SpawnPredefinedObjects");

            for (var index = 0; index < QNetIdentity.PredefinedObjects.Count; index++)
            {
                var obj = QNetIdentity.PredefinedObjects[index];
                if (obj == null) continue;
                if (GetObject(obj) != null)
                    throw new NotSupportedException("Failed to spawn predefined object. " +
                                                    $"QNetObject of identity {obj.Identity} already exists in network scene.");

                // Activate object.
                ActivatePredefinedObject(obj);

                // Spawn
                obj.Spawn(obj.transform.position, obj.transform.rotation, obj.transform.localScale, obj.gameObject.activeSelf);
            }

            QNetIdentity.PredefinedObjects.Clear();

            Profiler.EndSample();
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
        ///     Try to get free object from pool that has prefab of given identity.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        [CanBeNull]
        internal static QNetIdentity GetPooledObject(ushort prefabIdentity)
        {
            if (prefabIdentity == 0) throw new ArgumentOutOfRangeException(nameof(prefabIdentity));
            for (var index = 0; index < Objects.Count; index++)
            {
                var obj = Objects[index];
                if (obj.IsPooled && obj.PrefabIdentity == prefabIdentity)
                    return obj;
            }

            return null;
        }

        /// <summary>
        ///     Returns a entity of given identity.
        /// </summary>
        [CanBeNull]
        public static QNetIdentity GetObject(ushort objectIdentity) => GetObject(objectIdentity, false);
        
        /// <summary>
        ///     Returns a entity of given identity.
        /// </summary>
        [CanBeNull]
        public static QNetIdentity GetObject(ushort objectIdentity, bool includePooledObjects)
        {
            if (objectIdentity == 0) return null;
            for (var index = 0; index < Objects.Count; index++)
            {
                var obj = Objects[index];
                if (obj.Identity == objectIdentity)
                {
                    if (obj.IsPooled && !includePooledObjects)
                        return null;

                    return obj;
                }
            }

            return null;
        }

        /// <summary>
        ///     If enabled, instead of localy destroying the object, system will disable and tag them as pooled
        ///     so they could be utilized by new object of the same prefab identity.
        /// </summary>
        public static bool AlwaysReturnToPool { get; set; } = true;

        /// <summary>
        ///     List of all spawned QNetIdentities in scene.
        /// </summary>
        public static List<QNetIdentity> Objects { get; } = new List<QNetIdentity>();
    }
}
