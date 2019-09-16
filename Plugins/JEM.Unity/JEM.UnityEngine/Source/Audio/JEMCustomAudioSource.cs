//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JEM.UnityEngine.Audio
{
    /// <inheritdoc />
    /// <summary>
    ///     JEM Custom AudioSource.
    ///     Handles the audio source volume.
    /// </summary>
    [RequireComponent(typeof(AudioSource)), DisallowMultipleComponent]
    public class JEMCustomAudioSource : MonoBehaviour
    {
        [Header("Settings")]
        public JEMAudioSourceType AudioSourceType = JEMAudioSourceType.Unknown;

        [Header("Events")]
        public UnityEvent OnStart;
        public UnityEvent OnStop;

        /// <summary>
        ///     Defines whether the audio source should ignore direct updates on volume.
        /// </summary>
        public bool IgnoreDirectUpdates { get; set; } = false;

        /// <summary>
        ///     Base/initial volume of this audio source.
        /// </summary>
        public float BaseVolume { get; private set; }

        /// <summary>
        ///     Currently targeted volume of this audio source.
        /// </summary>
        public float TargetVolume { get; private set; }

        /// <summary>
        ///     Reference to the audio source.
        /// </summary>
        public AudioSource AudioSource { get; private set; }

        private bool _wasPlaying;
        private bool _started;

        private void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
            BaseVolume = AudioSource.volume;

            _gameAudioSources.Add(this);

            UpdateAudioSource();
            _started = true;
        }

        private void Start() => StartCoroutine(InternalRecordPlayingState());    
        private void OnDestroy()
        {
            _gameAudioSources.Remove(this);
        }

        private IEnumerator InternalRecordPlayingState()
        {
            while (true)
            {
                if (_wasPlaying != AudioSource.isPlaying)
                {
                    _wasPlaying = AudioSource.isPlaying;
                    if (_wasPlaying)
                    {
                        OnStart.Invoke();
                    }
                    else
                    {
                        OnStop.Invoke();
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        ///     Updates the volume of audio source.
        /// </summary>
        /// <remarks>
        ///     Resolves the <see cref="TargetVolume"/> from <see cref="JEMAudioManager"/> using defined <see cref="AudioSourceType"/> and <see cref="BaseVolume"/>
        /// </remarks>
        public void UpdateAudioSource(JEMKeyBasedDatabase customDatabase = null)
        {
            TargetVolume = JEMAudioManager.GetAudioSourceVolume(BaseVolume, AudioSourceType, customDatabase);
            if (!IgnoreDirectUpdates || !_started)
            {
                AudioSource.volume = TargetVolume;
            }
        }

        /// <summary>
        ///     List of all custom audio sources.
        /// </summary>
        public static IReadOnlyList<JEMCustomAudioSource> GameAudioSources => _gameAudioSources;
        private static readonly List<JEMCustomAudioSource> _gameAudioSources = new List<JEMCustomAudioSource>();
    }
}
