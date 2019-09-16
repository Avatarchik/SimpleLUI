//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;
using UnityEngine;

namespace JEM.UnityEngine.Handlers
{
    /// <summary>
    ///     Helps to handle currently used camera state and events.
    /// </summary>
    public static class JEMCameraHandler
    {
        /// <summary>
        ///     Apply the camera change.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void ApplyCameraChange([NotNull] Camera cam)
        {
            if (cam == null) throw new ArgumentNullException(nameof(cam));
            CameraReference = cam;
            AudioListenerReference = cam.GetComponent<AudioListener>();

            OnCameraChanged?.Invoke(cam);
        }

        /// <summary>
        ///     Event called when active camera changes.
        /// </summary>
        public static event Action<Camera> OnCameraChanged;

        /// <summary>
        ///     Reference to current/last active camera.
        /// </summary>
        public static Camera CameraReference { get; private set; }

        /// <summary>
        ///     Reference to current/last active camera transform.
        /// </summary>
        public static Transform CameraTransform => CameraReference == null ? null : CameraReference.transform;

        /// <summary>
        ///     Reference to a currently used audio listener.
        /// </summary>
        public static AudioListener AudioListenerReference { get; private set; }
    }
}
