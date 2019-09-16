//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using UnityEngine;

namespace JEM.UnityEngine.Audio
{
    /// <summary>
    ///     Data used to create new audio source via <see cref="JEMAudioManager"/>
    /// </summary>
    public struct JEMAudioData
    {
        /// <summary>
        ///     Audio clip to play.
        /// </summary>
        public AudioClip Clip { get; }

        /// <summary>
        ///     World space point of audio.
        /// </summary>
        public Vector3 Point;

        /// <summary>
        ///     If false, the audio will be played in 3D.
        /// </summary>
        public bool Is2D;

        /// <summary>
        ///     Base volume of the audio.
        /// </summary>
        public float Volume;

        /// <summary>
        ///     Type of the audio. Used to adjust audio source volume to user settings.
        /// </summary>
        public JEMAudioSourceType Type;

        /// <summary>
        ///     Pitch of audio.
        /// </summary>
        public float Pitch;

        /// <summary>
        ///     Min distance.
        /// </summary>
        public float MinDistance;

        /// <summary>
        ///     Max distance.
        /// </summary>
        public float MaxDistance;

        /// <summary>
        ///     Auto destroy time in seconds. Disabled if set to zero.
        /// </summary>
        public float AutoDestroy;

        /// <summary>
        ///     Audio instance spawn event.
        /// </summary>
        public Action<JEMAudioInstance> OnSpawned;

        /// <summary/>
        public JEMAudioData(AudioClip clip)
        {
            Clip = clip;

            Point = Vector3.zero;
            Is2D = false;
            Volume = 1f;
            Type = JEMAudioSourceType.Effects;
            Pitch = 1f;
            MinDistance = 5f;
            MaxDistance = 100f;
            AutoDestroy = 0;

            OnSpawned = null;
        }
    }
}