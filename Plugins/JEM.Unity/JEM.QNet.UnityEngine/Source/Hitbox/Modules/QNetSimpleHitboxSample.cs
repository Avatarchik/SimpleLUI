//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    /// <inheritdoc cref="IQNetObjectHitboxSample" />
    /// <summary>
    ///     Hitbox sample used by <see cref="QNetSimpleHitboxBodySnapshot" />
    /// </summary>
    public struct QNetSimpleHitboxSample : IQNetObjectHitboxSample
    {
        public Vector3 WorldPosition;
        public Quaternion WorldOrientation;

        /// <inheritdoc />
        public IQNetObjectHitboxSample Lerp(IQNetObjectHitboxSample target, float time)
        {
            if (target is QNetSimpleHitboxSample simpleSample)
            {
                return new QNetSimpleHitboxSample
                {
                    WorldPosition = Vector3.Lerp(WorldPosition, simpleSample.WorldPosition, time),
                    WorldOrientation = Quaternion.Lerp(WorldOrientation, simpleSample.WorldOrientation, time)
                };
            }

            return default;
        }
    }
}
