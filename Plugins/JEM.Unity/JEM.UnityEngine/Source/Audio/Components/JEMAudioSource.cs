//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace JEM.UnityEngine.Audio.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     A simple <see cref="global::UnityEngine.AudioSource"/> volume controller.
    ///     Implements <see cref="OnStart"/> and <see cref="OnStop"/> <see cref="UnityEvent"/> based events.
    /// </summary>
    [AddComponentMenu("JEM/Audio/JEM Audio Source")]
    [RequireComponent(typeof(AudioSource)), DisallowMultipleComponent]
    public class JEMAudioSource : MonoBehaviour
    {
        /// <summary>
        ///     Type of the audio.
        /// </summary>
        [Header("Settings")]
        [Obsolete]
        public JEMAudioSourceType AudioSourceType = JEMAudioSourceType.Unknown;

        /// <summary>
        ///     Used to generate random pitch.
        /// </summary>
        [Range(0f, 0.3f)]
        public float Pith = 0f;

        /// <summary>
        ///     OnStart event called when the audio starts playing.
        /// </summary>
        [Header("Events")]
        public UnityEvent OnStart;

        /// <summary>
        ///     OnStop event called when the audio stops playing.
        /// </summary>
        public UnityEvent OnStop;

        /// <summary>
        ///     Defines whether the audio source should ignore direct updates on volume.
        /// </summary>
        public bool IgnoreDirectUpdates { get; set; } = false;

        /// <summary>
        ///     Base volume of this audio source.
        /// </summary>
        /// <remarks>
        ///     Always set to <see cref="AudioSource.volume"/> at <see cref="Awake"/>.
        /// </remarks>
        public float BaseVolume { get; set; }

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

            UpdateAudioVolume();
            _started = true;

            if (Math.Abs(Pith) > 0.05f)
            {
                AudioSource.pitch += Random.Range(-Pith, Pith);
            }
        }

        private void OnEnable() => StartCoroutine(InternalRecordPlayingState());
        private void OnDestroy()
        {
            if (_wasPlaying)
            {
                // Invoke OnStop when object was destroyed while playing
                _wasPlaying = false;
                OnStop?.Invoke();
            }

            _gameAudioSources.Remove(this);
        }

        private IEnumerator InternalRecordPlayingState()
        {
            _wasPlaying = false;
            while (true)
            {
                if (_wasPlaying != AudioSource.isPlaying)
                {
                    _wasPlaying = AudioSource.isPlaying;
                    if (_wasPlaying)
                    {
                        OnStart?.Invoke();
                    }
                    else
                    {
                        OnStop?.Invoke();
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        ///     Register <see cref="OnStart"/> action.
        /// </summary>
        public void RegisterStartEvent([NotNull] UnityAction a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (OnStart == null)
            {
                OnStart = new UnityEvent();
            }

            OnStart.AddListener(a);
        }


        /// <summary>
        ///     Register <see cref="OnStop"/> action.
        /// </summary>
        public void RegisterStopEvent([NotNull] UnityAction a)
        {
            if (a == null) throw new ArgumentNullException(nameof(a));
            if (OnStop == null)
            {
                OnStop = new UnityEvent();
            }

            OnStop.AddListener(a);
        }

        /// <summary>
        ///     Updates the volume of audio source.
        /// </summary>
        /// <remarks>
        ///     Resolves the <see cref="TargetVolume"/> from <see cref="JEMAudioManager"/> using defined <see cref="AudioSourceType"/> and <see cref="BaseVolume"/>
        /// </remarks>
        [Obsolete]
        public void UpdateAudioVolume()
        {
            TargetVolume = JEMAudioManager.GetAudioSourceVolume(BaseVolume, AudioSourceType);
            if (!IgnoreDirectUpdates || !_started)
            {
                AudioSource.volume = TargetVolume;
            }
        }

        /// <summary>
        ///     List of all custom audio sources.
        /// </summary>
        public static IReadOnlyList<JEMAudioSource> GameAudioSources => _gameAudioSources;
        private static readonly List<JEMAudioSource> _gameAudioSources = new List<JEMAudioSource>();
    }
}
