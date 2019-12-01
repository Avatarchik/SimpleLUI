//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using Lidgren.Network;
using System;

namespace JEM.QNet
{
    /// <summary>
    ///     Base class of every network peer in QNet.
    /// </summary>
    public abstract partial class QNetPeer
    {
        /// <summary>
        ///     Called when the value of <see cref="Status"/> changes.
        /// </summary>
        public event QNetPeerStatusChanged OnStatusChanged;

        /// <summary>
        ///     Defines whether this peer started.
        /// </summary>
        public bool IsStarted { get; protected set; }

        /// <summary>
        ///     Original lidgren peer.
        /// </summary>
        public NetPeer OriginalPeer { get; protected set; }

        /// <summary>
        ///     Current configuration of peer.
        /// </summary>
        public QNetPeerConfiguration Configuration { get; protected set; }

        /// <summary>
        ///     Current connection latency.
        /// </summary>
        public float ConnectionLatency { get; protected set; }

        /// <summary>
        ///     Name of peer.
        /// </summary>
        public string PeerName { get; protected set; }

        /// <summary>
        ///     Peer status.
        /// </summary>
        public QNetPeerStatus Status
        {
            get => _status;
            set
            {
                if (_status == value) return;

                _status = value;
                OnStatusChanged?.Invoke(value);
            }
        }

        private QNetPeerStatus _status = QNetPeerStatus.NotConnected;

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected QNetPeer(string peerName)
        {
            PeerName = peerName;

            // Always try to collect serialized messages type in peer constructor.
            CollectAllSerializedMessageTypes();
        }

        /// <summary>
        ///     Gets size of network signature.
        /// </summary>
        /// <returns></returns>
        public abstract short GetSignatureSize();

        /// <summary>
        ///     Starts this peer with given configuration.
        /// </summary>
        public abstract bool Start(QNetPeerConfiguration configuration);

        /// <summary>
        ///     Stops this peer.
        /// </summary>
        public abstract void Stop(string stopReason);

        /// <summary>
        ///     Register default message handlers of peer.
        /// </summary>
        public abstract void RegisterLocalPeerHandlers();

        /// <summary>
        ///     Receive messages.
        /// </summary>
        public abstract void PollMessages();

        /// <summary>
        ///     Dispatch messages.
        /// </summary>
        public abstract void DispatchMessages();

        /// <summary>
        ///     Prepares a default configuration for <see cref="NetPeer"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static NetPeerConfiguration PrepareDefaultPeerConfig()
        {
            // Create peer configuration.
            var cfg = new NetPeerConfiguration("qnet_peer")
            {
                MaximumTransmissionUnit = 1024,
                AutoExpandMTU = false,

                EnableUPnP = false, // configuration.EnableUPnP,
            };

            // Disable not utilized messages.
            cfg.DisableMessageType(NetIncomingMessageType.ConnectionApproval);
            cfg.DisableMessageType(NetIncomingMessageType.Receipt);
            cfg.DisableMessageType(NetIncomingMessageType.DiscoveryRequest);
            cfg.DisableMessageType(NetIncomingMessageType.DiscoveryResponse);
            cfg.DisableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            cfg.DisableMessageType(NetIncomingMessageType.DebugMessage);
            cfg.DisableMessageType(NetIncomingMessageType.WarningMessage);
            cfg.DisableMessageType(NetIncomingMessageType.ErrorMessage);
            cfg.DisableMessageType(NetIncomingMessageType.NatIntroductionSuccess);

            return cfg;
        }

        /// <summary>
        ///     Try to return a byte from given enum.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static byte GetByteFromEnum(Enum e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            var val = Convert.ChangeType(e, e.GetTypeCode());
            var value = Convert.ToUInt16(val);
            return (byte) value;
        }
    }
}