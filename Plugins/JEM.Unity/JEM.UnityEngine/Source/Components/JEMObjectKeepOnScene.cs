//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Simple script that prevents from gameObject destroy on scene load.
    /// </summary>
    [AddComponentMenu("JEM/Object/JEM KeepObjectOnScene")]
    [DisallowMultipleComponent]
    public sealed class JEMObjectKeepOnScene : MonoBehaviour
    {
        // yikes
        private void Awake() => DontDestroyOnLoad(gameObject);     
    }
}