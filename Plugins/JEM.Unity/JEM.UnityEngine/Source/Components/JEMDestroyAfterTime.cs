//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Attribute;
using System.Collections;
using UnityEngine;

namespace JEM.UnityEngine.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Simple script that will destroy gameobject after given time.
    /// </summary>
    [AddComponentMenu("JEM/Object/JEM Destroy After Time")]
    [DisallowMultipleComponent]
    public class JEMDestroyAfterTime : MonoBehaviour
    {
        /// <summary>
        ///     Defines how much time in seconds system need to wait.
        /// </summary>
        [Header("Settings")]
        public float Time = 2;

        /// <summary>
        ///     When true, object will be disabled instead of destroying.
        /// </summary>
        public bool OnlyDisable = false;

        /// <summary>
        ///     Defines whether the default hard Object.Destroy method should be used.
        /// </summary>
        [JEMPropertyBased(nameof(OnlyDisable), InvertedCondition = true)]
        public bool HardDestroy;

        private void Start()
        {
            if (OnlyDisable) return;
            if (HardDestroy)
            {
                Destroy(gameObject, Time);
                enabled = false;
            }
            else
                StartCoroutine(FixedDestroy());
        }

        private void OnEnable()
        {
            if (!OnlyDisable) return;
            StartCoroutine(FixedDisable());
        }

        private IEnumerator FixedDestroy()
        {
            yield return new WaitForSeconds(Time);
            Destroy(gameObject);
            yield return new WaitForEndOfFrame();
        }

        private IEnumerator FixedDisable()
        {
            yield return new WaitForSeconds(Time);
            gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
        }
    }
}