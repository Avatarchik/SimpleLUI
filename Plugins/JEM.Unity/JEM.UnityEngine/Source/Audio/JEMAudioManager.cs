//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// TODO: JEMAudioManager refactor needed.

using JEM.UnityEngine.Audio.Components;
using JEM.UnityEngine.Audio.Interface;
using JEM.UnityEngine.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace JEM.UnityEngine.Audio
{
    /// <inheritdoc />
    /// <summary>
    ///     A simple audio manager system.
    /// </summary>
    [AddComponentMenu("JEM/Systems/JEM Audio Manager"), DisallowMultipleComponent]
    public class JEMAudioManager : JEMSingletonBehaviour<JEMAudioManager>
    {
        [Header("References Settings")]
        public JEMAudioInterfaceDatabase AudioInterfaceDatabase;

        /// <summary>
        ///     Default <see cref="AudioMixerGroup"/> used when there is not group defined by user.
        /// </summary>
        [Header("Default Groups")]
        public AudioMixerGroup DefaultGroup;

        /// <summary>
        ///     Default <see cref="AudioMixerGroup"/> used for UI sfx when there is not group defined by user.
        /// </summary>
        public AudioMixerGroup DefaultUIGroup;

        /// <inheritdoc />
        protected override void OnAwake()
        {
            // Ignore
        }

        /// <summary>
        ///     Collects all audio sources in scene and disables them.
        /// </summary>
        /// <remarks>     
        ///     The main goal of this method is to try to disable
        ///      unity's spam about no audio listener in scene on scene load.
        /// </remarks>
        public static void CollectAndDisableLevelAudioSources()
        {
            ReleaseCollection();
            CollectedAudioSources.AddRange(FindObjectsOfType<AudioSource>());
            for (var index = 0; index < CollectedAudioSources.Count; index++)
            {
                var c = CollectedAudioSources[index];
                c.enabled = false;
            }
        }

        /// <summary>
        ///     Releases all audio sources collected by <see cref="CollectAndDisableLevelAudioSources"/>.
        /// </summary>
        public static void ReleaseCollection()
        {
            for (var index = 0; index < CollectedAudioSources.Count; index++)
            {
                var s = CollectedAudioSources[index];
                if (s == null) continue;
                s.enabled = true;
            }

            CollectedAudioSources.Clear();
        }

        /// <summary>
        ///     It attempts to fix all audio listeners by destroying all except the one that is currently on Camera.main.
        /// </summary>
        /// <remarks>
        ///     This is sad that we just can't disable those <see cref="AudioListener"/> that are don't currently needed :(
        /// </remarks>
        public static void FixAudioListeners()
        {
            var audioListeners = FindObjectsOfType<AudioListener>();
            var mainListener = JEMCameraHandler.AudioListenerReference;
            for (var index = 0; index < audioListeners.Length; index++)
            {
                var a = audioListeners[index];
                if (a == mainListener) continue;
                Destroy(a);
            }
        }

        /// <summary>
        ///     Updates all currently registered game audio sources.
        /// </summary>
        public static void UpdateAudioSources()
        {
            // Update JEMAudioSources
            foreach (var gameAudioSource in JEMAudioSource.GameAudioSources)
                gameAudioSource.UpdateAudioVolume();
        }

        /// <summary>
        ///     Gets volume of audio source of given type.
        /// </summary>
        [Obsolete]
        public static float GetAudioSourceVolume(float baseVolume, JEMAudioSourceType type)
        {
            if (OnMixAudioVolume == null)
                return baseVolume;

            return OnMixAudioVolume(type, baseVolume);
        }

        /// <summary>
        ///     Spawns new audio source using given data.
        /// </summary>
        public static void SpawnDefault(JEMAudioData data)
        {
            if (Instance == null) throw new NullReferenceException("To spawn audio you first need to define the " +
                                                                   "JEMAudioManager component on your scene.");
            if (data.Clip == null) return;
            Instance.StartCoroutine(SpawnDefaultWorker(data));
        }

        /// <summary>
        ///     Spawns new audio source in this frame.
        /// </summary>
        public static JEMAudioInstance SpawnHard(JEMAudioData data)
        {
            if (data.Clip == null) throw new ArgumentNullException(nameof(data.Clip), "Clip of target JEMAudioData was not set.");
            var instance = CreateAudioInstance(data);
            instance.Data.CallOnSpawned(instance);
            return instance;
        }

        private static IEnumerator SpawnDefaultWorker(JEMAudioData data)
        {
            var instance = CreateAudioInstance(data);
            yield return instance;
            instance.Data.CallOnSpawned(instance);
        }

        /// <summary>
        ///     Creates new <see cref="JEMAudioInstance"/> from given <see cref="JEMAudioData"/>.
        /// </summary>
        private static JEMAudioInstance CreateAudioInstance(JEMAudioData data)
        {
            // Resolve item.
            var item = Spawned.ResolveItem();
            if (!(item is JEMAudioInstance instance))
                throw new InvalidOperationException();

            // Get AudioSource reference.
            var source = instance.Instance.AudioSource;

            // Set transform.
            if (data.Mode == JEMAudioSpaceMode.UserInterface && JEMCameraHandler.AudioListenerReference != null)
            {
                // When using UserInterface mode, plug audio in to active camera.
                source.transform.SetParent(JEMCameraHandler.AudioListenerReference.transform);
                source.transform.localPosition = Vector3.zero;
            }
            else
            {
                // Apply world audio position
                source.transform.localPosition = data.Point;
            }

            // Apply base audio info.
            source.clip = data.Clip;
            source.loop = false;

            // Apply mixer group
            if (data.MixerGroup == null)
            {
                switch (data.Mode)
                {
                    case JEMAudioSpaceMode.ThreeDimensional:
                    case JEMAudioSpaceMode.TwoDimensional:
                        source.outputAudioMixerGroup = Instance.DefaultGroup;
                        break;
                    case JEMAudioSpaceMode.UserInterface:
                        source.outputAudioMixerGroup = Instance.DefaultUIGroup;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else source.outputAudioMixerGroup = data.MixerGroup;

            // Apply mode.
            switch (data.Mode)
            {
                case JEMAudioSpaceMode.ThreeDimensional:
                    source.spatialBlend = 1f;
                    source.rolloffMode = AudioRolloffMode.Logarithmic;
                    break;
                case JEMAudioSpaceMode.TwoDimensional:
                    source.spatialBlend = 1f;
                    source.rolloffMode = AudioRolloffMode.Linear;
                    break;
                case JEMAudioSpaceMode.UserInterface:
                    source.spatialBlend = 0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Apply pitch
            source.pitch = data.Pitch;

            // Apply min and max distance
            source.minDistance = data.MinDistance;
            source.maxDistance = data.MaxDistance;

            // Enable and play audio.
            source.enabled = true;
            source.Play();

            // Apply item data
            instance.Data = data;

            // Apply JEMAudioSource info
            instance.Instance.AudioSourceType = data.Type;
            instance.Instance.BaseVolume = data.Volume;

            // Update audio volume
            instance.Instance.UpdateAudioVolume();

            if (data.AutoDestroy >= 0.005f)
            { 
                // Run autoDestroy operation
                JEMOperation.InvokeAction(data.AutoDestroy, () =>
                {
                    if (item.IsValid() && source != null && source.gameObject != null)
                        Spawned.DestroyItem(item);
                });
            }
            else
            {
                // Hook onStop handler to know when this AudioInstance should be pooled
                instance.Instance.RegisterStopEvent(() => Spawned.PoolItem(item));
            }

            return instance;
        }

        /// <summary>
        ///     Called by the <see cref="GetAudioSourceVolume"/> to resolve audio source's volume for given <see cref="JEMAudioSourceType"/>.
        /// </summary>
        [Obsolete]
        public static event Func<JEMAudioSourceType, float, float> OnMixAudioVolume; 

        /// <summary>
        ///     Returns a count of all audio sources spawned by <see cref="JEMAudioManager"/> (a total value of items in pool).
        /// </summary>
        public static int AudioSourcesCount => Spawned.Pool.Count;

        /// <summary>
        ///     Returns a count of collected audio sources via <see cref="CollectAndDisableLevelAudioSources"/>.
        /// </summary>
        public static int CollectedAudioSourcesCount => CollectedAudioSources.Count;

        private static bool _wasReloaded;
        private static readonly JEMAudioPool Spawned = new JEMAudioPool();

        private static List<AudioSource> CollectedAudioSources { get; } = new List<AudioSource>();
    }
}
