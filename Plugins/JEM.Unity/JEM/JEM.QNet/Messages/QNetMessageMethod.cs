//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet.Messages
{
    /// <summary>
    ///     How the networking deals with resends and handling of late messages
    /// </summary>
    public enum QNetMessageMethod : byte
    {
        /// <summary>
        ///     Indicates an error
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Unreliable, unordered delivery
        /// </summary>
        Unreliable = 1,

        /// <summary>
        ///     Unreliable delivery, but automatically dropping late messages
        /// </summary>
        UnreliableSequenced = 2,

        /// <summary>
        ///     Reliable delivery, but unordered
        /// </summary>
        ReliableUnordered = 34,

        /// <summary>
        ///     Reliable delivery, except for late messages which are dropped
        /// </summary>
        ReliableSequenced = 35,

        /// <summary>
        ///     Reliable, ordered delivery
        /// </summary>
        ReliableOrdered = 67
    }
}