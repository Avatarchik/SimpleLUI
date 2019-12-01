//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;

namespace JEM.QNet.Messages
{
    /// <summary>
    ///     QNet message class delegate.
    /// </summary>
    /// <param name="message">Source message of this delegate.</param>
    /// <param name="reader">Reader of message</param>
    /// <param name="disallowRecycle">Disallow to recycle this message.</param>
    public delegate void QNetMessageDelegate(QNetMessage message, QNetMessageReader reader, ref bool disallowRecycle);

    /// <summary>
    ///     QNet message class.
    /// </summary>
    public struct QNetMessage
    {
        /// <summary>
        ///     Header of this message.
        /// </summary>
        public byte Header { get; }

        /// <summary>
        ///     Message delegate.
        /// </summary>
        public QNetMessageDelegate MessageDelegate { get; }

        /// <summary>
        ///     Defines whether this message has been registered on server.
        /// </summary>
        public bool IsServerMessage { get; }

        /// <summary>
        ///     Defines whether this message has been registered on client.
        /// </summary>
        public bool IsClientMessage { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Creates new message instance.
        /// </summary>
        public QNetMessage(bool isServer, Enum header, QNetMessageDelegate messageDelegate) : this(isServer, QNetPeer.GetByteFromEnum(header), messageDelegate)
        {

        }

        /// <summary>
        ///     Creates new message instance.
        /// </summary>
        public QNetMessage(bool isServer, byte header, QNetMessageDelegate messageDelegate)
        {
            IsServerMessage = isServer;
            IsClientMessage = !isServer;

            Header = header;
            MessageDelegate = messageDelegate;
        }

        /// <summary>
        ///     Resolve size of local header.
        /// </summary>
        public static short ResolveLocalHeaderSize()
        {
            return (short) Enum.GetNames(typeof(QNetBaseHeader)).Length;
        }
    }
}