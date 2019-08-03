//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using UnityEngine;

namespace SimpleLUI.API.Core
{
    public abstract class SLUIComponent : SLUIObject
    {
        public SLUIGameObject gameObject { get; internal set; }
        public SLUIRectTransform rectTransform => gameObject.rectTransform;

        public bool enable
        {
            get => OriginalBehaviour == null || OriginalBehaviour.enabled;
            set
            {
                if (OriginalBehaviour != null)
                    OriginalBehaviour.enabled = value;
            }
        }

        public bool isActiveAndEnabled => OriginalBehaviour == null || OriginalBehaviour.isActiveAndEnabled;

        internal GameObject OriginalGameObject => gameObject.Original;
        internal Component OriginalComponent { get; private set; }
        internal Behaviour OriginalBehaviour { get; private set; }

        internal virtual void OnComponentCreated() { }

        internal void LoadOriginalComponent()
        {
            OriginalComponent = OnLoadOriginalComponent();
            if (OriginalComponent == null)
                throw new NullReferenceException(nameof(OriginalComponent));

            if (OriginalComponent is Behaviour behaviour)
            {
                OriginalBehaviour = behaviour;
            }
        }

        internal abstract Component OnLoadOriginalComponent();
        internal virtual void OnComponentLoaded() { }
    }
}
