//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Attribute;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Objects
{
    /// <inheritdoc />
    /// <summary>
    ///     A simple object that contains reference to a QNetObject based prefab and it's network identity.
    /// </summary>
    [CreateAssetMenu(fileName = "newNetworkPrefab", menuName = "QNet/Network Prefab", order = 1)]
    public class QNetObjectPrefabObject : ScriptableObject
    {
        /// <summary>
        ///     Reference to the target prefab.
        /// </summary>
        public QNetIdentity Prefab;

        /// <summary>
        ///     Identity of this network prefab.
        /// </summary>
        [JEMReadonly]
        public ushort PrefabIdentity;
    }

    /// <summary>
    ///     A simple object that contains reference to a QNetObject based prefab and it's network identity.
    /// </summary>
    public struct QNetObjectPrefab
    {
        /// <summary>
        ///     Reference to the target prefab.
        /// </summary>
        public QNetIdentity Prefab;

        /// <summary>
        ///     Identity of this network prefab.
        /// </summary>
        public ushort PrefabIdentity;

        /// <summary>
        ///     Defines if this prefab struct is valid.
        /// </summary>
        public bool IsValid;

        public static implicit operator QNetObjectPrefab(QNetObjectPrefabObject obj) => new QNetObjectPrefab()
            {Prefab = obj.Prefab, PrefabIdentity = obj.PrefabIdentity};
    }
}