//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Collections.Generic;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    public sealed partial class QNetSimpleHitboxBody
    {
        #region RAYCAST
        /// <summary>
        ///     Run client-side hit test on all <see cref="QNetSimpleHitboxBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics.Raycast"/>.
        /// </summary>
        public static void ClientRayCast(Vector3 origin, Vector3 direction, IList<QNetSimpleHitboxHit> hits,
            float rayDistance, LayerMask layer)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ClientSideTest<QNetSimpleHitbox, QNetSimpleHitboxHit>(hits, hitbox => 
                    RunRaycastTest(body, hitbox, origin, direction, rayDistance, layer));
            }
        }

        /// <summary>
        ///     Run server-side hit test on all <see cref="QNetSimpleHitboxBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics.Raycast"/>.
        /// </summary>
        public static void ServerRayCast(uint frame, Vector3 origin, Vector3 direction, IList<QNetSimpleHitboxHit> hits,
            float rayDistance, LayerMask layer)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ServerSideTest<QNetSimpleHitbox, QNetSimpleHitboxHit>(frame, hits, hitbox =>
                    RunRaycastTest(body, hitbox, origin, direction, rayDistance, layer));
            }
        }
        #endregion

        #region SPHERECAST 
        /// <summary>
        ///     Run client-side hit test on all <see cref="QNetSimpleHitboxBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics.SphereCast"/>.
        /// </summary>
        public static void ClientSphereCast(Vector3 origin, float radius, Vector3 direction, IList<QNetSimpleHitboxHit> hits,
            float rayDistance, LayerMask layerMask)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ClientSideTest<QNetSimpleHitbox, QNetSimpleHitboxHit>(hits, hitbox => 
                    RunSpherecastTest(body, hitbox, origin, radius, direction, rayDistance, layerMask));
            }
        }

        /// <summary>
        ///     Run server-side hit test on all <see cref="QNetSimpleHitboxBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics.SphereCast"/>.
        /// </summary>
        public static void ServerSphereCast(uint frame, Vector3 origin, float radius, Vector3 direction, IList<QNetSimpleHitboxHit> hits,
            float rayDistance, LayerMask layerMask)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ServerSideTest<QNetSimpleHitbox, QNetSimpleHitboxHit>(frame, hits,  hitbox => 
                    RunSpherecastTest(body, hitbox, origin, radius, direction, rayDistance, layerMask));
            }
        }
        #endregion

        #region OVERLAPSPHERE
        /// <summary>
        ///     Run client-side hit test on all <see cref="QNetSimpleHitboxBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics.OverlapSphere"/>.
        /// </summary>
        public static void ClientOverlapSphere(Vector3 position, float radius, IList<QNetSimpleHitboxHit> hits,
            LayerMask layer)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ClientSideTest<QNetSimpleHitbox, QNetSimpleHitboxHit>(hits, hitbox => 
                    RunOverlapSphereTest(body, hitbox, position, radius, layer));
            }
        }

        /// <summary>
        ///     Run server-side hit test on all <see cref="QNetSimpleHitboxBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics.OverlapSphere"/>.
        /// </summary>
        public static void ServerOverlapSphere(uint frame, Vector3 position, float radius, IList<QNetSimpleHitboxHit> hits,
            LayerMask layer)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ServerSideTest<QNetSimpleHitbox, QNetSimpleHitboxHit>(frame, hits, hitbox => 
                    RunOverlapSphereTest(body, hitbox, position, radius, layer));
            }
        }
        #endregion
    }
}
