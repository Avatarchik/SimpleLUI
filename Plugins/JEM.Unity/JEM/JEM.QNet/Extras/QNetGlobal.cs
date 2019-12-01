//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet.Extras
{
    /// <summary>
    ///     This class contains some static properties that are updated by last active type of given type.
    /// </summary>
    public static class QNetGlobal
    {
        /// <summary>
        ///     True when there is at least one <see cref="QNetServer"/> active.
        /// </summary>
        public static bool IsServerActive => ServerReference != null;

        /// <summary>
        ///     True when there is at least one <see cref="QNetClient"/> active.
        /// </summary>
        public static bool IsClientActive => ClientReference != null;

        /// <summary>
        ///     True when there is any peer active.
        /// </summary>
        public static bool IsAnyPeerActive => IsServerActive || IsClientActive;

        /// <summary>
        ///     Reference to the last active <see cref="QNetServer"/>.
        /// </summary>
        public static QNetServer ServerReference { get; set; }

        /// <summary>
        ///     Reference to the last active <see cref="QNetClient"/>.
        /// </summary>
        public static QNetClient ClientReference { get; set; }
    }
}
