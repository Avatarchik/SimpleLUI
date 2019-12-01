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
    ///     Hitbox sample used by <see cref="QNetSimpleHitbox2DBodySnapshot" />
    /// </summary>
    public struct QNetSimpleHitbox2DSample : IQNetObjectHitboxSample
    {
        public Vector2 WorldPosition;
        public float WorldAngle;

        /// <inheritdoc />
        public IQNetObjectHitboxSample Lerp(IQNetObjectHitboxSample target, float time)
        {
            if (target is QNetSimpleHitbox2DSample simpleSample)
            {
                return new QNetSimpleHitbox2DSample
                {
                    WorldPosition = Vector3.Lerp(WorldPosition, simpleSample.WorldPosition, time),
                    WorldAngle = Quaternion.Lerp(Quaternion.Euler(0f, 0f, WorldAngle), Quaternion.Euler(0f, 0f, simpleSample.WorldAngle), time).eulerAngles.z
                };
            }

            return default;
        }
    }
}
