//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Extension.Internal;
using JetBrains.Annotations;
using System;
using UnityEngine;

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
            Script.StartCoroutine(Script.InternalFadeOut(audioSource, speed, onComplete));
        }

        /// <summary>
        ///     Slowly fade in audio source volume.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static void FadedIn([NotNull] this AudioSource audioSource, float speed = 15f, Action onComplete = null)
        {
            if (audioSource == null) throw new ArgumentNullException(nameof(audioSource));
            Script.StartCoroutine(Script.InternalFadeIn(audioSource, speed, onComplete));
        }

        private static JEMExtensionAudioSourceScript Script
        {
            get
            {
                if (_script == null)
                    _script = JEMExtensionAudioSourceScript.GetScript();

                return _script;
            }
        }

        private static JEMExtensionAudioSourceScript _script;
    }
}