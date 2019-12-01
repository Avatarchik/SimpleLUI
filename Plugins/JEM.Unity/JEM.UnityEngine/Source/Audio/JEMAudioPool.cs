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
    /// <inheritdoc />
    internal class JEMAudioPool : JEMPoolManager
    {
        /// <inheritdoc />
        public override IJEMPoolItem CreateItem(object[] args)
        {
            // Create new AudioSource
            var instance = CreateAudioSource();

            // Create and return JEMAudioInstance struct.
            return new JEMAudioInstance(instance);
        }

        /// <summary>
        ///     Creates new <see cref="GameObject"/> with <see cref="AudioSource"/> component on it.
        /// </summary>
        internal static JEMAudioSource CreateAudioSource()
        {
            // Create new object
            var obj = new GameObject("GameAudioSource");
            if (_parentGameObject == null)
            {
                _parentGameObject = new GameObject("GameAudioSources (Root)");
            }

            // Set as parent of _parentGameObject to not trash the hierarchy
            obj.transform.SetParent(_parentGameObject.transform);

            // Add and AudioSource component
            // var audioSource = obj.AddComponent<AudioSource>();

            // Add JEMAudioSource
            var instance = obj.AddComponent<JEMAudioSource>();
            return instance;
        }

        private static GameObject _parentGameObject;
    }
}
