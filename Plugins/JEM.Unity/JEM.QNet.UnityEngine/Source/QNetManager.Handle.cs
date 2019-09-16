//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using System;
using System.ComponentModel;

namespace JEM.QNet.UnityEngine
{
    // A extensions methods to QNetManager that helps reduce the Send/ToAll method invoke.
    public sealed partial class QNetManager
    {
        /// <summary>
        ///     Generate new client outgoing message that then can be send to the connected server.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="NotSupportedException"/>
        public static QNetMessageWriter GenerateClientMessage(Enum header, params object[] args) =>
            Instance.Client.GenerateOutgoingMessage(header, args);

        /// <summary>
        ///     Generate new server outgoing message that then can be send to connected clients.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="NotSupportedException"/>
        public static QNetMessageWriter GenerateServerMessage(Enum header, params object[] args) =>
            Instance.Server.GenerateOutgoingMessage(header, args);

        /// <summary>
        ///     Sends a client message to server the client is connected with.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public static void Send(Enum channel, QNetMessageMethod method, QNetMessageWriter writer) =>
            Instance.Client.Send(channel, method, writer);

        /// <summary>
        ///     Sends a client message to server the client is connected with.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public static void Send(Enum channel, QNetMessageMethod method, Enum header, params object[] args) =>
            Instance.Client.Send(channel, method, header, args);

        /// <summary>
        ///     Sends a server message to the given connection.
        /// </summary>
        public static void Send(QNetConnection target, Enum channel, QNetMessageMethod method, QNetMessageWriter writer) =>
            Instance.Server.Send(target, channel, method, writer);

        /// <summary>
        ///     Sends a server message to the given connection.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public static void Send(QNetConnection target, Enum channel, QNetMessageMethod method, Enum header, params object[] args) =>
            Instance.Server.Send(target, channel, method, header, args);

        /// <summary>
        ///     Sends a server message to all connected clients except internal one (if host).
        /// </summary>
        /// <remarks>
        ///     We except you to handle the message for internal connection by your self.
        /// </remarks>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public static void SendToAll(Enum channel, QNetMessageMethod method, QNetMessageWriter writer) =>
            Instance.Server.SendToAll(Instance.HostClientConnection, channel, method, writer);

        /// <summary>
        ///     Sends a server message to all connected clients except internal one (if host).
        /// </summary>
        /// <remarks>
        ///     We except you to handle the message for internal connection by your self.
        /// </remarks>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public static void SendToAll(Enum channel, QNetMessageMethod method, Enum header, params object[] args) =>
            Instance.Server.SendToAll(Instance.HostClientConnection, channel, method, header, args);

        /// <summary>
        ///     Closes given connection with current server.
        /// </summary>
        /// <param name="connection">Connection to close.</param>
        /// <param name="reason">Connection closing reason.</param>
        public static void CloseConnection(QNetConnection connection, string reason) => Instance.Server.CloseConnection(connection, reason);

        /// <summary>
        ///     Gets QNet connection by connection identity.
        /// </summary>
        public static QNetConnection GetConnection(ushort connectionIdentity) =>
            Instance.Server.GetConnection(connectionIdentity);

        /// <summary>
        ///     Recycle the server network message.
        /// </summary>
        public static void RecycleClient(QNetMessageReader reader) =>
            Instance.Client.OriginalClient.Recycle(reader.GetMessage());

        /// <summary>
        ///     Recycle the server network message.
        /// </summary>
        public static void RecycleServer(QNetMessageReader reader) =>
            Instance.Server.OriginalServer.Recycle(reader.GetMessage());
    }
}
