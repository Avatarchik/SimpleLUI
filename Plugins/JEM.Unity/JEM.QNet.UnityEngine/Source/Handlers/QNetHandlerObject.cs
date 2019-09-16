//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Objects;
using JEM.UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

// ReSharper disable RedundantAssignment

namespace JEM.QNet.UnityEngine.Handlers
{
    // QNetObject related network header.
    // Contains methods to de-serialize objects on clients same as send it's queries.
    internal static class QNetHandlerObject
    {
        private struct LocalSerializedObject
        {
            public QNetObjectSerialized Object;
            public QNetMessageReader Reader;
        }

        /// <summary>
        ///     Header handle for QNetUnityHeader.OBJECT_QUERY
        ///     Called by server to add or remove component from QNetIdentity object.
        /// </summary>
        public static void OnClientObjectQuery(QNetMessage message, QNetMessageReader reader, 
            ref bool disallowRecycle)
        {
            // As server(host), ignore object queries.
            if (QNetManager.Instance.IsServerActive)
            {
                return;
            }

            var objectIdentity = reader.ReadUInt16();
            var operationType = reader.ReadEnum<QNetObjectComponentQuery>();

            // Get the object.
            var qNetObject = QNetObject.GetObject(objectIdentity);
            if (qNetObject == null)
            {
                QNetManager.PrintLogWarning("Local client has received a ClientObjectQuery " +
                                            $"for object of identity {objectIdentity} that not exist in local network scene.");
                return;
            }

            switch (operationType)
            {
                case QNetObjectComponentQuery.AddComponent:
                    // Get component type.
                    var typeIndex = reader.ReadByte();
                    AddCustomComponent(qNetObject, typeIndex, true);
                    break;
                case QNetObjectComponentQuery.DestroyComponent:
                    // Destroy!
                    var componentIdentity = reader.ReadByte();
                    qNetObject.GetObjectByIndex(componentIdentity).DestroyCustomComponent(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null);
            }
        }

        /// <summary>
        ///     Adds new component of given type index to given object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        private static void AddCustomComponent([NotNull] QNetObject qNetObject, byte typeIndex, bool full)
        {
            if (qNetObject == null) throw new ArgumentNullException(nameof(qNetObject));
            if (!QNetObject.GetQNetObjectType(typeIndex, out var type))
                throw new NullReferenceException($"Failed to add new component to QNetIdentity object({(ushort) qNetObject.Identity}). " +
                                                 $"Type index type of type index {typeIndex} not exist.");

            // Add component.
            QNetObject.ClientIsModifyingCustomComponent = true;
            var component = qNetObject.gameObject.AddComponent(type) as QNetObject;
            QNetObject.ClientIsModifyingCustomComponent = false;
            if (component == null)
                throw new NullReferenceException("Failed to add new component. " +
                                                 "AddComponent returned null or received value can't be converted to QNetObject based type.");

            // Initialize custom component.
            component.InitializeCustomComponent(typeIndex, full);
        }

        /// <summary>
        ///     Header handle for QNetUnityHeader.OBJECT_CREATE.
        ///     Called by server to create QNetObject.
        /// </summary>
        internal static void OnServerObjectCreate(QNetMessage message, QNetMessageReader reader,
            ref bool disallowRecycle)
        {
            // Read and write new QNetObject
            var qNetObject = reader.ReadMessage<QNetObjectSerialized>();
            WriteSerializedObject(new LocalSerializedObject
            {
                Object = qNetObject,
                Reader = reader
            });

            // Disallow to recycle this message.
            // We can't allow to do that because of Reader and it's late utilization by second phase initialization.
            // NOTE: Message will be still recycled but after the QNetObject spawn.
            disallowRecycle = true;
        }

        /// <summary>
        ///     Header handle for QNetUnityHandler.OBJECT_DELETE.
        ///     Called by server to destroy QNetObject.
        /// </summary>
        internal static void OnServerObjectDelete(QNetMessage message, QNetMessageReader reader, ref bool disallowRecycle)
        {
            // Read what QNetObject to delete.
            var objectIdentity = reader.ReadUInt16();
            // And process.
            RemoveSerializedObject(objectIdentity);
        }

        /// <summary>
        ///     Header handle for QNetUnityHeader.OBJECT_STATE.
        ///     Called by the server to de-serialize object's state.
        ///     This may by received only if something in server call <see cref="QNetObject.SerializeAndSendServerState"/>.
        /// </summary>
        internal static void OnServerObjectState(QNetMessage message, QNetMessageReader reader, ref bool disallowRecycle)
        {
            // As server(host), custom state deserialization should be always ignore.
            if (QNetManager.Instance.IsServerActive)
            {
                return;
            }

            if (!QNetNetworkScene.CanSpawnNetworkObjects || QNetNetworkScene.IsSecondPhaseRunning)
                return; // As the network object can't be currently spawned, ignore the incoming objects states.

            // Read header.
            var objectIdentity = reader.ReadUInt16();
            var componentIndex = reader.ReadByte();

            // Get object.
            var qNetObject = QNetObject.GetObject(objectIdentity);
            if (qNetObject == null)
            {
                QNetManager.PrintLogWarning("Local client has received a ServerObjectState " +
                                            $"for object of identity {objectIdentity} that not exist in local network scene.");
                return;
            }

            // Get component and de-serialize.
            var component = qNetObject.GetObjectByIndex(componentIndex);
            component.CallOnDeserializeServerState(reader);
        }

