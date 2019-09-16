//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Handlers;
using JEM.QNet.UnityEngine.Messages;
using JEM.QNet.UnityEngine.Objects;
using JEM.QNet.UnityEngine.Simulation;
using JEM.UnityEngine.Components;
using JEM.UnityEngine.Extension;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace JEM.QNet.UnityEngine
{
    /// <summary>
    ///     Defines a level od QNet debugging.
    /// </summary>
    public enum QNetDebuggingLevel
    {
        /// <summary>
        ///     Never print a log by QNet.
        /// </summary>
        None,

        /// <summary>
        ///     Print only a important information like client connections etc.
        /// </summary>
        Info,

        /// <summary>
        ///     Print everything!
        /// </summary>
        Full,

        /// <summary>
        ///     Print everything but with developer info like, what has been written to the last message etc.
        /// </summary>
        Developer,

        /// <summary>
        ///     Only errors will be printed.
        /// </summary>
        ErrorOnly
    }

    /// <inheritdoc />
    /// <summary>
    ///     QNet Manager.
    ///     Core QNetworking component.
    /// </summary>
    /// <remarks>
    ///     Once QNetManager is initialized, should never be destroyed!
    /// </remarks>
    public sealed partial class QNetManager : MonoBehaviour
    {
        /// <summary>
        ///     Default IP Address used by manager.
        /// </summary>
        [Header("Network Settings")]
        public string DefaultAddress = "127.0.0.1";

        /// <summary>
        ///     Default Port used by manager.
        /// </summary>
        public ushort DefaultPort = 21115;

        /// <summary>
        ///     Default size of the server used by manager.
        /// </summary>
        [Range(2, 64)]
        public ushort DefaultServerSize = 8;

        /// <summary>
        ///     Default name of level to load by server.
        /// </summary>
        public string DefaultLevel = string.Empty;

        /// <summary>
        ///     A name of level that QNet will load to unload loaded network scene.
        /// </summary>
        public string MenuLevel = "Main";

        /// <summary>
        ///     Reference to the QNetDatabase that holds all prefabs that could be spawned in network.
        /// </summary>
        [Header("References Settings")]
        public QNetDatabase DatabaseReference;

        /// <summary>
        ///     Defines the active debugging level QNetManager has.
        /// </summary>
        [Header("Debugging Settings")]
        public QNetDebuggingLevel DebuggingLevel;

        /// <summary>
        ///     Forces QNet to print logs via <see cref="Debug"/> instead of <see cref="JEMLogger"/>.
        /// </summary>
        public bool ForceUnityDebug;

        /// <summary>
        ///     List of all currently running peers.
        /// </summary>
        /// <remarks>
        ///     In most cases there will be only one running peer (client or server),
        ///      or two (host that creates a server instance with internal client).
        /// </remarks>
        internal List<QNetPeer> RunningPeers { get; } = new List<QNetPeer>();

        /// <summary>
        ///     Active QNetClient instance.
        /// </summary>
        public QNetClient Client { get; private set; }

        /// <summary>
        ///     Active QNetServer instance.
        /// </summary>
        public QNetServer Server { get; private set; }

        /// <summary>
        ///     Reference to the host Client QNetConnection struct.
        /// </summary>
        public QNetConnection HostClientConnection { get; private set; } = default;

        /// <summary>
        ///     True, if the client is currently active.
        /// </summary>
        public bool IsClientActive { get; private set; }

        /// <summary>
        ///     True, if the server is currently active.
        /// </summary>
        public bool IsServerActive { get; private set; }

        /// <summary>
        ///     True, if the host is currently active.
        /// </summary>
        /// <remarks>
        ///     While host is active, both <see cref="IsClientActive"/> and <see cref="IsServerActive"/> are always true.
        /// </remarks>
        public bool IsHostActive { get; private set; }

        /// <summary>
        ///     Defines whether the network is active.
        /// </summary>
        /// <remarks>
        ///     Returns true if <see cref="IsClientActive"/> or <see cref="IsServerActive"/> is equals true.
        /// </remarks>
        public bool IsNetworkActive => IsClientActive || IsServerActive;

        /// <summary>
        ///     True if QNet is currently shutting down by ApplicationQuit.
        /// </summary>
        public bool ShuttingDownByApplicationQuit { get; private set; }

        /// <summary>
        ///     Defines if the manager should simulate the network scene.
        /// </summary>
        public bool ShouldSimulate => IsNetworkActive;

        /// <summary>
        ///     True if QNetManager is currently stopping something.
        /// </summary>
        public bool IsStopping { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                PrintLogWarning("Another QNetManager component detected!. Only one QNetManager is allowed.", this);
                gameObject.SetActive(false);
                return;
            }

            Instance = this;

            // Always keep QNetManager
            gameObject.CollectComponent<JEMObjectKeepOnScene>();

            // Check if database is set
            if (DatabaseReference == null)
            {
                PrintLogError("QNetDatabase reference is not set!", this);
            }

            // Collect the QNetSerializedMessages types.
            QNetBehaviour.CollectAllSerializedMessageTypes();
            // Collect the QNetObject types.
            QNetObject.CollectAllQNetObjectTypes();
        }

        private void Update()
        {
            if (!ShouldSimulate)
            {
                return;
            }

            // Unsafe Simulate.
            QNetSimulator.UnsafeSimulate();
        }

        private void LateUpdate()
        {
            if (!ShouldSimulate)
            {
                return;
            }

            // Unsafe Late Simulate.
            QNetSimulator.UnsafeLateSimulate();
        }

        private void FixedUpdate()
        {
            if (!ShouldSimulate)
            {
                return;
            }

            // Receive messages.
            PollMessages();

            // Try to sync server frames.
            QNetSimulator.AdjustFrames();

            // Dispatch messages.
            DispatchMessages();

            // Simulate.
            var shouldSimulate = QNetNetworkScene.SceneState == QNetSceneState.Loaded;
            if (shouldSimulate)
            {
                QNetSimulator.Simulate();
            }
        }

        private void OnApplicationQuit()
        {
            // Make sure that all peer will be closed on application quit
            ShuttingDownByApplicationQuit = true;
            if (RunningPeers.Count != 0)
                StopCurrentConnection(IsServerActive ? "Server shutdown by ApplicationQuit" : "Quit");
        }

        private void PollMessages()
        {
            Profiler.BeginSample("QNetManager.PollMessages");
            for (var index = 0; index < RunningPeers.Count; index++)
            {
                var peer = RunningPeers[index];
                peer.PollMessages();
            }

            Profiler.EndSample();
        }

        private void DispatchMessages()
        {
            Profiler.BeginSample("QNetManager.DispatchMessages");
            for (var index = 0; index < RunningPeers.Count; index++)
            {
                var peer = RunningPeers[index];
                peer.DispatchMessages();
            }

            Profiler.EndSample();
        }

        /// <summary>
        ///     Reset the QNetManager.
        /// </summary>
        /// <remarks>
        ///     Just restarts the frames. 
        /// </remarks>
        private void ResetManager()
        {
            QNetTime.Frame = 0;
            QNetSimulator.EstimatedServerFrame = 0;
        }

        /// <summary>
        ///     Setups and starts given QNetPeer instance.
        /// </summary>
        /// <param name="peer">Peer to start.</param>
        /// <param name="configuration">Configuration of peer.</param>
        /// <param name="onRegisterHandlers"/>
        /// <exception cref="ArgumentNullException"/>
        private T InternalStartPeer<T>([NotNull] T peer, [NotNull] QNetConfiguration configuration, Action onRegisterHandlers) where T : QNetPeer
        {
            if (peer == null) throw new ArgumentNullException(nameof(peer));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            // First register peer's local handlers.
            peer.RegisterLocalPeerHandlers();
            // Then register our local handlers.
            onRegisterHandlers.Invoke();
            // Start this peer.
            peer.Start(configuration);
            // And update running peers.
            RunningPeers.Add(peer);

            return peer;
        }

        /// <summary>
        ///     Stops given peer.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        private void InternalStopPeer([NotNull] QNetPeer peer, string stopReason)
        {
            if (peer == null) throw new ArgumentNullException(nameof(peer));
            if (!RunningPeers.Contains(peer))
                throw new InvalidOperationException("Unable to stop given peer. " +
                                                    "Given peer can't be stopped while not started.");
            // Remove this peer.
            RunningPeers.Remove(peer);
            // And then stop!
            peer.Stop(stopReason);
        }

        /// <summary>
        ///     Stops all the current connections.
        /// </summary>
        /// <remarks>
        ///     This method will stop all current connection, so if we are host (with client and server) this will stop it all.
        /// </remarks>
        public void StopCurrentConnection() => StopCurrentConnection(null);

        /// <summary>
        ///     Starts local server based on given or local configuration.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public void StartServer()
        {
            StartServer(new QNetConfiguration
            {
                IpAddress = DefaultAddress,
                Port = DefaultPort,
                MaxConnections = (short)DefaultServerSize
            });
        }

        /// <summary>
        ///     Cashed server configuration to use at the end of <see cref="QNetNetworkScene.RunServerInitialization"/>.
        /// </summary>
        internal QNetConfiguration CashedServerConfiguration;

        /// <summary>
        ///     Starts local server based on given or local configuration.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public void StartServer([NotNull] QNetConfiguration configuration, Action onInitializationDone = null)
        {
            // Load the game level first.
            CashedServerConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            // Run the server initialization process
            // NOTE: InternalStartServer method is called at the end of RunServerInitialization routine.
            QNetNetworkScene.RunInitializationProcess(QNetNetworkScene.RunServerInitialization(onInitializationDone));
        }

        /// <summary>
        ///     Starts local server based on given or local configuration.
        /// </summary>
        /// <remarks>
        ///     This method starts just the server, so to load the game level we need to call <see cref="StartServer()"/> method instead.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        internal void InternalStartServer(QNetConfiguration configuration)
        {
            if (IsServerActive)
                throw new InvalidOperationException("QNet is unable to start server while " +
                                                    "there is already active instance of server.");

            // Check if configuration is set.
            if (configuration == null)
            {
                configuration = new QNetConfiguration
                {
                    IpAddress = DefaultAddress,
                    Port = DefaultPort,
                    MaxConnections = (short) DefaultServerSize
                };
            }

            // Update server active state
            IsServerActive = true;

            // Restart the manager.
            ResetManager();

            // Start the server.
            Server = new QNetServer();
            Server = InternalStartPeer(Server, configuration, () =>
            {
                // Register internal headers.
                QNetHandlerStack.RegisterServerHandlers(Server);

                // call registerCustomHandlers
                QNetManagerBehaviour.ForEach(b => b.CallRegisterCustomHandlers(Server, true));
            });

            // Write ServerFrame before every message QNet sends
            Server.OnBeforeMessage += message => message.Write(QNetTime.ServerFrame);

            // Send the current tickRate and serverFrame on connection authorizing
            // NOTE: OnConnectionAuthorizing is called when new client has been
            //       received and server should authorize/check if this client may join the server
            Server.OnConnectionAuthorizing += (QNetConnection connection, QNetMessageWriter writer, ref bool refuse) =>
            {
                writer.WriteInt32(QNetTime.TickRate);
                writer.WriteUInt32(QNetTime.ServerFrame);

                // call onServerAuthorize
                var r = refuse;
                QNetManagerBehaviour.ForEach(b => b.CallOnServerAuthorize(connection, writer, ref r));
                refuse = r;
            };

            // Read the connection init data (as a respond for our onConnectionAuthorizing)
            Server.OnConnectionReady += reader =>
            {     
                var clientToken = reader.ReadUInt32();
                var clientNickname = reader.ReadString();

                // PrintLog($"{reader.Connection.ConnectionIdentity} {Client.ConnectionIdentity}");
                // Check if the received connection is a internal host connection.
                if (IsHostActive && (reader.Connection.ConnectionIdentity == 0 ||
                                     reader.Connection.ConnectionIdentity == Client.ConnectionIdentity))
                {
                    HostClientConnection = reader.Connection;
#if DEBUG
                    PrintLogMsc("Received the host client connection.");
#endif

                }

                if (IsHostActive && HostClientConnection.Equals(default(QNetConnection)))
                {
                    PrintLogWarning("New connection received when we still don't have reference for the HostClientConnection!");
                }

                // Check if received client token is not in use by other client.
                if (QNetPlayer.GetQNetPlayerByToken(clientToken) != null)
                {
                    PrintLogWarning("Newly received connection " +
                                    "is using already used token. Disconnecting!");
                    Server.CloseConnection(reader.Connection, "TokenAlreadyInUse");
                    return;
                }

                // Create new QNetPlayer for received connection.
                var qNetPlayer = QNetPlayer.CreateQNetPlayer(reader.Connection.ConnectionIdentity, clientToken, clientNickname);
                if (qNetPlayer == null)
                {
                    PrintLogWarning("Newly received connection " +
                                    "don't have his QNetPlayer instance. Disconnecting!");
                    Server.CloseConnection(reader.Connection, "InternalQNetPlayerError");
                    return;
                }

                // call onServerNewPlayer
                QNetManagerBehaviour.ForEach(b => b.CallOnServerNewPlayer(qNetPlayer, reader));
                // Send the levelLoading message.
                Send(reader.Connection, QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered,
                    QNetUnityHeader.LEVEL_LOADING, QNetNetworkScene.SceneName);                
            };

            // We lost the connection! Destroy all objects related with it.
            Server.OnConnectionLost += (connection, reason) =>
            {
                var qNetPlayer = QNetPlayer.GetQNetPlayer(connection);
                if (qNetPlayer == null) return; // Well.. we failed to find QNetPlayer of this connection so...

                // call onServerLostPlayer
                QNetManagerBehaviour.ForEach(b => b.CallOnServerLostPlayer(qNetPlayer, reason));

                // The only thing to do here is to tag player as not ready (if ready).
                if (qNetPlayer.Ready)
                    qNetPlayer.TagAsNotReady();

                // And remove QNetPlayer from local machine.
                QNetPlayer.DestroyQNetPlayer(qNetPlayer);
            };

            // Server stop! de-initialize game.
            Server.OnServerStop += reason =>
            {
                // Server has been stopped, try to de-initialize game
                if (!ShuttingDownByApplicationQuit)
                {
                    QNetNetworkScene.RunInitializationProcess(QNetNetworkScene.UnloadNetworkScene(() =>
                    {
                        // call onServerStop
                        QNetManagerBehaviour.ForEach(b => b.CallOnServerStop(reason));
                    }));
                }
                else
                {
                    // Game is shutting down by applicationQuit.
                    // Do not deInitialize game, unity will do this for us anyway.
                    // call onServerStop
                    QNetManagerBehaviour.ForEach(b => b.CallOnServerStop(reason));
                }

                IsServerActive = false;
                IsHostActive = false;
            };

            // call onServerStarted
            QNetManagerBehaviour.ForEach(b => b.CallOnServerStarted());
        }

        /// <summary>
        ///     Starts client connection.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentNullException"/>
        public void StartClient([NotNull] string ipAddress, ushort port)
        {
            if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

            // Setup configuration
            var configuration = new QNetConfiguration
            {
                IpAddress = ipAddress,
                Port = port,
                MaxConnections = 2
            };

            StartClient(configuration);
        }

        /// <summary>
        ///     Starts client connection.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentNullException"/>
        public void StartClient()
        {
            StartClient(new QNetConfiguration
            {
                IpAddress = DefaultAddress,
                Port = DefaultPort
            });
        }

        /// <summary>
        ///     Starts client connection.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentNullException"/>
        public void StartClient([NotNull] QNetConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (IsClientActive)
                throw new InvalidOperationException("QNet is unable to start client while " +
                                                    "there is already active instance of client.");

            // Set the client active state.
            IsClientActive = true;

            // Restart the manager.
            ResetManager();

            // Start the client.
            Client = new QNetClient();
            Client = InternalStartPeer(Client, configuration, () =>
            {
                // Register internal headers.
                QNetHandlerStack.RegisterClientHandlers(Client);

                // call registerCustomHandlers
                QNetManagerBehaviour.ForEach(b => b.CallRegisterCustomHandlers(Client, false));
            });

            // Read the Server frame that is bundled by server to every message QNet sends.
            Client.OnMessagePoll += reader =>
            {
                // as the server always send the server frame
                // we need to read that right here
                QNetSimulator.ReceivedServerFrame = reader.ReadUInt32();
                QNetSimulator.AdjustServerFrames = QNetSimulator.ReceivedServerFrame > QNetTime.ServerFrame;
            };

            // Handle the OnClientConnected just for the manager behaviour.
            Client.OnConnected += () => QNetManagerBehaviour.ForEach(b => b.CallOnClientConnected());

            // OnConnectionReady is a result of server's OnConnectionAuthorizing
            //   we need to read tickRate and serverFrame here.
            // In on connection ready we also need to get and write the local client token.
            Client.OnConnectionReady += (reader, writer) =>
            {
                var tickRate = reader.ReadInt32();
                var serverFrame = reader.ReadUInt32();

                // Set TickRate
                QNetTime.TickRate = tickRate;

                // Set the server frame.
                QNetSimulator.ReceivedServerFrame = serverFrame;
                QNetSimulator.EstimatedServerFrame = serverFrame;

                uint token = 0; // TODO: Randomize token so we may allow to not implement onClientPrepare method
                string nickname = "Ramaja :)";
                QNetManagerBehaviour.ForEach(b => b.CallOnClientPrepare(ref token, ref nickname));
                writer.WriteUInt32(token);
                writer.WriteString(nickname);
            };

            // Handle client disconnection event
            Client.OnDisconnection += (lostConnection, reason) =>
            {
                // Ignore next events if client is not active. (BUG: QNet is sending a OnDisconnection duplicates)
                if (!IsClientActive)
                {
                    return;
                }

                // Stop peer if not already stopped.
                if (RunningPeers.Contains(Client))
                    InternalStopPeer(Client, reason);

                // Update state
                IsClientActive = false;

                // Try to unload the scene only if the QNetStateState is not equals NotLoaded
                //   This is because OnDisconnection is called also as connection failed result.
                if (QNetNetworkScene.SceneState != QNetSceneState.NotLoaded)
                {
                    if (!IsHostActive)
                    {
                        // De-initialize only if not a host.
                        // NOTE: While host is active, the server will unload the level anyway.
                        QNetNetworkScene.RunInitializationProcess(QNetNetworkScene.UnloadNetworkScene(() =>
                        {
                            // call onClientStop.
                            QNetManagerBehaviour.ForEach(b => b.CallOnClientStop(lostConnection, reason));
                        }));
                        return;
                    }
                }

                // call onClientStop.
                QNetManagerBehaviour.ForEach(b => b.CallOnClientStop(lostConnection, reason));
            };

            // call onClientStarted
            QNetManagerBehaviour.ForEach(b => b.CallOnClientStarted(configuration));
        }
 
        /// <summary>
        ///     Starts a new host on given configuration.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentNullException"/>
        public void StartHost()
        {
            StartHost(new QNetConfiguration
            {
                IpAddress = DefaultAddress,
                Port = DefaultPort,
                MaxConnections = (short) DefaultServerSize
            });
        }

        /// <summary>
        ///     Starts a new host on given configuration.
        /// </summary>
        /// <remarks>
        ///     This method starts just the host, so to load the game level we need to call <see cref="StartHost()"/> method instead.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentNullException"/>
        public void StartHost([NotNull] QNetConfiguration configuration)
        {
            if (IsHostActive)
                throw new InvalidOperationException("QNet is unable to start host while " +
                                                    "there is already active instance of host.");
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            PrintLogInfo("QNet is starting local host.");

            // Set the host active state.
            IsHostActive = true;

            // Start server connection.
            StartServer(configuration, () =>
            {
                // Start the internal client when server is ready.
                StartClient(configuration.IpAddress, configuration.Port);
                Server.TagAsHostServer(Client.OriginalConnection);

                // Handle internal client disconnection event
                Client.OnDisconnection += (lostConnection, reason) =>
                {
                    reason = $"(Client) {reason}";
                    PrintLogWarning("QNet is stopping host because of client disconnection. " + reason);
                    // stop all peers
                    StopCurrentConnection(reason);
                };
            });
        }

        /// <summary>
        ///     Stops all the current connections.
        /// </summary>
        /// <remarks>
        ///     This method will stop all current connection, so if we are host (with client and server) this will stop it all.
        /// </remarks>
        public void StopCurrentConnection(string stopReason)
        {
            if (IsStopping)
            {
                return;
            }

            if (!IsNetworkActive)
            {
                // Network is not active, nothing to stop.
                return;
            }

            IsStopping = true;
            PrintLogInfo($"QNet is Stopping all current connections ({RunningPeers.Count}).");

            // check if reason message is empty or null
            if (string.IsNullOrEmpty(stopReason))
                stopReason = "Stopping*";

            // and just stop all peers
            while (RunningPeers.Count > 0)
                InternalStopPeer(RunningPeers[0], stopReason);

            Client = null;
            Server = null;

            IsClientActive = false;
            IsServerActive = false;
            IsHostActive = false;
            IsStopping = false;

            HostClientConnection = default;
        }

        #region PRINT_LOG

        /// <summary>
        ///     Print a QNet log.
        /// </summary>
        /// <remarks>
        ///     Will only be printed if debug level is at least: Info
        /// </remarks>
        /// <param name="log"/>
        /// <param name="context">Object context is utilized only by <see cref="Debug"/> that can be used if <see cref="ForceUnityDebug"/> is set to true.</param>
        internal static void PrintLogInfo(string log, Object context = null)
        {
            if (Instance == null)
                return; // Ignore logs if manager is not active.
            if (Instance.DebuggingLevel == QNetDebuggingLevel.None)
                return;

            if (Instance.ForceUnityDebug)
                Debug.Log($"QNet :: {log}", context);
            else
            {
                JEMLogger.Log(log, "QNet");
            }
        }

        /// <summary>
        ///     Print a QNet log.
        /// </summary>
        /// <remarks>
        ///     Will only be printed if debug level is at least: Developer
        ///     IMPORTANT: Developer logs are only included in DEBUG builds.
        /// </remarks>
        /// <param name="log"/>
        /// <param name="context">Object context is utilized only by <see cref="Debug"/> that can be used if <see cref="ForceUnityDebug"/> is set to true.</param>
        internal static void PrintLogMsc(string log, Object context = null)
        {
            if (Instance == null)
                return; // Ignore logs if manager is not active.
            if (Instance.DebuggingLevel != QNetDebuggingLevel.Developer)
                return;

            if (Instance.ForceUnityDebug)
                Debug.Log($"QNet :: {log}", context);
            else
            {
                JEMLogger.Log(log, "QNet");
            }
        }

        /// <summary>
        ///     Print a QNet warning log.
        /// </summary>
        /// <remarks>
        ///     Will only be printed if debug level is at least: Full
        /// </remarks>
        /// <param name="log"/>
        /// <param name="context">Object context is utilized only by <see cref="Debug"/> that can be used if <see cref="ForceUnityDebug"/> is set to true.</param>
        internal static void PrintLogWarning(string log, Object context = null)
        {
            if (Instance == null)
                return; // Ignore logs if manager is not active.
            if (Instance.DebuggingLevel != QNetDebuggingLevel.Full || 
                Instance.DebuggingLevel != QNetDebuggingLevel.Developer)
                return;

            if (Instance.ForceUnityDebug)
                Debug.LogWarning($"QNet :: {log}", context);
            else
            {
                JEMLogger.LogWarning(log, "QNet");
            }
        }

        /// <summary>
        ///     Print a QNet error log.
        /// </summary>
        /// <remarks>
        ///     Will only be printed if debug level is at least: Info or ErrorOnly
        /// </remarks>
        /// <param name="log"/>
        /// <param name="context">Object context is utilized only by <see cref="Debug"/> that can be used if <see cref="ForceUnityDebug"/> is set to true.</param>
        internal static void PrintLogError(string log, Object context = null)
        {
            if (Instance == null)
                return; // Ignore logs if manager is not active.
            if (Instance.DebuggingLevel == QNetDebuggingLevel.None)
                return;

            if (Instance != null && Instance.ForceUnityDebug)
                Debug.LogError($"QNet :: {log}", context);
            else
            {
                JEMLogger.LogError(log, "QNet");
            }
        }

        /// <summary>
        ///     Print a QNet exception log.
        /// </summary>
        /// <remarks>
        ///     Exception are always thrown.
        /// </remarks>
        /// <param name="ex"/>
        /// <param name="context">Object context is utilized only by <see cref="Debug"/> that can be used if <see cref="ForceUnityDebug"/> is set to true.</param>
        internal static void PrintLogException(Exception ex, Object context = null)
        {
            if (Instance != null && Instance.ForceUnityDebug)
                Debug.LogException(ex, context);
            else
            {
                JEMLogger.LogException(ex, "QNet");
            }
        }

        /// <summary>
        ///     Print a QNet fixed exception log.
        /// </summary>
        /// <remarks>
        ///     Exception are always thrown.
        /// </remarks>
        /// <param name="message"/>
        /// <param name="stackTrace"></param>
        /// <param name="context">Object context is utilized only by <see cref="Debug"/> that can be used if <see cref="ForceUnityDebug"/> is set to true.</param>
        internal static void PrintLogException(string message, string stackTrace, Object context = null)
        {
            if (Instance != null && Instance.ForceUnityDebug)
                Debug.LogError($"{message}\n{stackTrace}", context);
            else
            {
                JEMLogger.LogException(message, stackTrace, "QNet");
            }
        }

        /// <summary>
        ///     Print a QNet assert log.
        /// </summary>
        /// <remarks>
        ///     Will only be printed if debug level is at least: Info or ErrorOnly
        /// </remarks>
        /// <param name="condition"/>
        /// <param name="message"/>
        /// <param name="context">Object context is utilized only by <see cref="Debug"/> that can be used if <see cref="ForceUnityDebug"/> is set to true.</param>
        internal static void PrintLogAssert(bool condition, string message, Object context = null)
        {
            if (Instance == null)
                return; // Ignore logs if manager is not active.
            if (Instance.DebuggingLevel == QNetDebuggingLevel.None)
                return;

            if (Instance != null && Instance.ForceUnityDebug)
                Debug.Assert(condition, $"QNet :: {message}", context);
            else
            {
                if (!condition) JEMLogger.LogAssert(message, "QNet");
            }
        }

        #endregion

        /// <summary>
        ///     Reference to the currently active <see cref="QNetManager"/> component.
        /// </summary>
        public static QNetManager Instance { get; private set; }
    }
}
