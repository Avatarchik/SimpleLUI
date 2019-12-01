//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Audio.Components;
using System;
using UnityEngine;
using UnityEngine.Audio;

namespace JEM.UnityEngine.Audio
{
    /// <summary>
    ///     Defines a type of audio source.
    ///     Utilized by <see cref="JEMAudioManager"/> system.
    /// </summary>
    /// <remarks>
    ///     IMPORTANT: We may want to implement source type definition by string for more flexibility.
    /// </remarks>
    [Obsolete]
    public enum JEMAudioSourceType
    {
        /// <summary>
        ///     The type of audio source is unknown.
        ///     You should never use this as a <see cref="JEMAudioSource"/> audio source type.
        /// </summary>
        Unknown,

        /// <summary>
        ///     Source is a interface (UI).
        /// </summary>
        Interface,

        /// <summary>
        ///     Source is a some kind of game effects.
        /// </summary>
        Effects,

        /// <summary>
        ///     Source is a game music.
        /// </summary>
        Music,

        /// <summary>
        ///     Source is a game ambient/something playing in background.
        /// </summary>
        Ambient
    }

    /// <summary>
    ///     Defines on what type of space audio should work.
    ///     
    /// </summary>
    public enum JEMAudioSpaceMode
    {
        /// <summary>
        ///     Audio will play in full 3D.
        /// </summary>
        ThreeDimensional,

        /// <summary>
        ///     Audio will play in full 3D but with fixed audio distance.
        ///     Recommended for 2D game's sound effects.
        /// </summary>
        TwoDimensional,

        /// <summary>
        ///     Audio will play in full 2D.
        /// </summary>
        UserInterface
    }

    /// <summary>
    ///     Data used to create new audio source via <see cref="JEMAudioManager" />
    /// </summary>
    public struct JEMAudioData
    {
        /// <summary>
        ///     Target <see cref="AudioClip"/> to run.
        /// </summary>
        public AudioClip Clip { get; }

        /// <summary>
        ///     World space point of <see cref="Clip"/> to play.
        /// </summary>
        public Vector3 Point { get; set; }

        /// <summary>
        ///     Defines a space mode audio should work on.
        /// </summary>
        public JEMAudioSpaceMode Mode { get; set; }

        /// <summary>
        ///     Base volume of the audio.
        ///     IMPORTANT: It's best for you to use <see cref="MixerGroup"/> instead to handle the volume of audio.
        ///                Mixing <see cref="Volume"/> with <see cref="JEMAudioSourceType"/> is possible but not recommended.
        /// </summary>
        public float Volume { get; set; }

        /// <summary>
        ///     Type of the audio.
        ///     Used to adjust final audio volume to the user's settings.
        /// </summary>
        [Obsolete]
        public JEMAudioSourceType Type { get; set; }

        /// <summary>
        ///     The <see cref="AudioMixerGroup"/> this audio should be played at.
        /// </summary>
        public AudioMixerGroup MixerGroup { get; set; }

        /// <summary>
        ///     Pitch at the <see cref="Clip"/> will be played.
        /// </summary>
        public float Pitch { get; set; }

        /// <summary>
        ///     Minimal distance of playing audio. 
        /// </summary>
        public float MinDistance { get; set; }

        /// <summary>
        ///     Maximal distance of playing audio.
        /// </summary>
        public float MaxDistance { get; set; }

        /// <summary>
        ///     Auto destroy time in seconds. Disabled if set to zero.
        /// </summary>
        public float AutoDestroy { get; set; }

        /// <summary>
        ///     Audio instance spawn event.
        /// </summary>
        public event Action<JEMAudioInstance> OnSpawned;

        /// <summary/>
        public JEMAudioData(AudioClip clip)
        {
            Clip = clip;

            Point = Vector3.zero;
            Mode = JEMAudioSpaceMode.ThreeDimensional;
            Volume = 1f;
            Type = JEMAudioSourceType.Effects;
            MixerGroup = null;
            Pitch = 1f;
            MinDistance = 5f;
            MaxDistance = 100f;
            AutoDestroy = 0;

            OnSpawned = null;
        }

        internal void CallOnSpawned(JEMAudioInstance obj) => OnSpawned?.Invoke(obj);

        /// <summary>
        ///     Creates new <see cref="JEMAudioData"/> that work in <see cref="JEMAudioSpaceMode.TwoDimensional"/> space using <see cref="JEMAudioSourceType.Effects"/> type.
        /// </summary>
        public static JEMAudioData Create2D(AudioClip clip, Vector2 point) => Create2D(clip, point, null);
       
        /// <summary>
        ///     Creates new <see cref="JEMAudioData"/> that work in <see cref="JEMAudioSpaceMode.TwoDimensional"/> space using <see cref="JEMAudioSourceType.Effects"/> type.
        /// </summary>
        public static JEMAudioData Create2D(AudioClip clip, Vector2 point, AudioMixerGroup group)
        {
            var data = new JEMAudioData(clip) {Mode = JEMAudioSpaceMode.TwoDimensional, Type = JEMAudioSourceType.Effects, Point = point, MixerGroup = group, MinDistance = 5f, MaxDistance = 15f};
            return data;
        }

        /// <summary>
        ///     Creates new <see cref="JEMAudioData"/> that work in <see cref="JEMAudioSpaceMode.UserInterface"/> space using <see cref="JEMAudioSourceType.Interface"/> type.
        /// </summary>
        public static JEMAudioData CreateUI(AudioClip clip)
        {
            var data = new JEMAudioData(clip) {Mode = JEMAudioSpaceMode.UserInterface, Type = JEMAudioSourceType.Interface};
            return data;
        }
    }
}