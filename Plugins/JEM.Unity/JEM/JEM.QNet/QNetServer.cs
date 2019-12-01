//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.QNet.Extras;
using JEM.QNet.Messages;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace JEM.QNet
{
    /// <inheritdoc />
    /// <summary>
    ///     QNet server base class.
    /// </summary>
    public partial class QNetServer : QNetPeer
    {
        /// <summary>
        ///     Defines whether the new connections should be accepted or not.
        /// </summary>
        public bool AcceptNewConnections = true;

        /// <summary>
        ///     Message that will be send to client while new connections are not accepted.
        /// </summary>
        public string NewConnectionDeniedMessage = "Connection denied.";

        /// <summary>
        ///     Original lidgren server instance.
        /// </summary>
        public NetServer OriginalServer { get; private set; }

        /// <summary>
        ///     Defines whether this server is at the same time client.
        /// </summary>
        public bool IsHostServer { get; private set; }

        ///// <summary>
        /////     Original connection of host client.
        ///// </summary>
        //public NetConnection OriginalHostConnection { get; private set; }

        /// <summary>
        ///     List of all connections.
        /// </summary>
        public IReadOnlyList<QNetConnection> Connections => InternalConnections;
        private List<QNetConnection> InternalConnections { get; } = new List<QNetConnection>();

        private bool _shutdownByException;
        private List<QNetMessageReader> IncomingMessages { get; } = new List<QNetMessageReader>();

        /// <inheritdoc />
        public QNetServer() : base("QNetServer")
        {
            // ignore
        }

        /// <inheritdoc />
        public override short GetSignatureSize()
        {
            return QNetMessage.ResolveLocalHeaderSize();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public override bool Start(QNetPeerConfiguration configuration)
        {
            if (IsStarted) throw new InvalidOperationException();
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (!IPAddress.TryParse(configuration.IpAddress, out var address))
                throw new ArgumentException($"'{configuration.IpAddress}' is not a valid address");

            // Setup Lidgren.Network configuration.
            var cfg = PrepareDefaultPeerConfig();
            cfg.Port = configuration.Port;
            cfg.BroadcastAddress = address;

            cfg.MaximumConnections = configuration.MaxConnections;
            cfg.AcceptIncomingConnections = true;

            cfg.ConnectionTimeout = configuration.ConnectionTimeout;
            cfg.UseMessageRecycling = configuration.UseMessageRecycling;

            cfg.NetworkThreadName = "qnet_server";
            
            // Enable utilized messages.
            cfg.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            if (configuration.AllowUnconnectedData)
            {
                JEMLogger.Log("AllowUnconnected data enabled.", "QNet");
                cfg.SetMessageTypeEnabled(NetIncomingMessageType.UnconnectedData, true);
            }

            // Create server instance.
            OriginalServer = new NetServer(cfg);

            // Run.
            IsStarted = true;
            Configuration = configuration;
            OriginalPeer = OriginalServer;
            OriginalServer.Start();

            JEMLogger.Log($"QNetServer started at {configuration.IpAddress}:{configuration.Port} " +
                          $"with maximum players set to {configuration.MaxConnections}.", "QNet");

            QNetGlobal.ServerReference = this;
            QNetCVarManager.HookNetworkCVar();
            return true;
        }

        /// <inheritdoc />
        public override void Stop(string stopReason)
        {
            if (!IsStarted)
            {
                return;
            }

            if (stopReason == null)
                stopReason = "null";

            JEMLogger.Log($"Stopping QNetServer. Reason: {stopReason}", "QNet");

            OnServerStop?.Invoke(stopReason);

            IsStarted = false;
            IsHostServer = false;

            OriginalServer.Shutdown(stopReason);
            OriginalServer = null;
            OriginalPeer = null;
            Configuration = null;
            //OriginalHostConnection = null;
            if (QNetGlobal.ServerReference == this) QNetGlobal.ServerReference = null;
        }

        /// <inheritdoc />
        public override void RegisterLocalPeerHandlers()
        {
            // Register local handler.
            SetHandler(new QNetMessage(true, QNetBaseHeader.CONNECTION_MESSAGE, OnConnectionMessage));
            SetHandler(new QNetMessage(true, QNetBaseHeader.CONNECTION_CLIENT_MESSAGE, OnConnectionClientMessage));
            SetHandler(new QNetMessage(true, QNetBaseHeader.COMMAND_DATA, QNetCommandManager.OnCommandData));
        }

        private void OnConnectionMessage(QNetMessage message, QNetMessageReader reader, ref bool disallowRecycle)
        {
            // Client is ready and is waiting for initial message with it's 
            // Send it's identity and some data from authorizing event
            var outgoingMessage = GenerateOutgoingMessage(QNetBaseHeader.CONNECTION_SERVER_MESSAGE);
            outgoingMessage.WriteUInt16(reader.Connection.ConnectionIdentity);
            var refuse = false;
            OnConnectionAuthorizing?.Invoke(reader.Connection, outgoingMessage, ref refuse);
            if (refuse)
                reader.Connection.CloseConnection("Connection refused by server");
            else
            {
                // Connection not refused.
                // Send network cVars.
                QNetCVarManager.SendAllNetworkedCVars(reader.Connection);

                // And authorizing message.
                Send(reader.Connection, (byte) QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered,
                    outgoingMessage);
            }
        }

        private void OnConnectionClientMessage(QNetMessage message, QNetMessageReader reader, ref bool disallowRecycle)
        {
            // Connection received it's identity and sent it's ready message
            // Invoke ready event with reader
            OnConnectionReady?.Invoke(reader);
        }

        /// <inheritdoc />
        public override void PollMessages()
        {
            if (!IsStarted)
                return;

            NetIncomingMessage msg;
            while (OriginalServer != null && (msg = OriginalServer.ReadMessage()) != null)
            {
                var connectionIpAddress =
                    msg.SenderConnection?.RemoteEndPoint?.Address.ToString() ?? "CANT_GET_ADDRESS";
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.UnconnectedData:
                        if (Configuration == null || !Configuration.AllowUnconnectedData)
                        {
                            break;
                        }

                        try
                        {
                            // Try to read incoming data header.
                            var header = msg.ReadByte();
                            if (header != 101)
                            {
                                // The message is not a client ping.
                                // Call the onUnconnectedDataReceived event.
                                OnUnconnectedDataReceived?.Invoke(msg);
                            }
                            else
                            {
                                // Server is not allowing to ping via unconnected data.
                                if (!Configuration.AllowToPing)
                                    break;

                                // Respond to unconnected data
                                var respondMessage = OriginalServer.CreateMessage();
                                OnPingRespond?.Invoke(respondMessage);

                                // Send the unconnected message back.
                                OriginalServer.SendUnconnectedMessage(respondMessage, msg.SenderEndPoint);
                            }
                        }
                        catch (Exception)
                        {
                            // Ignore any exception received from unconnected data.
                        }

                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        if (msg.SenderConnection == null)
                            break;

                        if (OnApproveConnection == null)
                            msg.SenderConnection.Approve();
                        else
                            OnApproveConnection?.Invoke(msg);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        if (msg.SenderConnection == null)
                            break;

                        var connectionStatus = (NetConnectionStatus)msg.ReadByte();
                        var connectionMessage = msg.ReadString();

                        if (connectionStatus == NetConnectionStatus.Connected)
                        {
                            try
                            {
                                var connectionHeaderSize = msg.SenderConnection.RemoteHailMessage.ReadInt16();
                                if (connectionHeaderSize == GetSignatureSize())
                                {
                                    if (!AcceptNewConnections)
                                    {
                                        msg.SenderConnection.Disconnect(NewConnectionDeniedMessage);
                                    }
                                    else
                                    {
                                        if (GetConnection(msg.SenderConnection).Equals(default(QNetConnection)))
                                        {
                                            JEMLogger.Log($"\'{connectionIpAddress}\' connected to server.", "QNet");

                                            // Generate pseudo random connection identity
                                            var random = new Random();
                                            var connectionIdentity = (ushort) random.Next(ushort.MinValue, ushort.MaxValue);
                                            while (!GetConnection(connectionIdentity).Equals(default(QNetConnection)))
                                                connectionIdentity = (ushort) random.Next(ushort.MinValue, ushort.MaxValue);

                                            // Add new connection
                                            var connection = new QNetConnection(this, msg.SenderConnection, connectionIdentity);
                                            InternalConnections.Add(connection);

                                            // and then do some stuff with it
                                            OnConnectionReceived?.Invoke(connection);
                                        }
                                        else
                                        {
                                            JEMLogger.LogWarning($"Connection \'{connectionIpAddress}\' is " +
                                                                 "already authorized with server..", "QNet");
                                            msg.SenderConnection.Disconnect("ConnectionAlreadyAuthorized");
                                        }
                                    }
                                }
                                else
                                {
                                    JEMLogger.LogWarning("Newly received connection has been refused to connect with server " +
                                                         $"by incorrect signature. Ip Address -> {connectionIpAddress}", "QNet");
                                    msg.SenderConnection.Disconnect("Incompatible network signature.");
                                }
                            }
                            catch (Exception ex)
                            {
                                JEMLogger.LogException(ex);
                                msg.SenderConnection.Disconnect("ConnectionAuthorizingError");
                            }
                        }
                        else if (connectionStatus == NetConnectionStatus.Disconnected)
                        {
                            var connection = GetConnection(msg.SenderConnection);
                            if (!connection.Equals(default(QNetConnection)))
                            {
                                JEMLogger.Log($"\'{connectionIpAddress}\' disconnected " +
                                              $"from server. Reason: {connectionMessage}", "QNet");

                                // Remove this connection.
                                InternalConnections.Remove(connection);

                                // And then unload what this player load's.
                                OnConnectionLost?.Invoke(connection, connectionMessage);
                            }
                            else
                            {
                                JEMLogger.LogWarning($"Unauthorized connection \'{connectionIpAddress}\' " +
                                                     "disconnected from server.", "QNet");
                            }
                        }

                        break;
                    case NetIncomingMessageType.Data:
                        if (msg.SenderConnection == null)
                            break;

                        var senderConnection = GetConnection(msg.SenderConnection);
                        if (senderConnection.Equals(default(QNetConnection)))
                        {
                            JEMLogger.LogWarning("Server received message from unauthorized " +
                                                 $"connection \'{connectionIpAddress}\'.", "QNet");
                            msg.SenderConnection.Disconnect("NotAuthorizedConnection");
                        }
                        else
                        {
                            var message = ReadIncomingMessage(senderConnection, msg);
                            OnMessagePoll?.Invoke(message);
                            IncomingMessages.Add(message);
                        }

                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        if (msg.SenderConnection == null)
                            break;

                        ConnectionLatency = msg.ReadFloat();
                        break;
                    default:
                        if (Configuration == null || !Configuration.DrawNetworkWarningMessages)
                            break;

                        JEMLogger.LogWarning($"UnexceptedConnectionMessageReceived -> {msg.MessageType}", "QNet");
                        try
                        {
                            JEMLogger.LogWarning($"--> (String from unexcepted message: {msg.ReadString()})", "QNet");
                        }
                        catch (Exception)
                        {
                            JEMLogger.LogWarning("Unexcepted message does not have any string type data.", "QNet");
                        }

                        break;
                }
            }
        }

        /// <inheritdoc />
        public override void DispatchMessages()
        {
            for (var index = 0; index < IncomingMessages.Count; index++)
            {
                if (_shutdownByException)
                {
                    continue;
                }

                var message = IncomingMessages[index];
                var canRecycle = true;
                var connectionIpAddress = message.GetMessage().SenderConnection?.RemoteEndPoint?.Address.ToString() ??
                                          "CANT_GET_ADDRESS";

                try
                {
                    var result = InvokeHandler(message);
                    if (!result.WasSuccessful)
                    {
                        message.GetMessage().SenderConnection.Disconnect("ServerNotImplementedHandler");
                    }
                    else
                    {
                        if (result.DisallowToRecycle) canRecycle = false;
                    }
                }
                catch (Exception ex)
                {
                    _shutdownByException = true;
                    JEMLogger.LogException("Server system encounter unhandled exception" +
                                           $" generated by data received from connection {connectionIpAddress}.", ex.ToString(), "QNet");
                    message.GetMessage().SenderConnection.Disconnect("UnhandledServerHeaderError");
                }


                // recycle this message
                if (Configuration == null || Configuration.UseMessageRecycling && canRecycle)
                    OriginalServer?.Recycle(message.GetMessage());
            }

            IncomingMessages.Clear();
        }

        /// <summary>
        ///     Gets QNet connection by connection identity.
        /// </summary>
        public QNetConnection GetConnection(ushort connectionIdentity)
        {
            for (var index = 0; index < InternalConnections.Count; index++)
            {
                var connection = InternalConnections[index];
                if (connection.ConnectionIdentity == connectionIdentity)
                    return connection;
            }

            return default(QNetConnection);
        }

        /// <summary>
        ///     Gets QNet connection by original connection.
        /// </summary>
        public QNetConnection GetConnection(NetConnection originalConnection)
        {
            for (var index = 0; index < InternalConnections.Count; index++)
            {
                var connection = InternalConnections[index];
                if (connection.OriginalConnection == originalConnection)
                    return connection;
            }

            return default(QNetConnection);
        }

        /// <summary>
        ///     Closes given connection with server.
        /// </summary>
        /// <param name="connection">Connection to close.</param>
        /// <param name="reason">Connection closing reason.</param>
        public void CloseConnection(QNetConnection connection, string reason)
        {
            if (connection.Equals(default(QNetConnection)))
                return;

            // Send disconnection message.
            Send(connection, QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered, QNetBaseHeader.DISCONNECTION_MESSAGE, reason);

            // and then make sure that real disconnection will be received on client after message above
            // TODO: Some simple code to wait few milliseconds in background.
            //JEMOperation.InvokeAction(0.0F, () =>
            {
                if (connection.OriginalConnection.Status == NetConnectionStatus.None ||
                    connection.OriginalConnection.Status == NetConnectionStatus.Disconnecting ||
                    connection.OriginalConnection.Status == NetConnectionStatus.Disconnected)
                    return;
                connection.OriginalConnection.Disconnect(reason);
            } //);
        }

        ///// <summary>
        /////     Tags this server instance as a host. 
        ///// </summary>
        //public void TagAsHostServer(NetConnection originalConnection)
        //{
        //    if (IsHostServer)
        //    {
        //        throw new InvalidOperationException("Host server is already initialized.");
        //    }

        //    IsHostServer = true;
        //    OriginalHostConnection = originalConnection ?? throw new ArgumentNullException(nameof(originalConnection));
        //}

        /// <summary>
        ///     Checks if given UDP port is open and QNet can bind server to it.
        /// </summary>
        public static bool IsPortOpen(ushort port) => IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners().All(p => p.Port != port);       
    }
}