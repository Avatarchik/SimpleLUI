//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using SimpleLUI.API.Core;
using SimpleLUI.API.Core.Math;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleLUI.API
{
    internal class SLUICore
    {
        internal SLUIWorker Parent { get; }
        private List<SLUIObject> Objects { get; } = new List<SLUIObject>();

        internal SLUICore(SLUIWorker parent)
        {
            Parent = parent;
        }

        /// <summary>
        ///     Destroys all the content of SLUI.
        /// </summary>
        internal void DestroyContent()
        {
            foreach (var obj in Objects)
            {
                Object.Destroy(obj.Original);
            }

            Objects.Clear();
        }

        public SLUIGameObject Create([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            return Create(name, SLUIVector2.Zero);
        }

        public SLUIGameObject Create([NotNull] string name, [NotNull] SLUIVector2 position)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (position == null) throw new ArgumentNullException(nameof(position));
            return Create(name, position, SLUIVector2.Zero);
        }

        public SLUIGameObject Create([NotNull] string name, [NotNull] SLUIVector2 position, [NotNull] SLUIVector2 size)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (position == null) throw new ArgumentNullException(nameof(position));
            if (size == null) throw new ArgumentNullException(nameof(size));
            var newGameObject = new GameObject(name);
            InternalInitializeGameObject(newGameObject);

            var newReference = new SLUIGameObject();
            newReference.LoadSLUIObject(Parent.Parent, newGameObject);
            InternalInitializeObject(newReference);
            return newReference;
        }

        public void Destroy([NotNull] SLUIObject obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (Objects.Contains(obj))
            {
                Objects.Remove(obj);
            }

            Object.Destroy(obj.Original);
        }

        private void InternalInitializeGameObject(GameObject obj)
        {
            obj.transform.SetParent(Parent.Parent.Canvas.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }

        private void InternalInitializeObject(SLUIObject obj)
        {
            if (Objects.Contains(obj))
            {
                return;
            }

            Objects.Add(obj);
        }
    }
}
