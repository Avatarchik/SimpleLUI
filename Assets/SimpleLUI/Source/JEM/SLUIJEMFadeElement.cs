﻿//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Interface;
using SimpleLUI.API;
using SimpleLUI.API.Core;
using UnityEngine;

namespace SimpleLUI.JEM
{
    [SLUIComponent]
    public sealed class SLUIJEMFadeElement : SLUIComponent
    {
        internal InterfaceFadeElement Original { get; private set; }

        public override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.CollectComponent<InterfaceFadeElement>();
        }
    }
}
