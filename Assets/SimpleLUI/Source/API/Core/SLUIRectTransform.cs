//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using SimpleLUI.API.Core.Math;
using System;
using System.ComponentModel;
using UnityEngine;
using Component = UnityEngine.Component;

namespace SimpleLUI.API.Core
{
    /// <summary>
    ///     Interface window anchor name.
    /// </summary>
    public enum SLUIRectAnchorName
    {
        Unknown,
        TopLeft,
        Top,
        TopRight,
        MiddleLeft,
        Middle,
        MiddleRight,
        BottomLeft,
        Bottom,
        BottomRight,
        Stretch,
        StretchLeft,
        StretchCenter,
        StretchRight,
        StretchBottom,
        StretchMiddle,
        StretchTop
    }

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

        public SLUIRectAnchorName GetAnchor() => GetAnchor(Original);

        public void SetAnchor(string anchor)
        {
            if (Enum.TryParse<SLUIRectAnchorName>(anchor, true, out var t))
            {
                SetAnchor(t);
            } else Debug.LogError($"Failed to parse '{anchor}' in to {typeof(SLUIRectAnchorName)}");
        }

        public void SetAnchor(int anchor) => SetAnchor(Original, (SLUIRectAnchorName) anchor);
        public void SetAnchor(SLUIRectAnchorName anchor) => SetAnchor(Original, anchor);

        /// <summary>
        ///     Get current anchor name of given rectTransform.
        /// </summary>
        internal static SLUIRectAnchorName GetAnchor(RectTransform rectTransform)
        {
            if (rectTransform == null) return SLUIRectAnchorName.Unknown;
            var min = rectTransform.anchorMin;
            var max = rectTransform.anchorMax;
            if (min.x == 0.0f && min.y == 1.0f && max.x == 0.0f && min.y == 1.0f)
                return SLUIRectAnchorName.TopLeft;
            if (min.x == 0.5f && min.y == 1.0f && max.x == 0.5f && min.y == 1.0f)
                return SLUIRectAnchorName.Top;
            if (min.x == 1.0f && min.y == 1.0f && max.x == 1.0f && min.y == 1.0f)
                return SLUIRectAnchorName.TopRight;
            if (min.x == 0.0f && min.y == 0.5f && max.x == 0.0f && min.y == 0.5f)
                return SLUIRectAnchorName.MiddleLeft;
            if (min.x == 0.5f && min.y == 0.5f && max.x == 0.5f && min.y == 0.5f)
                return SLUIRectAnchorName.Middle;
            if (min.x == 1.0f && min.y == 0.5f && max.x == 1.0f && min.y == 0.5f)
                return SLUIRectAnchorName.MiddleRight;
            if (min.x == 0.0f && min.y == 0.0f && max.x == 0.0f && min.y == 0.0f)
                return SLUIRectAnchorName.BottomLeft;
            if (min.x == 0.5f && min.y == 0.0f && max.x == 0.5f && min.y == 0.0f)
                return SLUIRectAnchorName.Bottom;
            if (min.x == 1.0f && min.y == 0.0f && max.x == 1.0f && min.y == 0.0f)
                return SLUIRectAnchorName.BottomRight;
            if (min.x == 0.0f && min.y == 0.0f && max.x == 0.0f && min.y == 1.0f)
                return SLUIRectAnchorName.StretchLeft;
            if (min.x == 0.5f && min.y == 0.0f && max.x == 0.5f && min.y == 1.0f)
                return SLUIRectAnchorName.StretchCenter;
            if (min.x == 1.0f && min.y == 0.0f && max.x == 1.0f && min.y == 1.0f)
                return SLUIRectAnchorName.StretchRight;
            if (min.x == 0.0f && min.y == 0.0f && max.x == 1.0f && min.y == 1.0f)
                return SLUIRectAnchorName.Stretch;
            if (min.x == 0.0f && min.y == 0.0f && max.x == 1.0f && min.y == 0.0f)
                return SLUIRectAnchorName.StretchBottom;
            if (min.x == 0.0f && min.y == 0.5f && max.x == 1.0f && min.y == 0.5f)
                return SLUIRectAnchorName.StretchMiddle;
            if (min.x == 0.0f && min.y == 1.0f && max.x == 1.0f && min.y == 1.0f)
                return SLUIRectAnchorName.StretchTop;

            return SLUIRectAnchorName.Unknown;
        }

        /// <summary>
        ///     Set current anchor name of given rectTransform.
        /// </summary>
        internal static void SetAnchor([NotNull] RectTransform rectTransform, SLUIRectAnchorName anchor)
        {
            if (rectTransform == null) throw new ArgumentNullException(nameof(rectTransform));
            if (!Enum.IsDefined(typeof(SLUIRectAnchorName), anchor))
                throw new InvalidEnumArgumentException(nameof(anchor), (int) anchor, typeof(SLUIRectAnchorName));

            var min = Vector2.zero;
            var max = Vector2.zero;
            switch (anchor)
            {
                case SLUIRectAnchorName.TopLeft:
                    min.y = 1f;
                    max.y = 1f;
                    break;
                case SLUIRectAnchorName.Top:
                    min.x = 0.5f;
                    min.y = 1f;
                    max.x = 0.5f;
                    max.y = 1f;
                    break;
                case SLUIRectAnchorName.TopRight:
                    min = Vector2.one;
                    max = Vector2.one;
                    break;
                case SLUIRectAnchorName.MiddleLeft:
                    min.y = 0.5f;
                    max.y = 0.5f;
                    break;
                case SLUIRectAnchorName.Middle:
                    min.x = 0.5f;
                    min.y = 0.5f;
                    max.x = 0.5f;
                    max.y = 0.5f;
                    break;
                case SLUIRectAnchorName.MiddleRight:
                    min.x = 1f;
                    min.y = 0.5f;
                    max.x = 1f;
                    max.y = 0.5f;
                    break;
                case SLUIRectAnchorName.BottomLeft:
                    // nothing lul
                    break;
                case SLUIRectAnchorName.Bottom:
                    min.x = 0.5f;
                    max.x = 0.5f;
                    break;
                case SLUIRectAnchorName.BottomRight:
                    min.x = 1f;
                    max.x = 1f;
                    break;
                case SLUIRectAnchorName.Stretch:
                    max = Vector2.one;
                    break;
                case SLUIRectAnchorName.StretchLeft:
                    max.y = 1f;
                    break;
                case SLUIRectAnchorName.StretchCenter:
                    min.x = 0.5f;
                    max.x = 0.5f;
                    max.y = 1f;
                    break;
                case SLUIRectAnchorName.StretchRight:
                    min.x = 1f;
                    max.x = 1f;
                    max.y = 1f;
                    break;
                case SLUIRectAnchorName.StretchBottom:
                    max.x = 1f;
                    break;
                case SLUIRectAnchorName.StretchMiddle:
                    min.y = 0.5f;
                    max.x = 1f;
                    max.y = 0.5f;
                    break;
                case SLUIRectAnchorName.StretchTop:
                    min.y = 1f;
                    max.x = 1f;
                    max.y = 1f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null);
            }

            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
        }
    }
}
