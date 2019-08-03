//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace SimpleLUI.API.Core
{
    public sealed class SLUIRectTransform : SLUIComponent
    {
        public SLUIRectTransform Parent { get; private set; }
        internal new RectTransform Original { get; private set; }

        public SLUIRectTransform()
        {

        }

        internal override void LoadOriginalComponent()
        {
            Original = OriginalGameObject.CollectComponent<RectTransform>();
        }

        public void SetParent(SLUIRectTransform parent)
        {
            Parent = parent;
            Original.SetParent(parent?.Original);
        }
    }
}
