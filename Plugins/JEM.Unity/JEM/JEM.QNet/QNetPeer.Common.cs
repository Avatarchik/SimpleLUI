//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using Lidgren.Network;

namespace JEM.QNet
{
    /// <summary>
    ///     Result of peer message invoke.
    /// </summary>
    public struct QNetPeerMessageResult
    {
        /// <summary/>
        public bool WasSuccessful;

        /// <summary/>
        public bool DisallowToRecycle;
    }

    /// <summary>
    ///     QNetPeer status change event.
    /// </summary>
    public delegate void QNetPeerStatusChanged(QNetPeerStatus status);

    /// <summary>
    ///     QNetPeer event called at start of message generation.
    /// </summary>
    public delegate void QNetPeerBeforeMessage(NetOutgoingMessage message);

}
