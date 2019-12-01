//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Messages;
using JetBrains.Annotations;
using System;

namespace JEM.QNet.UnityEngine.Handlers
{
    /// <summary>
    ///     Class with two functions dedicated only to register all handlers of given peer.
    /// </summary>
    internal static class QNetHandlerStack
    {
        /// <summary>
        ///     Registers all server side handlers.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RegisterServerHandlers([NotNull] QNetServer server)
        {
            if (server == null) throw new ArgumentNullException(nameof(server), "Server is null.");
            server.SetHandler(new QNetMessage(true, QNetUnityHeader.LEVEL_LOADED, QNetHandlerWorld.OnClientLevelLoaded));
            server.SetHandler(new QNetMessage(true, QNetUnityHeader.WORLD_SERIALIZED, QNetHandlerWorld.OnClientWorldSerialized));
            server.SetHandler(new QNetMessage(true, QNetUnityHeader.ENTITY_QUERY, QNetHandlerEntity.OnClientEntityQuery));
        }

        /// <summary>
        ///     Registers all client side handlers.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RegisterClientHandlers([NotNull] QNetClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client), "Client is null.");
            client.SetHandler(new QNetMessage(false, QNetUnityHeader.LEVEL_LOADING, QNetHandlerWorld.OnServerLevelLoading));
            client.SetHandler(new QNetMessage(false, QNetUnityHeader.WORLD_SERIALIZATION, QNetHandlerWorld.OnServerWorldSerialization));
            client.SetHandler(new QNetMessage(false, QNetUnityHeader.OBJECT_CREATE, QNetHandlerObject.OnServerObjectCreate));
            client.SetHandler(new QNetMessage(false, QNetUnityHeader.OBJECT_DELETE, QNetHandlerObject.OnServerObjectDelete));
            client.SetHandler(new QNetMessage(false, QNetUnityHeader.OBJECT_STATE, QNetHandlerObject.OnServerObjectState));
            client.SetHandler(new QNetMessage(false, QNetUnityHeader.OBJECT_ACTIVATION, QNetHandlerObject.OnServerObjectActivation));
            client.SetHandler(new QNetMessage(false, QNetUnityHeader.OBJECT_QUERY, QNetHandlerObject.OnClientObjectQuery));

            // For object query we can pass the same method as for the server.
            client.SetHandler(new QNetMessage(false, QNetUnityHeader.ENTITY_QUERY, QNetHandlerEntity.OnClientEntityQuery));
        }
    }
}