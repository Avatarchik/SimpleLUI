//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using JetBrains.Annotations;
using UnityEngine;

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     Set of utility methods: Camera
    /// </summary>
    public static class JEMExtensionCamera
    {
        /// <summary>
        ///     Interpolate camera <see cref="Camera.fieldOfView"/>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpFieldOfField([NotNull] this Camera camera, float value, float time)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, value, time);
        }

        /// <summary>
        ///     Interpolate camera <see cref="Camera.orthographicSize"/>
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpOrthographicSize([NotNull] this Camera camera, float value, float time)
        {
            if (camera == null) throw new ArgumentNullException(nameof(camera));
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, value, time);
        }
    }
}