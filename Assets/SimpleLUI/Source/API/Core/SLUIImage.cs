//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.UI;

namespace SimpleLUI.API.Core
{
    public sealed class SLUIImage : SLUIMaskableGraphic
    {
        internal new Image Original { get; private set; }

        public SLUIImage()
        {

        }

        /// <inheritdoc />
        internal override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.CollectComponent<Image>();
        }
    }
}
