//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using Lidgren.Network;
using System;
using System.ComponentModel;

namespace JEM.QNet
{
    public partial class QNetServer
    {
        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="connection">Target connection of message.</param>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"/>
        /// <param name="args">Additional parameters.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void Send(QNetConnection connection, Enum channel, QNetMessageMethod method, Enum header,
            params object[] args) =>
            Send(connection, channel, method, GenerateOutgoingMessage(header, args));

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="connection">Target connection of message.</param>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"/>
        /// <param name="args">Additional parameters.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void Send(QNetConnection connection, byte channel, QNetMessageMethod method, byte header,
            params object[] args) =>
            Send(connection, channel, method, GenerateOutgoingMessage(header, args));

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="connection">Target connection of message.</param>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void Send(QNetConnection connection, Enum channel, QNetMessageMethod method, Enum header) =>
            Send(connection, channel, method, GenerateOutgoingMessage(header));

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="connection">Target connection of message.</param>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void Send(QNetConnection connection, byte channel, QNetMessageMethod method, byte header) =>
            Send(connection, channel, method, GenerateOutgoingMessage(header));

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="connection">Target connection of message.</param>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="writer">Message to send.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void Send(QNetConnection connection, Enum channel, QNetMessageMethod method, QNetMessageWriter writer) =>
            Send(connection, GetByteFromEnum(channel), method, writer);

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="connection">Target connection of message.</param>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="writer">Message to send.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void Send(QNetConnection connection, byte channel, QNetMessageMethod method, QNetMessageWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (!Enum.IsDefined(typeof(QNetMessageMethod), method))
                throw new InvalidEnumArgumentException(nameof(method), (int) method, typeof(QNetMessageMethod));
            OriginalServer.SendMessage(writer.GetMessage(), connection.OriginalConnection,
                (NetDeliveryMethod) (byte) method, channel);
        }

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="except">Message will be send to all connections except this.</param>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"/>
        /// <param name="args">Additional parameters to serialize.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void SendToAll(QNetConnection except, Enum channel, QNetMessageMethod method, Enum header, params object[] args) =>
            SendToAll(except, channel, method, GenerateOutgoingMessage(header, args));

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"/>
        /// <param name="args">Additional parameters to serialize.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void SendToAll(Enum channel, QNetMessageMethod method, Enum header, params object[] args) =>
            SendToAll(channel, method, GenerateOutgoingMessage(header, args));

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void SendToAll(byte channel, QNetMessageMethod method, byte header) =>
            SendToAll(channel, method, GenerateOutgoingMessage(header));

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="writer">Message to send.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void SendToAll(Enum channel, QNetMessageMethod method, QNetMessageWriter writer) =>
            SendToAll(default(QNetConnection), channel, method, writer);

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="writer">Message to send.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void SendToAll(byte channel, QNetMessageMethod method, QNetMessageWriter writer) =>
            SendToAll(default(QNetConnection), channel, method, writer);

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="except">Message will be send to all connections except this.</param>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="writer">Message to send.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void SendToAll(QNetConnection except, Enum channel, QNetMessageMethod method,
            QNetMessageWriter writer) =>
            SendToAll(except, GetByteFromEnum(channel), method, writer);

        /// <summary>
        ///     Sends message to given connection.
        /// </summary>
        /// <param name="except">Message will be send to all connections except this.</param>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="writer">Message to send.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void SendToAll(QNetConnection except, byte channel, QNetMessageMethod method, QNetMessageWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (!Enum.IsDefined(typeof(QNetMessageMethod), method))
                throw new InvalidEnumArgumentException(nameof(method), (int) method, typeof(QNetMessageMethod));
            OriginalServer.SendToAll(writer.GetMessage(),
                except.Equals(default(QNetConnection)) ? null : except.OriginalConnection,
                (NetDeliveryMethod) (byte) method, channel);
        }
    }
}
