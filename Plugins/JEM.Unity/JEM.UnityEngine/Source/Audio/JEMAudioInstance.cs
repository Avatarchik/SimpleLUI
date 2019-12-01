//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using JEM.UnityEngine.Audio.Components;
using UnityEngine;

namespace JEM.UnityEngine.Audio
{
    /// <inheritdoc cref="IJEMPoolItem" />
    /// <summary>
    ///     A instance of <see cref="JEMAudioData"/> received by <see cref="JEMAudioManager"/>.
    /// </summary>
    public struct JEMAudioInstance : IJEMPoolItem
    {
        /// <summary>
        ///     Reference to the <see cref="AudioSource"/> instance.
        /// </summary>
        public JEMAudioSource Instance { get; }

        /// <summary>
        ///     A data, this instance of audio was created from.
        /// </summary>
        public JEMAudioData Data { get; set; }

        /// <inheritdoc />
        public bool IsPooled { get; set; }

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMAudioInstance(JEMAudioSource instance)
        {
            Instance = instance;

            Data = default;
            IsPooled = false;
        }

        /// <inheritdoc />
        public bool IsValid() => Instance != null;

        /// <inheritdoc />
        public void Dispose()
        {
            if (Instance == null)
            {
                return;
            }

            Object.Destroy(Instance.gameObject);
        }

        /// <inheritdoc />
        public void OnPooled()
        {
            if (Instance == null)
            {
                return;
            }

            Instance.gameObject.SetActive(false);

            // Always clear onStop event when pooled
            Instance.OnStop?.RemoveAllListeners();
        }

        /// <inheritdoc />
        public void OnResolved(object[] args)
        {
            if (Instance == null)
            {
                return;
            }

            Instance.gameObject.SetActive(true);
        }
    }
}