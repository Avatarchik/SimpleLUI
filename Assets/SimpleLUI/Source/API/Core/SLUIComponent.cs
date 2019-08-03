//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace SimpleLUI.API.Core
{
    public abstract class SLUIComponent : SLUIObject
    {
        public SLUIGameObject GameObject { get; internal set; }
        public SLUIRectTransform RectTransform => GameObject.RectTransform;

        internal GameObject OriginalGameObject => GameObject.Original;

        internal virtual void OnComponentCreated() { }
        internal abstract void LoadOriginalComponent();

    }
}
