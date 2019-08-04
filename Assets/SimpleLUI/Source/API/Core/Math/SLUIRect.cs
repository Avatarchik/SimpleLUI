//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace SimpleLUI.API.Core.Math
{
    public class SLUIRect
    {
        public float x, y, height, width;

        public SLUIRect() { }
        public SLUIRect(float x, float y, float height, float width)
        {
            this.x = x;
            this.y = y;
            this.height = height;
            this.width = width;
        }

        public static SLUIVector2 Zero => new SLUIVector2(0f, 0f);
    }

    public static class SLUIRectUtil
    {
        public static Rect ToRealRect(this SLUIRect r)
        {
            return new Rect(r.x, r.y, r.width, r.height);
        }

        public static SLUIRect ToSLUIRect(this Rect r)
        {
            return new SLUIRect(r.x, r.y, r.height, r.width);
        }
    }
}
