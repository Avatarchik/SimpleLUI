//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Diagnostics.CodeAnalysis;
using JEM.QNet.Messages;

namespace JEM.QNet.UnityEngine.Messages
{
    /// <summary>
    ///     QNet channel.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum QNetUnityChannel : byte
    {
        /// <summary>
        ///     Object default channel.
        /// </summary>
        OBJECT_QUERY = QNetBaseChannel.LAST_CHANNEL + 1,

        /// <summary>
        ///     Player default channel.
        /// </summary>
        PLAYER_QUERY,

        /// <summary>
        ///     Entities/AI default channel.
        /// </summary>
        ENTITY_QUERY,

        /// <summary>
        ///     Always last channel.
        /// </summary>
        LAST_CHANNEL
    }
}