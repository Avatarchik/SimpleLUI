//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Collections.Generic;
using UnityEngine;

namespace JEM.UnityEngine
{
    /// <summary>
    ///     Set of utility methods: Input.
    /// </summary>
    public static class JEMInput
    {
        /// <summary>
        ///     Returns true when <see cref="Input.GetKeyDown(KeyCode)"/> has changed it's state twice in given latency.
        /// </summary>
        public static bool IsKeyDownTwice(KeyCode keyCode, float latency = 20f)
        {
            if (!Input.GetKeyDown(keyCode))
                return false;

            if (!KeyDownMemory.ContainsKey(keyCode))
            {
                KeyDownMemory.Add(keyCode, Time.time);
                return false;
            }

            var last = KeyDownMemory[keyCode];
            var twice = Time.time - last < Time.deltaTime * latency;
            KeyDownMemory[keyCode] = Time.time;
            return twice;
        }

        private static Dictionary<KeyCode, float> KeyDownMemory { get; } = new Dictionary<KeyCode, float>();
    }
}
