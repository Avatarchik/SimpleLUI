//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace SimpleLUI.API.Core.Math
{
    public class SLUIColor
    {
        public float r, g, b, a;

        public SLUIColor() { }
        public SLUIColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }

    public static class SLUIColorUtil
    {
        public static Color ToRealColor(this SLUIColor c)
        {
            return new Color(c.r, c.g, c.b, c.a);
        }

        public static SLUIColor ToSLUIColor(this Color c)
        {
            return new SLUIColor(c.r, c.g, c.b, c.a);
        }
    }
}
