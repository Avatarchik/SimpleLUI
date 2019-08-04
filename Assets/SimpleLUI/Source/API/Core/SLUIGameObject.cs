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
        public SLUIRectTransform rectTransform { get; private set; }

        public bool activeSelf => Original.activeSelf;

        public new GameObject Original { get; private set; }
        public List<SLUIComponent> Components { get; } = new List<SLUIComponent>();

        internal override void LoadSLUIObject(SLUIManager manager, Object original)
        {
            // invoke base method
            base.LoadSLUIObject(manager, original);

            // get original
            Original = (GameObject) original;

            // collect components
            rectTransform = AddComponent<SLUIRectTransform>();
        }

        // Casting in lua is not possible
        // Cant return just a SLUIComponent
        public SLUIComponent AddComponent(string componentName)
        {
            if (!componentName.StartsWith("SLUI"))
            {
                componentName = $"SLUI{componentName}";
            }

            var type = Type.GetType($"SimpleLUI.API.Core.{componentName}");
            if (type == null)
                throw new ArgumentException($"Failed to find component of type {componentName}");

            return AddComponent(type);
        }

        internal T AddComponent<T>() where T : SLUIComponent
        {
            return (T) AddComponent(typeof(T));
        }

        internal SLUIComponent AddComponent(Type type)
        {
            if (!type.IsSubclassOf(typeof(SLUIComponent)))
                throw new ArgumentException($"Type {type.FullName} is not a subclass of {typeof(SLUIComponent)}");

            var newComponent = (SLUIComponent) Activator.CreateInstance(type);
            newComponent.gameObject = this;
            newComponent.OnComponentCreated();
            newComponent.LoadOriginalComponent(Manager);
            Components.Add(newComponent);
            newComponent.OnComponentLoaded();
            Core.InternalInitializeObject(newComponent);
            return newComponent;
        }

        public void SetActive(bool activeState) => Original.SetActive(activeState);   
    }
}
