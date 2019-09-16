//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Extension.Internal;
using JetBrains.Annotations;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     Set of utility methods: AudioSource
    /// </summary>
    public static class JEMExtensionAudioSource
    {
        /// <summary>
        ///     Slowly fade out audio source volume.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void FadedOut([NotNull] this AudioSource audioSource, float speed = 15f, Action onComplete = null)
        {
            if (audioSource == null) throw new ArgumentNullException(nameof(audioSource));
            RegenerateLocalScript();
            _script.StartCoroutine(_script.InternalFadeOut(audioSource, speed, onComplete));
        }

        /// <summary>
        ///     Slowly fade in audio source volume.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void FadedIn([NotNull] this AudioSource audioSource, float speed = 15f, Action onComplete = null)
        {
            if (audioSource == null) throw new ArgumentNullException(nameof(audioSource));
            RegenerateLocalScript();
            _script.StartCoroutine(_script.InternalFadeIn(audioSource, speed, onComplete));
        }

        internal static void RegenerateLocalScript()
        {
            if (_script != null)
                return;

            var obj = new GameObject(nameof(JEMExtensionAudioSourceScript));
            Object.DontDestroyOnLoad(obj);
            _script = obj.AddComponent<JEMExtensionAudioSourceScript>();

            if (_script == null)
                throw new NullReferenceException(
                    $"System was unable to regenerate local script of {nameof(JEMExtensionAudioSource)}@{nameof(JEMExtensionAudioSourceScript)}");
        }

        private static JEMExtensionAudioSourceScript _script;
    }
}