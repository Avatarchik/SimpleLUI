//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace SimpleLUI.API.Core.Math
{
    public class SLUIVector2
    {
        public float x, y;

        public SLUIVector2() { }
        public SLUIVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static SLUIVector2 Zero => new SLUIVector2(0f, 0f);
    }

    public static class SLUIVector2Util
    {
        public static Vector2 ToRealVector(this SLUIVector2 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector3 ToRealVector3(this SLUIVector2 v)
        {
            return new Vector3(v.x, v.y, 0f);
        }

        public static SLUIVector2 ToSLUIVector(this Vector2 v)
        {
            return new SLUIVector2(v.x, v.y);
        }

        public static SLUIVector2 ToSLUIVector(this Vector3 v)
        {
            return new SLUIVector2(v.x, v.y);
        }
    }
}
