//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using SimpleLUI.API.Core.Math;
using UnityEngine;

namespace SimpleLUI.API.Core
{
    public sealed class SLUIRectTransform : SLUIComponent
    {
        public SLUIVector2 position
        {
            get => Original.position.ToSLUIVector();
            set => Original.position = value.ToRealVector3();
        }

        public SLUIVector2 localPosition
        {
            get => Original.localPosition.ToSLUIVector();
            set => Original.localPosition = value.ToRealVector3();
        }

        public SLUIQuaternion rotation
        {
            get => Original.rotation.ToSLUIQuaternion();
            set => Original.rotation = value.ToRealQuaternion();
        }

        public SLUIQuaternion localRotation
        {
            get => Original.localRotation.ToSLUIQuaternion();
            set => Original.localRotation = value.ToRealQuaternion();
        }

        public SLUIVector2 eulerAngles
        {
            get => Original.eulerAngles.ToSLUIVector();
            set => Original.eulerAngles = value.ToRealVector3();
        }

        public SLUIVector2 localEulerAngles
        {
            get => Original.localEulerAngles.ToSLUIVector();
            set => Original.localEulerAngles = value.ToRealVector3();
        }

        public SLUIVector2 localScale
        {
            get => Original.localScale.ToSLUIVector();
            set => Original.localScale = value.ToRealVector3();
        }

        public SLUIVector2 anchoredPosition
        {
            get => Original.anchoredPosition.ToSLUIVector();
            set => Original.anchoredPosition = value.ToRealVector();
        }

        public SLUIVector2 anchorMax
        {
            get => Original.anchorMax.ToSLUIVector();
            set => Original.anchorMax = value.ToRealVector();
        }

        public SLUIVector2 anchorMin
        {
            get => Original.anchorMin.ToSLUIVector();
            set => Original.anchorMin = value.ToRealVector();
        }

        public SLUIVector2 offsetMax
        {
            get => Original.offsetMax.ToSLUIVector();
            set => Original.offsetMin = value.ToRealVector();
        }

        public SLUIVector2 pivot
        {
            get => Original.pivot.ToSLUIVector();
            set => Original.pivot = value.ToRealVector();
        }

        public SLUIVector2 sizeDelta
        {
            get => Original.sizeDelta.ToSLUIVector();
            set => Original.sizeDelta = value.ToRealVector();
        }

        public SLUIRectTransform Parent { get; private set; }
        internal new RectTransform Original { get; private set; }

        public SLUIRectTransform()
        {
        
        }

        internal override Component OnLoadOriginalComponent()
        {
            return Original = OriginalGameObject.CollectComponent<RectTransform>();
        }

        public void SetParent(SLUIRectTransform parent)
        {
            Parent = parent;
            Original.SetParent(parent?.Original);
        }
    }
}
