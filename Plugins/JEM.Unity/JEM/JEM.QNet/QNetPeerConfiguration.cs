//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet
{
    /// <summary>
    ///     <see cref="QNetPeer"/> configuration data.
    /// </summary>
    public class QNetPeerConfiguration
    {
        /// <summary>
        ///     Defines whether the clients can ping this peer via unconnected message.
        /// </summary>
        /// <remarks>
        ///     Will work only if <see cref="AllowUnconnectedData"/> is set to true.
        /// </remarks>
        public bool AllowToPing = true;

        /// <summary>
        ///     Defines whether the peer should read unconnected data.
        /// </summary>
        public bool AllowUnconnectedData = false;

        /// <summary>
        /// </summary>
        public bool DrawNetworkWarningMessages = false;

        /// <summary>
        ///     IpAddress with server will be bind.
        /// </summary>
        public string IpAddress = "127.0.0.1";

        /// <summary>
        ///     Maximum possible connections of the server.
        /// </summary>
        public short MaxConnections = 4;

        /// <summary>
        ///     Port with the system will try to establish connection.
        /// </summary>
        public ushort Port = 21115;

        /// <summary>
        ///     Gets or sets if library should recycling messages.
        /// </summary>
        public bool UseMessageRecycling = true;

        /// <summary>
        ///     Number of seconds timeout will be postponed on a successful ping/pong.
        /// </summary>
        public int ConnectionTimeout = 10;
    }
}