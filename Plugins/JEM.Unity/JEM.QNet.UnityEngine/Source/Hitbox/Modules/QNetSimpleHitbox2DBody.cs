//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    /// <inheritdoc />
    /// <summary>
    ///     Simple hitbox implementation for 2D space.
    /// </summary>
    [AddComponentMenu("JEM/QNet/Hitbox/QNet Simple Hitbox 2D Body")]
    public sealed partial class QNetSimpleHitbox2DBody : QNetObjectHitboxBody
    {
        /// <summary>
        ///     If true, script will try to collect all components at the start from all of his children.
        /// </summary>
        [Header("Simple Settings")]
        public bool AutoCollectHitboxes = true;

        private void OnNetworkSpawned()
        {
            if (AutoCollectHitboxes)
            {
                var allHitboxes = GetComponentsInChildren<QNetSimpleHitbox2D>();
                var hitboxes = new List<IQNetObjectHitbox>();
                QNetSimpleHitbox2D proximityHitbox = null;
                for (var index = 0; index < allHitboxes.Length; index++)
                {
                    var hitbox = allHitboxes[index];
                    if (!hitbox.IsProximityHitbox)
                    {
                        hitboxes.Add(hitbox);
                        continue;
                    }

                    if (proximityHitbox != null)
                        throw new NotSupportedException("There is more that on proximity hitbox detected " +
                                                        "with is not allowed.");

                    proximityHitbox = hitbox;
                }

                if (proximityHitbox == null)
                    throw new NullReferenceException("Proximity hitbox not detected.");

                // If there is only one hitbox (proximity) ad it to the hitboxes list...
                if (hitboxes.Count == 0)
                {
                    hitboxes.Add(proximityHitbox);
                }

                // Register the hitboxes.
                RegisterHitboxes(proximityHitbox, hitboxes.ToArray());
            }

            HitboxBodies.Add(this);
        }

        private void OnNetworkDestroyed() => HitboxBodies.Remove(this);

        /// <inheritdoc />
        protected override QNetObjectHitboxBodySnapshot GetNewSnapshot(int maxHitboxAmount) => new QNetSimpleHitbox2DBodySnapshot(maxHitboxAmount);

        /// <summary>
        ///     Clears the <see cref="AllHitboxBodies"/>.
        /// </summary>
        public static void ClearBodies()
        {
            HitboxBodies.Clear();
        }

        /// <summary>
        ///     List of all <see cref="QNetSimpleHitbox2DBody"/> based components in scene.
        /// </summary>
        public static IReadOnlyList<QNetSimpleHitbox2DBody> AllHitboxBodies => HitboxBodies;
        private static List<QNetSimpleHitbox2DBody> HitboxBodies { get; } = new List<QNetSimpleHitbox2DBody>();
    }
}
