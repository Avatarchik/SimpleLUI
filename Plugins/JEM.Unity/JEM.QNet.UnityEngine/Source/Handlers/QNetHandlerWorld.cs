//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Messages;
using JEM.QNet.UnityEngine.Objects;

namespace JEM.QNet.UnityEngine.Handlers
{
    // Scene/level/world QNet related network header
    // Contains core header handlers of client level initialization.
    internal static class QNetHandlerWorld
    {
        /// <summary>
        ///     Header handle of QNetUnityHeader.LEVEL_LOADING.
        ///     Called by the server at the end of connection initialization (onConnectionReady event).
        ///         Triggers the client's level loading process.
        /// </summary>
        internal static void OnServerLevelLoading(QNetMessage message, QNetMessageReader reader,
            ref bool disallowRecycle)
        {
            if (QNetManager.Instance.IsServerActive)
            {
#if DEBUG
                QNetManager.PrintLogMsc("We are on server. " +
                                       $"Client initialization from {nameof(OnServerLevelLoading)} will be ignored.");
#endif

                // If this message was received on the client when the server is active,
                //  this means that we are on host.
                // As host, we don't need to load or initialize anything.
                // Just send to the server that this client is ready to go with network level.
                QNetManager.Send(QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered,
                    QNetUnityHeader.LEVEL_LOADED);
            }
            else
            {
                // Run the first phase of client initialization.
                // At the end of initialization we will receive a onDone event that will allow us to
                //  call the server that this client is ready with network level.
                // After the network level initialization the server will send us all QNetObject he has on the scene
                //  this will trigger second and final phase of initialization.
                var levelName = reader.ReadString();
                QNetNetworkScene.RunInitializationProcess(QNetNetworkScene.RunClientInitializationFirstPhase(levelName));
            }
        }

        /// <summary>
        ///     Header handle of QNetUnityHeader.LEVEL_LOADED.
        ///     Called by the client at the end of first phase initialization.
        ///         From here, the server need to send all the object he created.
        /// </summary>
        internal static void OnClientLevelLoaded(QNetMessage message, QNetMessageReader reader,
            ref bool disallowRecycle)
        {
            if (QNetManager.Instance.IsHostActive)
            {
                var isHost = QNetManager.Instance.HostClientConnection.ConnectionIdentity ==
                             reader.Connection.ConnectionIdentity;
                if (isHost)
                {
                    // loaded level message has been send from host client, send only WORLD_SERIALIZATION message
                    // As a host, we don't need to send any object serialization data.
                    // We can skip all the initialization same as in first phase.
                    QNetManager.Send(reader.Connection, QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered,
                        QNetUnityHeader.WORLD_SERIALIZATION);
                    return;
                }
            }

#if DEBUG
            QNetManager.PrintLogMsc($"Connection {reader.Connection} loaded network level. " +
                                 $"Will send all QNetObject based objects ({QNetObject.Objects.Count}).");
#endif

            // Send all objects to target client.
            QNetNetworkScene.SendAllObjectsToConnection(reader.Connection);

            // Notify client that all the object has  been send and he can start second phase initialization.
            QNetManager.Send(reader.Connection, QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered,
                QNetUnityHeader.WORLD_SERIALIZATION);
        }

        /// <summary>
        ///     Header handle for QNetUnityHeader.WORLD_SERIALIZATION.
        ///     Called by the server as a notify that all QNetObject based object has been send and we
        ///         can start the second phase initialization.
        /// </summary>
        internal static void OnServerWorldSerialization(QNetMessage message, QNetMessageReader reader,
            ref bool disallowRecycle)
        {
            if (QNetManager.Instance.IsServerActive)
            {
                // Method received on client when the server is also active, that means a host connection.
                // Ignore second phase and call the server that everything is done.
#if DEBUG
                QNetManager.PrintLogMsc("WORLD_SERIALIZATION received but we will " +
                                        "ignore that because of active host connection.");
#endif

                // When the host is active, the onNetworkWorldInitialized should be called from here.
                if (QNetManager.Instance.IsHostActive)
                {
                    // Call onNetworkWorldInitialized.
                    QNetManagerBehaviour.ForEach(b => b.CallOnNetworkWorldInitialized());
                }

                QNetManager.Send(QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered,
                    QNetUnityHeader.WORLD_SERIALIZED);
                return;
            }

            // Run second phase initialization to de-serialize all serialized objects.
#if DEBUG
            QNetManager.PrintLogMsc("Locale client received all the QNetObject of the server to de-serialize. " +
                                    "Running the second phase initialization.");
#endif
            QNetNetworkScene.RunInitializationProcess(QNetNetworkScene.RunClientInitializationSecondPhase());
        }

        /// <summary>
        ///     Header handle for QNetUnityHeader.WORLD_SERIALIZED.
        ///     Called by the server at the end of second phase initialization.
        ///         Now that this client connection is ready we can tag him as ready and spawn it's controller in scene.
        /// </summary>
        internal static void OnClientWorldSerialized(QNetMessage message, QNetMessageReader reader,
            ref bool disallowRecycle)
        {
            // Get player.
            var player = QNetPlayer.GetQNetPlayer(reader.Connection);
            if (player == null)
            {
                QNetManager.PrintLogError("System encounter an unexcepted error. " +
                                          "Connection that send us WORLD_SERIALIZED message does not have it's QNetPlayer reference.");
                QNetManager.CloseConnection(reader.Connection, "QNetUnexceptedError");
                return;
            }

            // Tag as ready.
            player.TagAsReady();
        }
    }
}