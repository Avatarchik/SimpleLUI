//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using Lidgren.Network;

namespace JEM.QNet
{
    /// <summary>
    ///     QNetClient ping update event.
    /// </summary>
    /// <returns>If the method returns False, it means that is ping process is no longer running.</returns>
    public delegate bool QNetClientPingUpdate();

    /// <summary>
    ///     QNetClient ping result event.
    /// </summary>
    /// <param name="isSuccessful">True if ping was successful and target server responded.</param>
    /// <param name="latency">A latency (RTT). Always -1 if <paramref name="isSuccessful"/> is false.</param>
    /// <param name="message">The message received as a respond of our ping.</param>
    public delegate void QNetClientPingResult(bool isSuccessful, int latency, NetIncomingMessage message);

    /// <summary>
    ///     QNetClient connection event.
    /// </summary>
    public delegate void QNetClientConnected();

    /// <summary>
    ///     QNetClient connection ready event.
    /// </summary>
    public delegate void QNetClientConnectionReady(QNetMessageReader reader, QNetMessageWriter writer);

    /// <summary>
    ///     QNetClient disconnection event.
    /// </summary>
    /// <param name="lostConnection">
    ///     Defines whether the client lost connection. False means, that client was unable to connect
    ///     with target server.
    /// </param>
    /// <param name="reason">Disconnection reason.</param>
    public delegate void QNetClientDisconnection(bool lostConnection, string reason);

    /// <summary>
    ///     QNetClient reads the message and adds it to the poll.
    /// </summary>
    public delegate void QNetClientMessagePoll(QNetMessageReader reader);

    public partial class QNetClient
    {
        /// <summary>
        ///     Client connected event.
        /// </summary>
        public event QNetClientConnected OnConnected;

        /// <summary>
        ///     Client connection ready event.
        /// </summary>
        public event QNetClientConnectionReady OnConnectionReady;

        /// <summary>
        ///     Client disconnection event.
        /// </summary>
        public event QNetClientDisconnection OnDisconnection;

        /// <summary>
        ///     Client message poll event.
        /// </summary>
        public event QNetClientMessagePoll OnMessagePoll;
    }
}
