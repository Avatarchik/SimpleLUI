//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using Lidgren.Network;

namespace JEM.QNet
{
    /// <summary/>
    public delegate void QNetServerConnectionReceived(QNetConnection connection);

    /// <summary>
    ///     Connection authorizing event.
    ///     This event is invoked when server is sending client's identity.
    ///     Here we can send some initial data like nickname or something to load.
    /// </summary>
    public delegate void QNetServerConnectionAuthorizing(QNetConnection connection, QNetMessageWriter writer, ref bool refuse);

    /// <summary/>
    public delegate void QNetServerConnectionReady(QNetMessageReader reader);

    /// <summary/>
    public delegate void QNetServerConnectionLost(QNetConnection connection, string reason);

    /// <summary/>
    public delegate void QNetServerStop(string reason);

    /// <summary/>
    public delegate void QNetServerUnconnectedDataReceived(NetIncomingMessage message);

    /// <summary/>
    public delegate void QNetServerApprovalConnection(NetIncomingMessage message);

    /// <summary/>
    public delegate void QNetServerMessagePoll(QNetMessageReader reader);

    /// <summary>
    ///     Event used by <see cref="QNetServer"/> when he receives a ping
    ///     via unconnected data and need to respond to the client.
    /// </summary>
    public delegate void QNetServerPingRespond(NetOutgoingMessage writer);

    // All the events
    public partial class QNetServer
    {
        /// <summary>
        ///     Server approve connection event.
        /// </summary>
        public event QNetServerApprovalConnection OnApproveConnection;

        /// <summary>
        ///     Connection authorizing event.
        ///     This event is invoked when server is sending client's identity.
        ///     Here we can send some initial data like nickname or something to load.
        /// </summary>
        public event QNetServerConnectionAuthorizing OnConnectionAuthorizing;

        /// <summary>
        ///     Connection lost event.
        /// </summary>
        public event QNetServerConnectionLost OnConnectionLost;

        /// <summary>
        ///     Connection ready event.
        ///     Client respond to OnConnectionAuthorizing event.
        ///     If received, client has been initialized successfully.
        /// </summary>
        public event QNetServerConnectionReady OnConnectionReady;

        /// <summary>
        ///     Connection receive event.
        /// </summary>
        public event QNetServerConnectionReceived OnConnectionReceived;

        /// <summary>
        ///     Server stop event.
        /// </summary>
        public event QNetServerStop OnServerStop;

        /// <summary>
        ///     Server unconnected data received.
        /// </summary>
        public event QNetServerUnconnectedDataReceived OnUnconnectedDataReceived;

        /// <summary>
        ///     Client message poll event.
        /// </summary>
        public event QNetClientMessagePoll OnMessagePoll;

        /// <summary>
        ///     Event used by <see cref="QNetServer"/> when he receives a ping
        ///     via unconnected data and need to respond to the client.
        /// </summary>
        public event QNetServerPingRespond OnPingRespond;
    }
}
