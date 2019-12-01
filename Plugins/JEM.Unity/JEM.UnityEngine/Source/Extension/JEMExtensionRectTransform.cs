//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;
using System.ComponentModel;
using UnityEngine;

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     An name of <see cref="RectTransform"/> anchor.
    /// </summary>
    public enum JEMRectAnchorName
    {
        /// <summary>
        ///     Unknown anchor that is most likely custom defined by user. 
        /// </summary>
        Unknown,

        /// <summary>
        ///     Anchored to top left of parent.
        /// </summary>
        TopLeft,

        /// <summary>
        ///     Anchored to top of parent.
        /// </summary>
        Top,

        /// <summary>
        ///     Anchored to top right of parent.
        /// </summary>
        TopRight,

        /// <summary>
        ///     Anchored to middle left of parent.
        /// </summary>
        MiddleLeft,

        /// <summary>
        ///     Anchored to middle of parent.
        /// </summary>
        Middle,

        /// <summary>
        ///     Anchored to middle right left of parent.
        /// </summary>
        MiddleRight,

        /// <summary>
        ///     Anchored to bottom left left of parent.
        /// </summary>
        BottomLeft,

        /// <summary>
        ///     Anchored to bottom of parent.
        /// </summary>
        Bottom,

        /// <summary>
        ///     Anchored to bottom right of parent.
        /// </summary>
        BottomRight,

        /// <summary>
        ///     Stretch on parent.
        /// </summary>
        Stretch,

        /// <summary>
        ///     Stretch to left of parent.
        /// </summary>
        StretchLeft,

        /// <summary>
        ///     Stretch to center of parent.
        /// </summary>
        StretchCenter,

        /// <summary>
        ///     Stretch to right of parent.
        /// </summary>
        StretchRight,

        /// <summary>
        ///     Stretch to bottom of parent.
        /// </summary>
        StretchBottom,

        /// <summary>
        ///     Stretch to middle of parent.
        /// </summary>
        StretchMiddle,

        /// <summary>
        ///     Stretch to top of parent.
        /// </summary>
        StretchTop
    }

    /// <summary>
    ///     Set of utility extensions to RectTransform class.
    /// </summary>
    public static class JEMExtensionRectTransform
    {
        /// <summary>
        ///     Tests if mouse have collision with given rectTransform.
        /// </summary>
        public static void TestRectTransformCollision(this RectTransform t, out bool isCollision)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(t, Input.mousePosition, t.GetComponentInParent<Canvas>().worldCamera, out var point);
            var p = Rect.PointToNormalized(t.rect, point);
            var pos = new Vector2(t.rect.width * p.x, t.rect.height * p.y);

            isCollision = false;
            if (pos.x > 1f && pos.y > 1f)
            {
                if (pos.x <= t.rect.width - 1f && pos.y <= t.rect.height - 1f)
                {
                    isCollision = true;
                }
            }
        }

        /// <summary>
        ///     Sets the x of <see cref="RectTransform.anchoredPosition"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void SetAnchoredX([NotNull] this RectTransform transform, float x)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.anchoredPosition = new Vector2(x, transform.anchoredPosition.y);
        }

        /// <summary>
        ///     Sets the y of <see cref="RectTransform.anchoredPosition"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void SetAnchoredY([NotNull] this RectTransform transform, float y)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.anchoredPosition = new Vector2(transform.anchoredPosition.x, y);
        }

        /// <summary>
        ///     Returns distance between this and target rectTransform sizeDelta.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float SizeDistance([NotNull] this RectTransform transform, [NotNull] RectTransform target) =>
            SizeDistance(transform, target.sizeDelta);

        /// <summary>
        ///     Returns distance between this and target sizeDelta.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float SizeDistance([NotNull] this RectTransform transform, Vector2 target)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            return Vector2.Distance(transform.sizeDelta, target);
        }

        /// <summary>
        ///     Get anchor name of given <see cref="RectTransform"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static JEMRectAnchorName GetAnchor([NotNull] this RectTransform transform)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            if (transform == null) return JEMRectAnchorName.Unknown;
            var min = transform.anchorMin;
            var max = transform.anchorMax;

            if (min.x == 0.0f && min.y == 1.0f && max.x == 0.0f && max.y == 1.0f)
                return JEMRectAnchorName.TopLeft;
            if (min.x == 0.5f && min.y == 1.0f && max.x == 0.5f && max.y == 1.0f)
                return JEMRectAnchorName.Top;
            if (min.x == 1.0f && min.y == 1.0f && max.x == 1.0f && max.y == 1.0f)
                return JEMRectAnchorName.TopRight;
            if (min.x == 0.0f && min.y == 0.5f && max.x == 0.0f && max.y == 0.5f)
                return JEMRectAnchorName.MiddleLeft;
            if (min.x == 0.5f && min.y == 0.5f && max.x == 0.5f && max.y == 0.5f)
                return JEMRectAnchorName.Middle;
            if (min.x == 1.0f && min.y == 0.5f && max.x == 1.0f && max.y == 0.5f)
                return JEMRectAnchorName.MiddleRight;
            if (min.x == 0.0f && min.y == 0.0f && max.x == 0.0f && max.y == 0.0f)
                return JEMRectAnchorName.BottomLeft;
            if (min.x == 0.5f && min.y == 0.0f && max.x == 0.5f && max.y == 0.0f)
                return JEMRectAnchorName.Bottom;
            if (min.x == 1.0f && min.y == 0.0f && max.x == 1.0f && max.y == 0.0f)
                return JEMRectAnchorName.BottomRight;
            if (min.x == 0.0f && min.y == 0.0f && max.x == 0.0f && max.y == 1.0f)
                return JEMRectAnchorName.StretchLeft;
            if (min.x == 0.5f && min.y == 0.0f && max.x == 0.5f && max.y == 1.0f)
                return JEMRectAnchorName.StretchCenter;
            if (min.x == 1.0f && min.y == 0.0f && max.x == 1.0f && max.y == 1.0f)
                return JEMRectAnchorName.StretchRight;
            if (min.x == 0.0f && min.y == 0.0f && max.x == 1.0f && max.y == 1.0f)
                return JEMRectAnchorName.Stretch;
            if (min.x == 0.0f && min.y == 0.0f && max.x == 1.0f && max.y == 0.0f)
                return JEMRectAnchorName.StretchBottom;
            if (min.x == 0.0f && min.y == 0.5f && max.x == 1.0f && max.y == 0.5f)
                return JEMRectAnchorName.StretchMiddle;
            if (min.x == 0.0f && min.y == 1.0f && max.x == 1.0f && max.y == 1.0f)
                return JEMRectAnchorName.StretchTop;

            return JEMRectAnchorName.Unknown;
        }

        /// <summary>
        ///     Set current anchor name of given rectTransform.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public static void SetAnchor([NotNull] RectTransform transform, JEMRectAnchorName anchor)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            if (!Enum.IsDefined(typeof(JEMRectAnchorName), anchor))
                throw new InvalidEnumArgumentException(nameof(anchor), (int)anchor, typeof(JEMRectAnchorName));

            var min = Vector2.zero;
            var max = Vector2.zero;
            switch (anchor)
            {
                case JEMRectAnchorName.TopLeft:
                    min.y = 1f;
                    max.y = 1f;
                    break;
                case JEMRectAnchorName.Top:
                    min.x = 0.5f;
                    min.y = 1f;
                    max.x = 0.5f;
                    max.y = 1f;
                    break;
                case JEMRectAnchorName.TopRight:
                    min = Vector2.one;
                    max = Vector2.one;
                    break;
                case JEMRectAnchorName.MiddleLeft:
                    min.y = 0.5f;
                    max.y = 0.5f;
                    break;
                case JEMRectAnchorName.Middle:
                    min.x = 0.5f;
                    min.y = 0.5f;
                    max.x = 0.5f;
                    max.y = 0.5f;
                    break;
                case JEMRectAnchorName.MiddleRight:
                    min.x = 1f;
                    min.y = 0.5f;
                    max.x = 1f;
                    max.y = 0.5f;
                    break;
                case JEMRectAnchorName.BottomLeft:
                    // nothing lul
                    break;
                case JEMRectAnchorName.Bottom:
                    min.x = 0.5f;
                    max.x = 0.5f;
                    break;
                case JEMRectAnchorName.BottomRight:
                    min.x = 1f;
                    max.x = 1f;
                    break;
                case JEMRectAnchorName.Stretch:
                    max = Vector2.one;
                    break;
                case JEMRectAnchorName.StretchLeft:
                    max.y = 1f;
                    break;
                case JEMRectAnchorName.StretchCenter:
                    min.x = 0.5f;
                    max.x = 0.5f;
                    max.y = 1f;
                    break;
                case JEMRectAnchorName.StretchRight:
                    min.x = 1f;
                    max.x = 1f;
                    max.y = 1f;
                    break;
                case JEMRectAnchorName.StretchBottom:
                    max.x = 1f;
                    break;
                case JEMRectAnchorName.StretchMiddle:
                    min.y = 0.5f;
                    max.x = 1f;
                    max.y = 0.5f;
                    break;
                case JEMRectAnchorName.StretchTop:
                    min.y = 1f;
                    max.x = 1f;
                    max.y = 1f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(anchor), anchor, null);
            }

            transform.anchorMin = min;
            transform.anchorMax = max;
        }

        /// <summary>
        ///     An rect data generated using <see cref="RectTransform.anchoredPosition"/> and <see cref="RectTransform.sizeDelta"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static Rect GetFixedRect([NotNull] this RectTransform transform)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            return new Rect(transform.anchoredPosition, transform.sizeDelta);
        }

        /// <summary>
        ///     Sets the fixed rect that could be resolved via <see cref="GetFixedRect"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void SetFixedRect([NotNull] this RectTransform transform, Rect rect)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.anchoredPosition = rect.position;
            transform.sizeDelta = rect.size;
        }

        /// <summary>
        ///     Clamps <see cref="RectTransform.sizeDelta"/>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void ClampSizeToRect(this RectTransform transform, Vector2 min, Vector2 max) =>
            ClampSizeToRect(transform, new Rect(min, max));

        /// <summary>
        ///     Clamps <see cref="RectTransform.sizeDelta"/> to given <see cref="Rect"/> where x,y is for min size, and width/height for maximal size.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void ClampSizeToRect([NotNull] this RectTransform transform, Rect rect)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            if (transform.sizeDelta.x < rect.x)
                transform.sizeDelta = new Vector2(rect.x, transform.sizeDelta.y);
            if (transform.sizeDelta.y < rect.y)
                transform.sizeDelta = new Vector2(transform.sizeDelta.x, rect.y);

            if (transform.sizeDelta.x > rect.width)
                transform.sizeDelta = new Vector2(rect.width, transform.sizeDelta.y);
            if (transform.sizeDelta.y > rect.height)
                transform.sizeDelta = new Vector2(transform.sizeDelta.x, rect.height);
        }

        public enum ClampMinMax
        {
            Negative,
            Positive,
            MixedMin,
            MixedMax
        }

        /// <summary>
        ///     Clamps <see cref="RectTransform.anchoredPosition"/> to not math given vector and given mode.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void ClampPosition([NotNull] this RectTransform transform, Vector2 min, ClampMinMax mode)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            switch (mode)
            {
                case ClampMinMax.Negative:
                    if (transform.anchoredPosition.x < min.x)
                        transform.SetAnchoredX(min.x);
                    if (transform.anchoredPosition.y < min.y)
                        transform.SetAnchoredY(min.y);
                    break;
                case ClampMinMax.Positive:
                    if (transform.anchoredPosition.x > min.x)
                        transform.SetAnchoredX(min.x);
                    if (transform.anchoredPosition.y > min.y)
                        transform.SetAnchoredY(min.y);
                    break;
                case ClampMinMax.MixedMin:
                    if (transform.anchoredPosition.x < min.x)
                        transform.SetAnchoredX(min.x);
                    if (transform.anchoredPosition.y > min.y)
                        transform.SetAnchoredY(min.y);
                    break;
                case ClampMinMax.MixedMax:
                    if (transform.anchoredPosition.x > min.x)
                        transform.SetAnchoredX(min.x);
                    if (transform.anchoredPosition.y < min.y)
                        transform.SetAnchoredY(min.y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        /// <summary>
        ///     Clamps anchoredPosition to given <see cref="Vector2"/>.
        /// </summary>
        /// <remarks>
        ///     Region should be based on <see cref="RectTransform.rect"/>.size.
        /// </remarks>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public static void ClampPositionToRegion([NotNull] this RectTransform transform, JEMRectAnchorName anchorName, Vector2 region)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            if (!Enum.IsDefined(typeof(JEMRectAnchorName), anchorName))
                throw new InvalidEnumArgumentException(nameof(anchorName), (int) anchorName, typeof(JEMRectAnchorName));

            Vector2 minScreen;
            Vector2 maxScreen;
            switch (anchorName)
            {
                case JEMRectAnchorName.Unknown:
                    break;
                case JEMRectAnchorName.TopLeft:
                    minScreen = new Vector2(transform.sizeDelta.x / 2f, -(transform.sizeDelta.y / 2f));
                    transform.ClampPosition(minScreen, ClampMinMax.MixedMin);

                    maxScreen = new Vector2(region.x - transform.sizeDelta.x / 2f, -(region.y - transform.sizeDelta.y / 2f));
                    transform.ClampPosition(maxScreen, ClampMinMax.MixedMax);     
                    break;
                case JEMRectAnchorName.Top:         
                    minScreen = new Vector2(-(region.x / 2f - transform.sizeDelta.x / 2f), -(transform.sizeDelta.y / 2f));
                    transform.ClampPosition(minScreen, ClampMinMax.MixedMin);

                    maxScreen = new Vector2(region.x / 2f - transform.sizeDelta.x / 2f,  -(region.y - transform.sizeDelta.y / 2f));
                    transform.ClampPosition(maxScreen, ClampMinMax.MixedMax);         
                    break;
                case JEMRectAnchorName.TopRight:
                    minScreen = new Vector2(-transform.sizeDelta.x / 2f, -(transform.sizeDelta.y / 2f));
                    transform.ClampPosition(minScreen, ClampMinMax.Positive);

                    maxScreen = new Vector2(-(region.x - transform.sizeDelta.x / 2f),  -(region.y - transform.sizeDelta.y / 2f));
                    transform.ClampPosition(maxScreen, ClampMinMax.Negative);        
                    break;
                case JEMRectAnchorName.MiddleLeft:
                    minScreen = new Vector2(transform.sizeDelta.x / 2f, region.y / 2f - transform.sizeDelta.y / 2f);
                    transform.ClampPosition(minScreen, ClampMinMax.MixedMin);

                    maxScreen = new Vector2(region.x - transform.sizeDelta.x / 2f, -(region.y / 2f - transform.sizeDelta.y / 2f));
                    transform.ClampPosition(maxScreen, ClampMinMax.MixedMax);                
                    break;
                case JEMRectAnchorName.Middle:      
                    minScreen = new Vector2(-(region.x / 2f - transform.sizeDelta.x / 2f), region.y / 2f - transform.sizeDelta.y / 2f);
                    transform.ClampPosition(minScreen, ClampMinMax.MixedMin);

                    maxScreen = new Vector2(region.x / 2f - transform.sizeDelta.x / 2f, -(region.y / 2f - transform.sizeDelta.y / 2f));
                    transform.ClampPosition(maxScreen, ClampMinMax.MixedMax);    
                    break;
                case JEMRectAnchorName.MiddleRight:
                    minScreen = new Vector2(-transform.sizeDelta.x / 2f, region.y / 2f - transform.sizeDelta.y / 2f);
                    transform.ClampPosition(minScreen, ClampMinMax.Positive);

                    maxScreen = new Vector2(-(region.x - transform.sizeDelta.x / 2f), -(region.y / 2f - transform.sizeDelta.y / 2f));
                    transform.ClampPosition(maxScreen, ClampMinMax.Negative);
                    break;
                case JEMRectAnchorName.BottomLeft:           
                    minScreen = new Vector2(transform.sizeDelta.x / 2f, transform.sizeDelta.y / 2f);
                    transform.ClampPosition(minScreen, ClampMinMax.Negative);

                    maxScreen = new Vector2(region.x - transform.sizeDelta.x / 2f, region.y - transform.sizeDelta.y / 2f);
                    transform.ClampPosition(maxScreen, ClampMinMax.Positive);      
                    break;
                case JEMRectAnchorName.Bottom:  
                    minScreen = new Vector2(-(region.x / 2f - transform.sizeDelta.x / 2f), transform.sizeDelta.y / 2f);
                    transform.ClampPosition(minScreen, ClampMinMax.Negative);
 
                    maxScreen = new Vector2(region.x / 2f - transform.sizeDelta.x / 2f, region.y - transform.sizeDelta.y / 2f);
                    transform.ClampPosition(maxScreen, ClampMinMax.Positive);
                    break;
                case JEMRectAnchorName.BottomRight:
                    minScreen = new Vector2(-transform.sizeDelta.x / 2f, transform.sizeDelta.y / 2f);
                    transform.ClampPosition(minScreen, ClampMinMax.MixedMax);

                    maxScreen = new Vector2(-(region.x - transform.sizeDelta.x / 2f), region.y - transform.sizeDelta.y / 2f);
                    transform.ClampPosition(maxScreen, ClampMinMax.MixedMin);
                    break;
                case JEMRectAnchorName.Stretch:
                case JEMRectAnchorName.StretchLeft:
                case JEMRectAnchorName.StretchCenter:
                case JEMRectAnchorName.StretchRight:
                case JEMRectAnchorName.StretchBottom:
                case JEMRectAnchorName.StretchMiddle:
                case JEMRectAnchorName.StretchTop:
                    throw new NotSupportedException("ClampPositionToRect currently does not support anchor of name " + anchorName);
                default:
                    throw new ArgumentOutOfRangeException(nameof(anchorName), anchorName, null);
            }
        }
    }
}
