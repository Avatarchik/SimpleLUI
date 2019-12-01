//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    public struct QNetSimpleHitboxHit : IQNetObjectHitboxHit
    {
        public QNetObjectHitboxBody Body { get; set; }
        public IQNetObjectHitbox Hitbox { get; set; }
        public bool IsValid { get; set; }

        public Vector3 HitPoint;
        public float HitDistance;
    }
}
