//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine.Audio.Interface
{
    /// <inheritdoc />
    /// <summary>
    ///     A database asset that contains all <see cref="AudioClip"/> references that
    ///      <see cref="JEMAudioForControl"/> based types could utilize.
    /// </summary>
    [CreateAssetMenu(fileName = "newInterfaceAudioDatabase", menuName = "JEM/Audio/Interface Audio Database", order = 0)]
    public class JEMAudioInterfaceDatabase : ScriptableObject
    {
        [Header("Base Settings")]
        public AudioClip ControlEnter;
        public AudioClip ControlClick;
        public AudioClip ControlCheck;
        public AudioClip ControlExpand;
        public AudioClip ControlMoveEdit;
        public AudioClip ControlEndEdit;

        /// <summary>
        ///     Reference to the active <see cref="JEMAudioInterfaceDatabase"/> asset.
        /// </summary>
        public static JEMAudioInterfaceDatabase Instance => JEMAudioManager.Instance == null ? null : JEMAudioManager.Instance.AudioInterfaceDatabase;
    }
}
