//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Collections.Generic;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    public sealed partial class QNetSimpleHitbox2DBody
    {
        #region RAYCAST
        /// <summary>
        ///     Run client-side hit test on all <see cref="QNetSimpleHitbox2DBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics2D.Raycast"/>.
        /// </summary>
        public static void ClientRayCast(Vector2 origin, Vector2 direction, IList<QNetSimpleHitbox2DHit> hits,
            float rayDistance, LayerMask layer)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ClientSideTest<QNetSimpleHitbox2D, QNetSimpleHitbox2DHit>(hits, hitbox => 
                    RunRaycastTest(body, hitbox, origin, direction, rayDistance, layer));
            }
        }

        /// <summary>
        ///     Run server-side hit test on all <see cref="QNetSimpleHitbox2DBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics2D.Raycast"/>.
        /// </summary>
        public static void ServerRayCast(uint frame, Vector2 origin, Vector2 direction, IList<QNetSimpleHitbox2DHit> hits,
            float rayDistance, LayerMask layer)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ServerSideTest<QNetSimpleHitbox2D, QNetSimpleHitbox2DHit>(frame, hits, hitbox =>
                    RunRaycastTest(body, hitbox, origin, direction, rayDistance, layer));
            }
        }
        #endregion

        #region CIRCLECAST
        /// <summary>
        ///     Run client-side hit test on all <see cref="QNetSimpleHitbox2DBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics2D.CircleCast"/>.
        /// </summary>
        public static void ClientCircleCast(Vector2 origin, float radius, Vector2 direction, IList<QNetSimpleHitbox2DHit> hits,
            float rayDistance, LayerMask layerMask)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ClientSideTest<QNetSimpleHitbox2D, QNetSimpleHitbox2DHit>(hits, hitbox => 
                    RunCircleCastTest(body, hitbox, origin, radius, direction, rayDistance, layerMask));
            }
        }

        /// <summary>
        ///     Run server-side hit test on all <see cref="QNetSimpleHitbox2DBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics2D.CircleCast"/>.
        /// </summary>
        public static void ServerCircleCast(uint frame, Vector2 origin, float radius, Vector2 direction, IList<QNetSimpleHitbox2DHit> hits,
            float rayDistance, LayerMask layerMask)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ServerSideTest<QNetSimpleHitbox2D, QNetSimpleHitbox2DHit>(frame, hits,  hitbox => 
                    RunCircleCastTest(body, hitbox, origin, radius, direction, rayDistance, layerMask));
            }
        }
        #endregion

        #region OVERLAPCIRCLE
        /// <summary>
        ///     Run client-side hit test on all <see cref="QNetSimpleHitbox2DBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics2D.OverlapCircle"/>.
        /// </summary>
        public static void ClientOverlapCircle(Vector2 position, float radius, IList<QNetSimpleHitbox2DHit> hits,
            LayerMask layer)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ClientSideTest<QNetSimpleHitbox2D, QNetSimpleHitbox2DHit>(hits, hitbox => 
                    RunOverlapCircleTest(body, hitbox, position, radius, layer));
            }
        }

        /// <summary>
        ///     Run server-side hit test on all <see cref="QNetSimpleHitbox2DBody"/> added to
        ///     <see cref="HitboxBodies"/> using <see cref="Physics2D.OverlapCircle"/>.
        /// </summary>
        public static void ServerOverlapCircle(uint frame, Vector2 position, float radius, IList<QNetSimpleHitbox2DHit> hits,
            LayerMask layer)
        {
            for (var index = 0; index < HitboxBodies.Count; index++)
            {
                var body = HitboxBodies[index];
                if (!body)
                    continue;

                body.ServerSideTest<QNetSimpleHitbox2D, QNetSimpleHitbox2DHit>(frame, hits, hitbox => 
                    RunOverlapCircleTest(body, hitbox, position, radius, layer));
            }
        }
        #endregion
    }
}
