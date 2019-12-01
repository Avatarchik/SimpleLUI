//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine.Objects;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

namespace JEM.QNet.UnityEngine
{
    /// <inheritdoc />
    /// <summary>
    ///     QNet peer database.
    ///     Database contains all data that game's networking generates an uses.
    ///     Database also contains all QNetObjectPrefabs and Scripts that can be spawned by QNet.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [CreateAssetMenu(fileName = "newNetworkPrefabDatabase", menuName = "QNet/Network Prefab Database", order = 1)]
    public class QNetDatabase : ScriptableObject
    {
        /// <summary>
        ///     Reference to the default player prefab <see cref="QNetManager"/> should use.
        /// </summary>
        [Header("Prefabs Settings")]
        public QNetObjectPrefabObject PlayerPrefab;

        /// <summary>
        ///     All network prefabs of the game.
        /// </summary>
        [SerializeField]
        public QNetObjectPrefabObject[] Prefabs;

        /// <summary>
        ///     A copy of prefabs used only when a <see cref="RegisterNewPrefab(QNetIdentity)"/> was used.
        /// </summary>
        public List<QNetObjectPrefab> PrefabsCopy { get; } = new List<QNetObjectPrefab>();

        private bool PrefabLoaded;
        private void TryToLoadPrefabs()
        {
            if (PrefabLoaded)
            {
                return;
            }

            PrefabLoaded = true;
            RefreshPrefabs();
        }

        /// <summary>
        ///     Refresh list of prefabs by copying Prefabs in to PrefabsCopy list.
        /// </summary>
        public void RefreshPrefabs()
        {
            Profiler.BeginSample("QNetDatabase.RefreshPrefabs");
            for (var index = 0; index < Prefabs.Length; index++)
            {
                var p = Prefabs[index];
                if (GetPrefab(p.PrefabIdentity).IsValid)
                    continue;

                PrefabsCopy.Add(new QNetObjectPrefab
                {
                    Prefab = p.Prefab,
                    PrefabIdentity = p.PrefabIdentity,
                    IsValid = true
                });
            }

            Profiler.EndSample();
        }

        /// <summary>
        ///     Gets prefab of given identity.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public QNetObjectPrefab GetPrefab(ushort prefabIdentity)
        {
            Profiler.BeginSample("QNetDatabase.GetPrefab");
            TryToLoadPrefabs();
            for (var index = 0; index < PrefabsCopy.Count; index++)
            {
                var prefab = PrefabsCopy[index];
                if (prefab.IsValid && prefab.PrefabIdentity == prefabIdentity)
                {
                    Profiler.EndSample();
                    return prefab;
                }
            }

            Profiler.EndSample();
            return default;
        }

        /// <summary>
        ///     Registers a new prefab.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public QNetObjectPrefab RegisterNewPrefab([NotNull] QNetIdentity prefab) => RegisterNewPrefab(prefab, out _);

        /// <summary>
        ///     Registers a new prefab.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public QNetObjectPrefab RegisterNewPrefab([NotNull] QNetIdentity prefab, out ushort prefabIdentity)
        {
            prefabIdentity = GenerateUniqueIdentity();
            return RegisterNewPrefab(prefab, prefabIdentity);
        }

        /// <summary>
        ///     Registers a new prefab.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public QNetObjectPrefab RegisterNewPrefab([NotNull] QNetIdentity prefab, ushort prefabIdentity)
        {
            if (prefab == null) throw new ArgumentNullException(nameof(prefab));
            if (prefabIdentity == 0)
                throw new ArgumentOutOfRangeException(nameof(prefabIdentity), prefabIdentity,
                    "Prefab identity can't equals zero.");

            TryToLoadPrefabs();
            var newPrefab = new QNetObjectPrefab {Prefab = prefab, PrefabIdentity = prefabIdentity, IsValid = true};
            PrefabsCopy.Add(newPrefab);
            return newPrefab;
        }

        /// <summary>
        ///     It checks if the given identifier is in use and increases its value until it finds unused one.
        /// </summary>
        public ushort GenerateUniqueIdentity(ushort prefabIdentity)
        {
            while (GetPrefab(prefabIdentity).IsValid)
            {
                prefabIdentity++;
            }

            return prefabIdentity;
        }

        /// <summary>
        ///     Generates a unique identity for new prefab.
        /// </summary>
        public ushort GenerateUniqueIdentity()
        {
            ushort identity = 0;
            while (identity == 0 || GetPrefab(identity).IsValid)
            {
                identity = GetRandomUnsignedInt16();
            }

            return identity;
        }

        /// <summary>
        ///     Gets a pseudo random unsigned short value.
        /// </summary>
        public static ushort GetRandomUnsignedInt16() => (ushort)Random.Range(ushort.MinValue, ushort.MaxValue);
    }
}