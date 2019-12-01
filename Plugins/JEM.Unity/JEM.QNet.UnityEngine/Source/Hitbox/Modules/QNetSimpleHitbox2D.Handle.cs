//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    public sealed partial class QNetSimpleHitbox2D
    {
        public bool RayCast(Vector2 origin, Vector2 direction, out float distance, out Vector2 hitPoint,
            float rayDistance, LayerMask layerMask)
        {
            distance = 0f;
            hitPoint = Vector2.zero;

            var test = Physics2D.Raycast(origin, direction, rayDistance, layerMask);
            if (test)
            {
                if (test.collider == ColliderReference)
                {
                    distance = test.distance;
                    hitPoint = test.point;
                }
                else return false;
            }

            return test;
        }

        public bool CircleCast(Vector2 origin, float radius, Vector2 direction, out float distance,
            out Vector2 hitPoint, float rayDistance, LayerMask layerMask)
        {
            distance = 0f;
            hitPoint = Vector2.zero;

            var test = Physics2D.CircleCast(origin, radius, direction, distance, layerMask);
            if (test)
            {
                if (test.collider == ColliderReference)
                {
                    distance = test.distance;
                    hitPoint = test.point;
                }
                else return false;
            }

            return test;
        }

        public bool OverlapCircle(Vector2 position, float radius, out float distance, out Vector2 hitPoint,
            LayerMask layer)
        {
            distance = 0f;
            hitPoint = Vector2.zero;

            var test = Physics2D.OverlapCircle(position, radius, layer);
            if (test && test == ColliderReference)
            {
                hitPoint = test.ClosestPoint(position);
                distance = Vector2.Distance(position, hitPoint);
                return true;
            }

            return false;
        }
    }
}
