//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleLUI.API.Core
{
    public sealed class SLUIGameObject : SLUIObject
    {
        public SLUIRectTransform RectTransform { get; private set; }

        public new GameObject Original { get; private set; }
        public List<SLUIComponent> Components { get; } = new List<SLUIComponent>();

        internal override void LoadSLUIObject(SLUIManager manager, Object original)
        {
            // invoke base method
            base.LoadSLUIObject(manager, original);

            // get original
            Original = (GameObject) original;

            // collect components
            RectTransform = AddComponent<SLUIRectTransform>();
        }

        public T AddComponent<T>() where T : SLUIComponent
        {
            return (T) AddComponent(typeof(T));
        }

        public SLUIComponent AddComponent(Type type)
        {
            if (!type.IsSubclassOf(typeof(SLUIComponent)))
                throw new ArgumentException($"Type {type.FullName} is not a subclass of {typeof(SLUIComponent)}");

            var newComponent = (SLUIComponent) Activator.CreateInstance(type);
            newComponent.GameObject = this;
            newComponent.OnComponentCreated();
            newComponent.LoadOriginalComponent();
            Components.Add(newComponent);
            return newComponent;
        }
    }
}
