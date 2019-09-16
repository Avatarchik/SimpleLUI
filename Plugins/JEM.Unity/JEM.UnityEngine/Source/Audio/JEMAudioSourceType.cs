//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.UnityEngine.Audio
{
    /// <summary>
    ///     Defines a type of audio source.
    ///     Utilized by <see cref="JEMAudioManager"/> system.
    /// </summary>
    public enum JEMAudioSourceType
    {
        /// <summary>
        ///     The type of audio source is unknown.
        ///     You should never use this as a <see cref="JEMCustomAudioSource"/> audio source type.
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
}