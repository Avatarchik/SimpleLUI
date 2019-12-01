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
    public partial class QNetClient
    {
        /// <summary>
        ///     Sends message to the server
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"/>
        /// <param name="args">Additional parameters.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void Send(Enum channel, QNetMessageMethod method, Enum header, params object[] args) =>
            Send(channel, method, GenerateOutgoingMessage(header, args));

        /// <summary>
        ///     Sends message to the server.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="header"/>
        /// <param name="args">Additional parameters.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void Send(byte channel, QNetMessageMethod method, byte header, params object[] args) =>
            Send(channel, method, GenerateOutgoingMessage(header, args));

        /// <summary>
        ///     Sends message to server.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="writer">Message to send.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void Send(Enum channel, QNetMessageMethod method, QNetMessageWriter writer)
            => Send(GetByteFromEnum(channel), method, writer);

        /// <summary>
        ///     Sends message to server.
        /// </summary>
        /// <param name="channel">Channel of message.</param>
        /// <param name="method">Method of message sending.</param>
        /// <param name="writer">Message to send.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void Send(byte channel, QNetMessageMethod method, QNetMessageWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (!Enum.IsDefined(typeof(QNetMessageMethod), method))
                throw new InvalidEnumArgumentException(nameof(method), (int)method, typeof(QNetMessageMethod));

            OriginalClient.SendMessage(writer.GetMessage(), (NetDeliveryMethod)(byte)method, channel);
        }
    }
}
