//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using UnityEngine;

namespace SimpleLUI.API.Core.Math
{
    [Serializable]
    public class SLUIRectOffset
    {
        public int bottom;
        public int left;
        public int right;
        public int top;

        public SLUIRectOffset() { }

        public SLUIRectOffset(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
    }

    public static class SLUIRectOffsetUtil
    {
        public static RectOffset ToRealRect(this SLUIRectOffset rect)
        {
            return new RectOffset(rect.left, rect.right, rect.top, rect.bottom);
        }

        public static SLUIRectOffset ToSLUIRect(this RectOffset rect)
        {
            return new SLUIRectOffset(rect.left, rect.right, rect.top, rect.bottom);
        }
    }
}
