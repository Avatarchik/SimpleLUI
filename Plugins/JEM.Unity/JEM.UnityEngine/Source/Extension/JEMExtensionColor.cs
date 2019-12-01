//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     Set of utility extensions to Color class.
    /// </summary>
    public static class JEMExtensionColor
    {
        /// <summary>
        ///     Multiply color's alpha.
        /// </summary>
        public static Color MultiplyAlpha(this Color color, float alpha)
        {
            return new Color
            {
                r = color.r,
                g = color.g,
                b = color.b,
                a = color.a * alpha
            };
        }

        /// <summary>
        ///     Returns a magnitude of colors (all channels combined).
        /// </summary>
        public static float Magnitude(this Color c) => c.a + c.g + c.b + c.a;
    }
}