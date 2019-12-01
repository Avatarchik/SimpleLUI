//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// ReSharper disable InconsistentNaming

namespace JEM.QNet.Messages
{
    /// <summary>
    ///     A internal and default channels used by the QNet.
    /// </summary>
    public enum QNetBaseChannel : byte
    {
        /// <summary>
        ///     Default QNet channel.
        /// </summary>
        DEFAULT,

        /// <summary>
        ///     An always last channel.
        /// </summary>
        LAST_CHANNEL
    }
}