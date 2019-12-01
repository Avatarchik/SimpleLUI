//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// ReSharper disable InconsistentNaming

namespace JEM.QNet.Messages
{
    /// <summary>
    ///     Base QNet network headers.
    /// </summary>
    public enum QNetBaseHeader : byte
    {
        /// <summary>
        ///     Default unknown header. Should never be used!
        /// </summary>
        UNKNOWN,

        /// <summary>
        ///     Client connection request message.
        /// </summary>
        CONNECTION_MESSAGE,

        /// <summary>
        ///     Server connection initialization to client message.
        /// </summary>
        CONNECTION_SERVER_MESSAGE,

        /// <summary>
        ///     Client initialization success to server message.
        /// </summary>
        CONNECTION_CLIENT_MESSAGE,

        /// <summary>
        ///     Disconnection message from server to client.
        /// </summary>
        DISCONNECTION_MESSAGE,

        /// <summary>
        ///     Used to broadcast command data between peers.
        /// </summary>
        COMMAND_DATA,

        /// <summary>
        ///     Used to broadcast cVar data between peers.
        /// </summary>
        CVAR_DATA,

        /// <summary>
        ///     Used to broadcast log data of executor from server to client peer.
        /// </summary>
        EXECUTOR_LOGS_DATA,

        /// <summary>
        ///     Always last header.
        /// </summary>
        LAST_HEADER
    }
}