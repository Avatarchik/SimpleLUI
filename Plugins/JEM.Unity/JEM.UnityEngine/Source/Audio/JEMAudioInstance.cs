//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine.Audio
{
    /// <summary>
    ///     Instance of a audio source.
    /// </summary>
    public struct JEMAudioInstance
    {
        /// <summary>
        ///     Reference to the instance.
        /// </summary>
        public AudioSource Instance { get; }

        /// <summary>
        ///     Data this audio instance was created from.
        /// </summary>
        public JEMAudioData Data { get; }

        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMAudioInstance(AudioSource instance, JEMAudioData data)
        {
            Instance = instance;
            Data = data;
        }
    }
}