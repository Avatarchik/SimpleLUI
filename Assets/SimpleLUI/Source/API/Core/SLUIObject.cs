//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace SimpleLUI.API.Core
{
    public abstract class SLUIObject
    {
        public Canvas Root => Manager.Canvas;
        public SLUIManager Manager { get; private set; }
        public Object Original { get; private set; }

        internal virtual void LoadSLUIObject(SLUIManager manager, Object original)
        {
            Manager = manager;
            Original = original;
        }
    }
}
