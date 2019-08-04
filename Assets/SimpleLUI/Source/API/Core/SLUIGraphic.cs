//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine.UI;

namespace SimpleLUI.API.Core
{
    public abstract class SLUIGraphic : SLUIComponent
    {
        public bool raycastTarget
        {
            get => Original.raycastTarget;
            set => Original.raycastTarget = value;
        }

        internal new Graphic Original { get; private set; }

        /// <inheritdoc />
        public override void OnComponentLoaded()
        {
            Original = (Graphic) OriginalComponent;
        }
    }
}
