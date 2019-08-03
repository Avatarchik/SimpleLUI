//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine.UI;

namespace SimpleLUI.API.Core
{
    public abstract class SLUISelectable : SLUIComponent
    {
        internal new Selectable Original { get; private set; }

        /// <inheritdoc />
        internal override void OnComponentLoaded()
        {
            Original = (Selectable) OriginalComponent;
        }
    }
}
