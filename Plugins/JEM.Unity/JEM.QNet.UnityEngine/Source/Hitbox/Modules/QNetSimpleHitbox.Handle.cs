//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    public sealed partial class QNetSimpleHitbox
    {
        public bool RayCast(Vector3 origin, Vector3 direction, out float distance, out Vector3 hitPoint,
            float rayDistance, LayerMask layerMask)
        {
            distance = 0f;
            hitPoint = Vector3.zero;

            var test = Physics.Raycast(origin, direction, out var hit, rayDistance, layerMask,
                QueryTriggerInteraction.Ignore);
            if (test)
            {
                if (hit.collider == ColliderReference)
                {
                    distance = hit.distance;
                    hitPoint = hit.point;
                }
                else test = false;
            }

            return test;
        }

        public bool SphereCast(Vector3 origin, float radius, Vector3 direction, out float distance,
            out Vector3 hitPoint,
            float rayDistance, LayerMask layerMask)
        {
            distance = 0f;
            hitPoint = Vector3.zero;

            var test = Physics.SphereCast(origin, radius, direction, out var hit, rayDistance, layerMask,
                QueryTriggerInteraction.Ignore);
            if (test)
            {
                if (hit.collider == ColliderReference)
                {
                    distance = hit.distance;
                    hitPoint = hit.point;
                }
                else test = false;
            }

            return test;
        }

        public bool OverlapSphere(Vector3 position, float radius, out float distance, out Vector3 hitPoint,
            LayerMask layer)
        {
            distance = 0f;
            hitPoint = Vector3.zero;

            var hits = Physics.OverlapSphere(position, radius, layer, QueryTriggerInteraction.Ignore);
            for (int index = 0; index < hits.Length; index++)
            {
                if (hits[0] == ColliderReference)
                {
                    hitPoint = hits[0].ClosestPoint(position);
                    distance = Vector3.Distance(position, hitPoint);
                    return true;
                }
            }

            return false;
        }
    }
}
