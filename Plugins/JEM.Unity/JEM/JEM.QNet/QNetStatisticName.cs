//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet
{
    /// <summary>
    ///     QNet network statistic name.
    /// </summary>
    public enum QNetStatisticName
    {
        /// <summary>
        ///     Total bytes received.
        /// </summary>
        ReceivedBytes,

        /// <summary>
        ///     Total packets received.
        /// </summary>
        ReceivedPackets,

        /// <summary>
        ///     Total messages received.
        /// </summary>
        ReceivedMessages,

        /// <summary>
        ///     Total bytes sent.
        /// </summary>
        SentBytes,

        /// <summary>
        ///     Total packets sent.
        /// </summary>
        SentPackets,

        /// <summary>
        ///     Total messages sent.
        /// </summary>
        SentMessages,

        /// <summary>
        ///     Amount of messages in recycle.
        /// </summary>
        MessagesInRecycle,

        /// <summary>
        ///     Allocated bytes.
        /// </summary>
        AllocatedBytes,

        /// <summary>
        ///     Network latency of current connection.
        /// </summary>
        ConnectionLatency,

        /// <summary/>
        Dropped
    }
}