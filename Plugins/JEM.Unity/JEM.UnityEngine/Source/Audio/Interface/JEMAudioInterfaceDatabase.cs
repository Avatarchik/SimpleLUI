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
    ///     Database of game audio clips for interface.
    /// </summary>
    [CreateAssetMenu(fileName = "newInterfaceAudioDatabase", menuName = "JEM/Audio/Interface Audio Database", order = 0)]
    public class JEMAudioInterfaceDatabase : ScriptableObject
    {
        [Header("Base Settings")]
        public AudioClip ControlEnter;

        [Header("Button Settings")]
        public AudioClip ButtonClick;

        [Header("Toggle Settings")]
        public AudioClip ToggleClick;

        [Header("Dropdown Settings")]
        public AudioClip DropdownClick;

        public static JEMAudioInterfaceDatabase Instance => JEMAudioManager.Instance.AudioInterfaceDatabase;
    }
}
