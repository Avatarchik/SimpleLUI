//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using JEM.Core.Debugging;
using JEM.UnityEngine.Audio.Interface;
using JEM.UnityEngine.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JEM.UnityEngine.Audio
{
    /// <inheritdoc />
    /// <summary>
    ///     A simple audio manager system.
    /// </summary>
    [AddComponentMenu("JEM/Systems/JEM Audio Manager")]
    [DisallowMultipleComponent]
    public class JEMAudioManager : MonoBehaviour
    {
        private class LocalSourceInstance
        {
            public AudioSource Source;
            public JEMAudioSourceType Type;
            public float BaseVolume;
        }

        [Header("References Settings")]
        public JEMAudioInterfaceDatabase AudioInterfaceDatabase;

        private void Awake()
        {
            if (Instance != null)
            {
                // Duplicate detested.
                gameObject.SetActive(false);
                return;
            }

            Instance = this;
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
            CollectedAudioSources.AddRange(Object.FindObjectsOfType<AudioSource>());
            foreach (var c in CollectedAudioSources)
            {
                c.enabled = false;
            }
        }

        /// <summary>
        ///     Releases all audio sources collected by <see cref="CollectAndDisableLevelAudioSources"/>.
        /// </summary>
        public static void ReleaseCollection()
        {
            foreach (var s in CollectedAudioSources)
            {
                if (s == null) continue;
                s.enabled = true;
            }

            CollectedAudioSources.Clear();
        }

        /// <summary>
        ///     It attempts to fix all audio listeners by destroying all except the one that is currently on Camera.main.
        /// </summary>
        public static void FixAudioListeners()
        {
            var audioListeners = Object.FindObjectsOfType<AudioListener>();
            var mainListener = JEMCameraHandler.AudioListenerReference;
            foreach (var a in audioListeners)
            {
                if (a == mainListener) continue;
                Object.Destroy(a);
            }
        }

        /// <summary>
        ///     Updates all currently registered game audio sources.
        /// </summary>
        public static void UpdateAudioSources(JEMKeyBasedDatabase settings = null)
        {
            // update custom audio sources
            foreach (var gameAudioSource in JEMCustomAudioSource.GameAudioSources)
                gameAudioSource.UpdateAudioSource(settings);

            // update local audio sources
            for (var index = 0; index < AudioSources.Count; index++)
            {
                var localSource = AudioSources[index];
                if (localSource == null || localSource.Source == null) // check if objects are missing and remove
                {
                    AudioSources.RemoveAt(index);
                    index--;
                    continue;
                }

                if (localSource.Source.isPlaying)
                {
                    localSource.Source.volume = GetAudioSourceVolume(localSource.BaseVolume, localSource.Type);
                }
            }
        }

        /// <summary>
        ///     Gets volume of audio source of given type.
        /// </summary>
        public static float GetAudioSourceVolume(float baseVolume, JEMAudioSourceType type, JEMKeyBasedDatabase settings = null)
        {
            var db = settings ?? JEMUnity.GetDatabase();
            if (db == null)
                return baseVolume;

            var mixedMaster = baseVolume * db.ResolveFromSystem<float>("audio_master");
            switch (type)
            {
                case JEMAudioSourceType.Unknown:
                    return mixedMaster;
                case JEMAudioSourceType.Interface:
                    return mixedMaster * db.ResolveFromSystem<float>("audio_interface");
                case JEMAudioSourceType.Effects:
                    return mixedMaster * db.ResolveFromSystem<float>("audio_effects");
                case JEMAudioSourceType.Music:
                    return mixedMaster * db.ResolveFromSystem<float>("audio_music");
                case JEMAudioSourceType.Ambient:
                    return mixedMaster * db.ResolveFromSystem<float>("audio_ambient");
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
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
            if (data.Clip == null) return new JEMAudioInstance(null, data);
            var instance = CreateAudioInstance(data);
            instance.Data.OnSpawned?.Invoke(instance);
            return instance;
        }

        private static IEnumerator SpawnDefaultWorker(JEMAudioData data)
        {
            var instance = CreateAudioInstance(data);
            yield return instance;
            instance.Data.OnSpawned?.Invoke(instance);
        }

        private static JEMAudioInstance CreateAudioInstance(JEMAudioData data)
        {
            LocalSourceInstance local;
            if (data.AutoDestroy <= 0.005f)
                local = GetOrCreateAudioSource();
            else
            {
                local = CreateNewAudioSource();
                JEMOperation.InvokeAction(data.AutoDestroy, () =>
                {
                    var s = local?.Source;
                    if (s != null && s.gameObject != null) JEMObject.LiteDestroy(s.gameObject);
                });
            }

            local.BaseVolume = data.Volume;
            local.Type = data.Type;
            var source = local.Source;
            if (data.Is2D)
            {
                if (data.Point == Vector3.zero)
                {
                    source.transform.SetParent(JEMCameraHandler.AudioListenerReference.transform);
                }
            }

            source.transform.localPosition = data.Point;
            source.clip = data.Clip;
            source.loop = false;
            source.spatialBlend = data.Is2D ? 0f : 1f;
            source.volume = GetAudioSourceVolume(data.Volume, data.Type);
            source.pitch = data.Pitch;
            source.minDistance = data.MinDistance;
            source.maxDistance = data.MaxDistance;
            source.enabled = true;
            source.Play();
            return new JEMAudioInstance(source, data);
        }

        /// <summary>
        ///     Gets free or creates new audio source.
        /// </summary>
        private static LocalSourceInstance GetOrCreateAudioSource()
        {
            LocalSourceInstance instance = null;
            for (var index = 0; index < AudioSources.Count; index++)
            {
                var audio = AudioSources[index];
                if (audio == null || audio.Source == null)
                {
                    AudioSources.RemoveAt(index);
                    index--;
                    continue;
                }

                if (audio.Source.clip != null && audio.Source.isPlaying) continue;
                instance = audio;
                break;
            }

            if (instance == null)
            {
                instance = CreateNewAudioSource();
                AudioSources.Add(instance);
            }

            return instance;
        }

        /// <summary>
        ///     Creates new audio source.
        /// </summary>
        private static LocalSourceInstance CreateNewAudioSource()
        {
            var obj = new GameObject("GameAudioSource");
            if (_parentGameObject == null)
            {
                _parentGameObject = new GameObject("GameAudioSources (Root)");
            }

            obj.transform.SetParent(_parentGameObject.transform);
            var instance = obj.AddComponent<AudioSource>();
            instance.loop = false;
            return new LocalSourceInstance {Source = instance, BaseVolume = 1f, Type = JEMAudioSourceType.Unknown};
        }

        /// <summary>
        ///     Registers change events of <see cref="JEMUnity.Database"/> so if any of the configuration objects related with audio changes,
        ///     all the audio sources will be automatically updated.
        /// </summary>
        public static void RegisterDatabaseChange()
        {
            void TryToReload()
            {
                if (_wasReloaded)
                    return;

                _wasReloaded = true;
                UpdateAudioSources();

                JEMOperation.InvokeAction(0.1f, () => { _wasReloaded = false; });
            }

            // Get the database.
            var database = JEMUnity.GetDatabase();
            if (database == null)
            {
                JEMLogger.LogWarning("Failed to register the database change for JEMAudioManager. Unable to load Database.", "JEM");
                return;
            }

            // Register the change events.
            JEMUnity.Database.RegisterObjectChange("audio_master_volume", obj => { TryToReload(); });
            JEMUnity.Database.RegisterObjectChange("audio_interface_volume", obj => { TryToReload(); });
            JEMUnity.Database.RegisterObjectChange("audio_effects_volume", obj => { TryToReload(); });
            JEMUnity.Database.RegisterObjectChange("audio_music_volume", obj => { TryToReload(); });
            JEMUnity.Database.RegisterObjectChange("audio_ambient_volume", obj => { TryToReload(); });
        }

        private static bool _wasReloaded;

        private static GameObject _parentGameObject;
        private static List<LocalSourceInstance> AudioSources { get; } = new List<LocalSourceInstance>();
        private static List<AudioSource> CollectedAudioSources { get; } = new List<AudioSource>();
        internal static JEMAudioManager Instance { get; private set; }

        /// <summary>
        ///     Returns a count of all audio sources spawned by <see cref="JEMAudioManager"/>.
        /// </summary>
        public static int AudioSourcesCount => AudioSources.Count;

        /// <summary>
        ///     Returns a count of collected audio sources via <see cref="CollectAndDisableLevelAudioSources"/>.
        /// </summary>
        public static int CollectedAudioSourcesCount => CollectedAudioSources.Count;
    }
}
