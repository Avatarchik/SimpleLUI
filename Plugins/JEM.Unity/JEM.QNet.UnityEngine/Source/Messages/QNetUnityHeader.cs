//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
// ReSharper disable InconsistentNaming

namespace JEM.QNet.UnityEngine.Messages
{
    /// <summary>
    ///     QNet local header for unity extension.
    /// </summary>
    public enum QNetUnityHeader : byte
    {
        /// <summary>
        ///     Level loading message send from server to client.
        /// </summary>
        LEVEL_LOADING = QNetBaseHeader.LAST_HEADER + 1,

        /// <summary>
        ///     Level loaded message send from client to server.
        /// </summary>
        LEVEL_LOADED,

        /// <summary>
        ///     World serialization message.
        /// </summary>
        WORLD_SERIALIZATION,

        /// <summary>
        ///     World serialized message.
        /// </summary>
        WORLD_SERIALIZED,

        /// <summary>
        ///     Object create message.
        /// </summary>
        OBJECT_CREATE,

        /// <summary>
        ///     Object update state message.
        /// </summary>
        OBJECT_STATE,

        /// <summary>
        ///     Object delete message.
        /// </summary>
        OBJECT_DELETE,

        /// <summary>
        ///     Object query message.
        /// </summary>
        OBJECT_QUERY,

        /// <summary>
        ///     Entity query message.
        /// </summary>
        ENTITY_QUERY,

        /// <summary>
        ///     Always last header
        /// </summary>
        LAST_HEADER
    }
}