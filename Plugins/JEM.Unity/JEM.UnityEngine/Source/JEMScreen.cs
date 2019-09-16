//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine
{
    /// <summary>
    ///     Set of utility methods: Screen
    /// </summary>
    public static class JEMScreen
    {
        /// <summary>
        ///     Returns a string of given resolution.
        /// </summary>
        /// <remarks>It's just a WidthxHeight instead of WidthxHeight@hz that .ToString returns.</remarks>
        public static string ResolutionToString(Resolution resolution)
        {
            return $"{resolution.width}x{resolution.height}";
        }

        /// <summary>
        ///     Converts resolution string in to index on array of screen resolutions.
        /// </summary>
        public static int StringToResolutionIndex(string resolutionString)
        {
            for (var index = 0; index < Screen.resolutions.Length; index++)
            {
                var res = Screen.resolutions[index];
                if (ResolutionToString(res) == resolutionString)
                    return index;
            }

            return Screen.resolutions.Length - 1;
        }

        /// <summary>
        ///     Returns maximum resolution of the current screen.
        /// </summary>
        public static Resolution ScreenMaximumResolution => Screen.resolutions[Screen.resolutions.Length - 1];
    }
}
