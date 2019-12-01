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
    [AddComponentMenu("JEM/Object/JEM ObjectKeepOnScene")]
    [DisallowMultipleComponent]
    public sealed class JEMObjectKeepOnScene : MonoBehaviour
    {
        // yikes
        private void Awake()
        {
            if (gameObject.transform.parent != null)
            {
                Debug.LogWarning("DontDestroyOnLoad only works for root GameObjects or components on root GameObjects.", this);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}