//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine.Objects;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

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
        ///     Prefab of player.
        /// </summary>
        [Header("Prefabs Settings")]
        public QNetObjectPrefabPair PlayerPrefab;

        /// <summary>
        ///     All network prefabs of the game.
        /// </summary>
        [SerializeField]
        public QNetObjectPrefabPair[] Prefabs;

        /// <summary>
        ///     Gets prefab of given identity.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public QNetObjectPrefabPair GetPrefab(ushort prefabIdentity)
        {
            if (prefabIdentity == 0)
                throw new ArgumentOutOfRangeException(nameof(prefabIdentity), prefabIdentity, "Prefab identity can't equals zero.");
            return Prefabs.FirstOrDefault(prefab => prefab != null && prefab.PrefabIdentity == prefabIdentity);
        }
    }
}