//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    public sealed partial class QNetSimpleHitboxBody
    {
        private static QNetSimpleHitboxHit RunRaycastTest(QNetObjectHitboxBody body, QNetSimpleHitbox hitbox,
            Vector3 origin, Vector3 direction, float rayDistance, LayerMask layer)
        {
            var hit = new QNetSimpleHitboxHit
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

        private static QNetSimpleHitboxHit RunSpherecastTest(QNetObjectHitboxBody body, QNetSimpleHitbox hitbox,
            Vector3 origin, float radius, Vector3 direction, float rayDistance, LayerMask layer)
        {
            var hit = new QNetSimpleHitboxHit
            {
                Body = body,
                Hitbox = hitbox,
                IsValid = false
            };

            var hasHit = hitbox.SphereCast(origin, radius, direction, out var distance, out var hitPoint, rayDistance,
                layer);
            if (hasHit)
            {
                hit.IsValid = true;
                hit.HitPoint = hitPoint;
                hit.HitDistance = distance;
            }

            return hit;
        }

        private static QNetSimpleHitboxHit RunOverlapSphereTest(QNetObjectHitboxBody body, QNetSimpleHitbox hitbox,
            Vector3 position, float radius, LayerMask layer)
        {
            var hit = new QNetSimpleHitboxHit
            {
                Body = body,
                Hitbox = hitbox,
                IsValid = false
            };

            var hasHit = hitbox.OverlapSphere(position, radius, out var distance, out var hitPoint, layer);
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
