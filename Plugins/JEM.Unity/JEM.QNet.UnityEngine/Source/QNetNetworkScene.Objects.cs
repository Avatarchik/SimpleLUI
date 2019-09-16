//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Messages;
using JEM.QNet.UnityEngine.Objects;
using JetBrains.Annotations;
using System;

namespace JEM.QNet.UnityEngine
{
    /// <summary>
    ///     Utility methods related to networked scene.
    /// </summary>
    /// <remarks>
    ///     You may only invoke methods from this utility class on the server-side.
    /// </remarks>
    public static partial class QNetNetworkScene
    {
        /// <summary>
        ///     Destroys all objects that given connection owns.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public static void DestroyAllOwnedObjectsOfConnection(QNetConnection connection)
        {
            if (!QNetManager.Instance.IsServerActive)
                throw new InvalidOperationException("This methods can only be used by server.");

            for (var index = 0; index < QNetObject.Objects.Count; index++)
            {
                var obj = QNetObject.Objects[index];
                if (!obj.HasOwner || !obj.IsOwner(connection)) continue;
                QNetObject.ServerDestroy(obj);
            }
        }

        /// <summary>
        ///     Sends destroy message of given object to all connections.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentNullException"/>
        public static void DestroyObjectOnAllConnections([NotNull] QNetObject qNetObject)
        {
            if (!QNetManager.Instance.IsServerActive)
                throw new InvalidOperationException("This methods can only be used by server.");
            if (qNetObject == null) throw new ArgumentNullException(nameof(qNetObject));
            QNetManager.SendToAll(QNetUnityChannel.OBJECT_QUERY, QNetMessageMethod.ReliableOrdered,
                QNetUnityHeader.OBJECT_DELETE, (ushort) qNetObject.Identity);
        }

        /// <summary>
        ///     Sends given QNetObject to given connection.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentNullException"/>
        public static void SendObjectToConnection(QNetConnection connection, [NotNull] QNetObject qNetObject)
        {
            if (!QNetManager.Instance.IsServerActive)
                throw new InvalidOperationException("This methods can only be used by server.");
            if (qNetObject == null) throw new ArgumentNullException(nameof(qNetObject));
            QNetManager.Send(connection, QNetUnityChannel.OBJECT_QUERY, QNetMessageMethod.ReliableOrdered,
                ResolveObjectCreateMessage(qNetObject));
        }

        /// <summary>
        ///     Sends given QNetObject to all connections.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentNullException"/>
        public static void SendObjectToAllConnections([NotNull] QNetObject qNetObject)
        {
            if (!QNetManager.Instance.IsServerActive)
                throw new InvalidOperationException("This methods can only be used by server.");
            if (qNetObject == null) throw new ArgumentNullException(nameof(qNetObject));
            QNetManager.SendToAll(QNetUnityChannel.OBJECT_QUERY, QNetMessageMethod.ReliableOrdered,
                ResolveObjectCreateMessage(qNetObject));
        }

        /// <summary>
        ///     Sends all QNetObjects in map to given connection.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public static void SendAllObjectsToConnection(QNetConnection connection)
        {
            if (!QNetManager.Instance.IsServerActive)
                throw new InvalidOperationException("This methods can only be used by server.");

            for (var index = 0; index < QNetObject.Objects.Count; index++)
            {
                var obj = QNetObject.Objects[index];
                SendObjectToConnection(connection, obj);
            }
        }

        /// <summary>
        ///     Resolves object message to send to connection.
        /// </summary>
        private static QNetMessageWriter ResolveObjectCreateMessage(QNetObject qNetObject)
        {
            // Create message.
            var writer = QNetManager.GenerateServerMessage(QNetUnityHeader.OBJECT_CREATE);

            // Write base message.
            writer.WriteMessage(new QNetObjectSerialized
            {
                ObjectIdentity = qNetObject.Identity,
                OwnerIdentity = qNetObject.Identity.Owner,
                PrefabIdentity = qNetObject.PrefabIdentity,

                Position = qNetObject.transform.position,
                Rotation = qNetObject.transform.rotation,
                Scale = qNetObject.transform.localScale,

                CustomComponents = qNetObject.Identity.GetArrayOfCustomComponentsTypes()
            });

            // And write object's custom data.
            qNetObject.Identity.SerializeAllObjects(writer);

            return writer;
        }
    }
}