//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using UnityEngine;

namespace JEM.UnityEngine.VersionManagement
{
    /// <summary>
    ///     Build version data.
    /// </summary>
    [Serializable]
    public class JEMBuildVersion
    {
        /// <summary>
        ///     Version name.
        /// </summary>
        [SerializeField]
        public string VersionName = "v0.01a";

        /// <summary>
        ///     Version release date.
        /// </summary>
        [SerializeField]
        public DateTime VersionRelease;
    }
}