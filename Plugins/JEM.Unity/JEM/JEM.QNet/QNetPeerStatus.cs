//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet
{
    /// <summary>
    ///     A status of QNet Peer.
    /// </summary>
    public enum QNetPeerStatus : byte
    {
        /// <summary>
        ///     Not connected with another peer.
        /// </summary>
        NotConnected,

        /// <summary>
        ///     Trying to connect with selected peer.
        /// </summary>
        Connecting,

        /// <summary>
        ///     Connected with other peer.
        /// </summary>
        Connected,

        /// <summary>
        ///     Disconnected/not connected with any peer.
        /// </summary>
        Disconnected
    }
}