        private static void WriteSerializedObject(LocalSerializedObject serializedObject)
        {
            // As server(host), ignore write serialized object just for sure
            if (QNetManager.Instance.IsServerActive)
            {
                QNetManager.PrintLogWarning("WriteSerializedObject was received while server is active. " +
                                            $"The message will be ignored. (Object identity: {serializedObject.Object.ObjectIdentity})");

                QNetManager.RecycleClient(serializedObject.Reader);
                return;
            }

            // Check if this is not a duplicate.
            if (!GetSerializedObject(serializedObject.Object.ObjectIdentity).Equals(default(LocalSerializedObject)))
            {
                // Duplicate detected.
                // TODO: Overwrite serialized data?
                return;
            }

            if (QNetNetworkScene.SceneState != QNetSceneState.Loaded)
            {
                // The network scene was not loaded yet.
                // Just add serialized object to list.
                SerializedObjects.Add(serializedObject);
            }
            else
            {
                // The network scene is loaded.
                // Spawn the object instantly.
                JEMOperation.StartCoroutine(FromSerializedObject(serializedObject));
            }
        }

        private static void RemoveSerializedObject(ushort objectIdentity)
        {
            // As server(host), ignore remove serialized object.
            if (QNetManager.Instance.IsServerActive)
            {
                return;
            }

            if (QNetNetworkScene.SceneState != QNetSceneState.Loaded)
            {
                // The network scene was not loaded yet.
                // Try to just remove the serialized object from list.
                var obj = GetSerializedObject(objectIdentity);
                if (obj.Equals(default(LocalSerializedObject)))
                {
                    // Serialized object not found.
                    // It may by that this object has been already spawned.
                    // Destroy created QNetObject.
                }
                else
                {
                    // Serialized object found.
                    // Just remove it from the list.
                    SerializedObjects.Remove(obj);
                    return;
                }
            }

            var qNetObject = QNetObject.GetObject(objectIdentity);
            if (qNetObject == null)
            {
                // Target object does not exist in current scene.
                // We can't do anything.
                return;
            }

            QNetObject.LocalDestroy(qNetObject);
        }

        /// <summary>
        ///     De-serializes all objects added to SerializedObjects list.
        ///     Called by <see cref="QNetNetworkScene.RunClientInitializationSecondPhase"/>
        /// </summary>
        internal static IEnumerator DeserializeAllObjects()
        {
            for (var index = 0; index < SerializedObjects.Count; index++)
            {
                var obj = SerializedObjects[index];
                yield return FromSerializedObject(obj);
            }

            SerializedObjects.Clear();
        }

        /// <summary>
        ///     Create QNetObject from LocaleSerializedObject data.
        /// </summary>
        private static IEnumerator FromSerializedObject(LocalSerializedObject obj)
        {
            // Check if this object is a predefined one.
            QNetIdentity qnetObject = QNetIdentity.GetPredefinedObject(obj.Object.ObjectIdentity);
            yield return qnetObject;

            if (qnetObject != null)
            {
                // Activate this predefined object.
                QNetObject.ActivatePredefinedObject(qnetObject);
            }
            else
            {
                if (obj.Object.PrefabIdentity == 0)
                {
                    throw new NullReferenceException($"Object of prefab identity 0 has been received. (Object Identity: {obj.Object.ObjectIdentity})");
                }

                var objectPrefab = QNetManager.Instance.DatabaseReference.GetPrefab(obj.Object.PrefabIdentity);
                if (objectPrefab == null)
                {
                    // Manual message recycling.
                    QNetManager.RecycleClient(obj.Reader);

                    throw new NullReferenceException("Unable to de-serialized QNetObject. " +
                                                     $"Not exist or missing QNetObjectPrefab reference were received ({obj.Object.PrefabIdentity}).");
                }

                // Create local object.
                qnetObject = QNetObject.LocalSpawn(objectPrefab.Prefab, objectPrefab.PrefabIdentity, obj.Object.ObjectIdentity, default, obj.Object.OwnerIdentity);
                yield return qnetObject;
            }

            // Create custom components.
            for (var index = 0; index < obj.Object.CustomComponents.Length; index++)
            {
                var typeIndex = obj.Object.CustomComponents[index];
                AddCustomComponent(qnetObject, typeIndex, false);
            }

            // Spawn.
            qnetObject.Spawn(obj.Object.Position, obj.Object.Rotation, obj.Object.Scale);

            // Deserialize his state.
            qnetObject.DeserializeAllObjects(obj.Reader);

            // Manual message recycling.
            QNetManager.RecycleClient(obj.Reader);
        }

        private static LocalSerializedObject GetSerializedObject(ushort objectIdentity)
        {
            foreach (var s in SerializedObjects)
            {
                if (s.Object.ObjectIdentity == objectIdentity)
                    return s;
            }

            return default;
        }

        internal static int SerializedObjectsCount => SerializedObjects.Count;
        private static List<LocalSerializedObject> SerializedObjects { get; } = new List<LocalSerializedObject>();
    }
}