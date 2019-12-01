//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet.UnityEngine.Hitbox
{
    /// <summary>
    ///     QNetObjectBodySnapshot implements snapshots handling for objects hitboxes.
    /// </summary>
    public abstract class QNetObjectHitboxBodySnapshot
    {
        /// <summary>
        ///     The frame at witch this snapshot was performed.
        /// </summary>
        public uint Frame { get; set; }

        /// <summary>
        ///     When true, object of this snapshot was active while this snapshot was performed.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        ///     The optional proximity hitbox sample.
        /// </summary>
        public IQNetObjectHitboxSample ProximitySample { get; set; }

        /// <summary>
        ///     List of hitboxes samples.
        /// </summary>
        public IQNetObjectHitboxSample[] HitboxesSamples { get; protected set; }
    }
}
