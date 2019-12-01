//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    public struct QNetSimpleHitbox2DHit : IQNetObjectHitboxHit
    {
        public QNetObjectHitboxBody Body { get; set; }
        public IQNetObjectHitbox Hitbox { get; set; }
        public bool IsValid { get; set; }

        public Vector2 HitPoint;
        public float HitDistance;
    }
}
