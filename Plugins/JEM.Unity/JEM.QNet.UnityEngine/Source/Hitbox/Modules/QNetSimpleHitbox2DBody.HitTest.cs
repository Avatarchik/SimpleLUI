//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    public sealed partial class QNetSimpleHitbox2DBody
    {
        private static QNetSimpleHitbox2DHit RunRaycastTest(QNetObjectHitboxBody body, QNetSimpleHitbox2D hitbox,
            Vector2 origin, Vector2 direction, float rayDistance, LayerMask layer)
        {
            var hit = new QNetSimpleHitbox2DHit
            {
                Body = body,
                Hitbox = hitbox,
                IsValid = false
            };

            var hasHit = hitbox.RayCast(origin, direction, out var distance, out var hitPoint, rayDistance, layer);
            if (hasHit)
            {
                hit.IsValid = true;
                hit.HitPoint = hitPoint;
                hit.HitDistance = distance;
            }

            return hit;
        }

        private static QNetSimpleHitbox2DHit RunCircleCastTest(QNetObjectHitboxBody body, QNetSimpleHitbox2D hitbox,
            Vector2 origin, float radius, Vector2 direction, float rayDistance, LayerMask layer)
        {
            var hit = new QNetSimpleHitbox2DHit
            {
                Body = body,
                Hitbox = hitbox,
                IsValid = false
            };

            var hasHit = hitbox.CircleCast(origin, radius, direction, out var distance, out var hitPoint, rayDistance,  layer);
            if (hasHit)
            {
                hit.IsValid = true;
                hit.HitPoint = hitPoint;
                hit.HitDistance = distance;
            }

            return hit;
        }

        private static QNetSimpleHitbox2DHit RunOverlapCircleTest(QNetObjectHitboxBody body, QNetSimpleHitbox2D hitbox,
            Vector2 position, float radius, LayerMask layer)
        {
            var hit = new QNetSimpleHitbox2DHit
            {
                Body = body,
                Hitbox = hitbox,
                IsValid = false
            };

            var hasHit = hitbox.OverlapCircle(position, radius, out var distance, out var hitPoint, layer);
            if (hasHit)
            {
                hit.IsValid = true;
                hit.HitPoint = hitPoint;
                hit.HitDistance = distance;
            }

            return hit;
        }
    }
}
