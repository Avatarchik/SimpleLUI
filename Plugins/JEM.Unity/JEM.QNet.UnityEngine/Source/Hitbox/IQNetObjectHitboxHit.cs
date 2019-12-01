//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet.UnityEngine.Hitbox
{
    public interface IQNetObjectHitboxHit
    {
        QNetObjectHitboxBody Body { get; set; }
        IQNetObjectHitbox Hitbox { get; set; }
        bool IsValid { get; set; }
    }
}
