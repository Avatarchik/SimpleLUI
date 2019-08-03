//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using SimpleLUI.API.Core;
using System.Collections.Generic;
using UnityEngine;

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

        public SLUIGameObject Create()
        {
            var newGameObject = new GameObject("(SLUI Object)");
            InternalInitializeGameObject(newGameObject);

            var newReference = new SLUIGameObject();
            newReference.LoadSLUIObject(Parent.Parent, newGameObject);
            InternalInitializeObject(newReference);
            return newReference;
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
