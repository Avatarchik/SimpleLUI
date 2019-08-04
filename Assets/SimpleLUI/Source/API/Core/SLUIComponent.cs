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

        public GameObject OriginalGameObject => gameObject.Original;
        public Component OriginalComponent { get; private set; }
        public Behaviour OriginalBehaviour { get; private set; }

        public virtual void OnComponentCreated() { }

        internal void LoadOriginalComponent(SLUIManager manager)
        {
            OriginalComponent = OnLoadOriginalComponent();
            if (OriginalComponent == null)
                throw new NullReferenceException(nameof(OriginalComponent));

            LoadSLUIObject(manager, OriginalComponent);
            if (OriginalComponent is Behaviour behaviour)
            {
                OriginalBehaviour = behaviour;
            }
        }

        public abstract Component OnLoadOriginalComponent();
        public virtual void OnComponentLoaded() { }

        public SLUIComponent AddComponent(string componentName) => gameObject.AddComponent(componentName);
    }
}
