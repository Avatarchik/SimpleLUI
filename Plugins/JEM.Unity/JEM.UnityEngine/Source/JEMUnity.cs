//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.UnityEngine.Components.Internal;
using JEM.UnityEngine.Extension.Internal;
using JEM.UnityEngine.Interface.Animation;
using JEM.UnityEngine.Internal;
using JEM.UnityEngine.Resource;
using System.Collections;
using System.Diagnostics;

namespace JEM.UnityEngine
{
    /// <summary>
    ///     Some core methods of <see cref="JEM"/> for <see cref="UnityEngine"/>.
    /// </summary>
    public static class JEMUnity
    {
        /// <summary>
        ///     Prepares all JEM scripts to work.
        ///     Prepares scripts that are used by systems that need to use for ex.: <see cref="IEnumerator"/>.
        /// </summary>
        public static void PrepareJEMScripts()
        {
            var sw = Stopwatch.StartNew();

            // JEM Operation
            JEMOperationScript.RegenerateScript();

            // JEM Game Resources
            JEMGameResourcesScript.RegenerateScript();

            // JEM Interface Fade
            JEMInterfaceFadeAnimationScript.RegenerateScript();

            // JEM Translator
            JEMTranslatorScript.RegenerateScript();

            // JEM Extensions
            JEMExtensionAudioSourceScript.RegenerateScript();
            JEMExtensionGameObjectScript.RegenerateScript();
            JEMExtensionTextScript.RegenerateScript();

            sw.Stop();
            JEMLogger.Log($"JEM scripts prepare took {sw.Elapsed.TotalSeconds:0.00} seconds.", "JEM");
        }
    }
}