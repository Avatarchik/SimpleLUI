﻿//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Interface;
using SimpleLUI.API.Core;
using UnityEngine;

namespace SimpleLUI.JEM
{
    [SLUIComponent]
    public sealed class SLUIJEMFadeAnimation : SLUIComponent
    {
        internal InterfaceFadeAnimation Original { get; private set; }

        public override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.AddComponent<InterfaceFadeAnimation>();
        }
    }
}
