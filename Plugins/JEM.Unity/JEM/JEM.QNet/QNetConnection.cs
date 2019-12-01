//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.QNet.Messages;
using Lidgren.Network;
using System;
using System.ComponentModel;

namespace JEM.QNet
{
    /// <summary>
    ///     QNet server connection.
    /// </summary>
    public struct QNetConnection
    {
        /// <summary>
        ///     QNetServer of this connection. Obviously on client peer is equal null.
        /// </summary>
        public QNetServer Server { get; }

        /// <summary>
        ///     Original lidgren connection reference.
        /// </summary>
        public NetConnection OriginalConnection { get; }

        /// <summary>
        ///     Identity of connection.
        /// </summary>
        public ushort ConnectionIdentity { get; }

        /// <summary>
        ///     Address of connection.
        /// </summary>
        public string ConnectionIpAddress => OriginalConnection.RemoteEndPoint.Address.ToString();

        /// <summary>
        ///     Port of connection.
        /// </summary>
        public int ConnectionPort => OriginalConnection.RemoteEndPoint.Port;

        /// <summary>
        ///     Creates new connection instance.
        /// </summary>
        public QNetConnection(QNetServer server, NetConnection originalConnection, ushort connectionIdentity)
        {
            Server = server;
            OriginalConnection = originalConnection;
            ConnectionIdentity = connectionIdentity;
        }

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"></param>
        /// <param name="args">Additional parameters.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public void Send(Enum channel, QNetMessageMethod method, Enum header, params object[] args) =>
            Send(QNetPeer.GetByteFromEnum(channel), method, QNetPeer.GetByteFromEnum(header), args);

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"></param>
        /// <param name="args">Additional parameters.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public void Send(byte channel, QNetMessageMethod method, byte header, params object[] args) =>
            Send(channel, method, Server.GenerateOutgoingMessage(header, args));

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public void Send(Enum channel, QNetMessageMethod method, Enum header) => 
            Send(QNetPeer.GetByteFromEnum(channel), method, QNetPeer.GetByteFromEnum(header));

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public void Send(byte channel, QNetMessageMethod method, byte header)
        {
            if (Server == null)
                throw new InvalidOperationException("This method can only be executed on " +
                                                    "server initialized connection.");
            Server.Send(this, channel, method, Server.GenerateOutgoingMessage(header));
        }

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="writer">Message to send.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public void Send(Enum channel, QNetMessageMethod method, QNetMessageWriter writer) =>
            Send(QNetPeer.GetByteFromEnum(channel), method, writer);

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="writer">Message to send.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public void Send(byte channel, QNetMessageMethod method, QNetMessageWriter writer)
        {
            if (Server == null)
                throw new InvalidOperationException("This method can only be executed on " +
                                                    "server initialized connection.");
            Server.Send(this, channel, method, writer);
        }

        /// <summary>
        ///     Close this connection with server.
        /// </summary>
        /// <param name="reason">Reason of closing connection.</param>
        /// <exception cref="InvalidOperationException"/>
        public void CloseConnection(string reason)
        {
            if (Server == null)
                throw new InvalidOperationException("This method can only be executed on " +
                                                    "server initialized connection.");
            if (reason == null)
                reason = string.Empty;

            Server.CloseConnection(this, reason);
        }

        /// <summary>
        ///     Log operation of connection.
        /// </summary>
        /// <param name="type">Type of log.</param>
        /// <param name="content">Operation content.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public void LogOperation(JEMLogType type, string content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (!Enum.IsDefined(typeof(JEMLogType), type))
                throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(JEMLogType));
            if (Server == null)
                throw new InvalidOperationException("This method can only be executed on " +
                                                    "server initialized connection.");
            var text = $"({ToStringExtended()}) {content}";
            switch (type)
            {
                case JEMLogType.Log:
                    JEMLogger.Log(text, "QNETCONNECTION");
                    break;
                case JEMLogType.Warning:
                    JEMLogger.LogWarning(text, "QNETCONNECTION");
                    break;
                case JEMLogType.Error:
                    JEMLogger.LogError(text, "QNETCONNECTION");
                    break;
                case JEMLogType.Exception:
                    JEMLogger.LogException(text, "<N/A>", "QNETCONNECTION");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        /// <summary>
        ///     Extended ToString that contains more data about this connection.
        /// </summary>
        public string ToStringExtended() => Server == null ? ToString() : $"{ToString()}/{ConnectionIpAddress}:{ConnectionPort}";

        /// <inheritdoc />
        public override string ToString() => $"QNetConn-{ConnectionIdentity}";
        
    }
}