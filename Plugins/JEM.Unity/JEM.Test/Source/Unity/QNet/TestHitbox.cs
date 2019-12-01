//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine.Hitbox.Modules;
using JEM.QNet.UnityEngine.Simulation;
using System.Collections.Generic;
using UnityEngine;

namespace JEM.Test.Unity.QNet
{
    public class TestHitbox : MonoBehaviour
    {
        public float RayDistance = 100f;
        public LayerMask HitMask;

        public bool AsServer;

        private void Update()
        {
            if (TestPlayer.ActivePlayer == null)
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * RayDistance);

            var hits = new List<QNetSimpleHitboxHit>();
            if (AsServer)
            {
                QNetSimpleHitboxBody.ServerRayCast(QNetTime.ServerFrame - 1, ray.origin, ray.direction, hits, RayDistance, HitMask);
            }
            else
            {
                QNetSimpleHitboxBody.ClientRayCast(ray.origin, ray.direction, hits, RayDistance, HitMask);
            }

            Debug.Log($"{hits.Count} hits");
            foreach (var h in hits)
            {
                Debug.Log(h.Hitbox.HitboxName, h.Hitbox as QNetSimpleHitbox);
            }
        }
    }
}
