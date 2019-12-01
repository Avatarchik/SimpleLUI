//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Components.Internal;
using System.Collections;
using UnityEngine;

#pragma warning disable 1591

namespace JEM.UnityEngine.Components
{
    /// <inheritdoc />
    /// <summary>
    ///     Simple script that translates Target from StartPoint to EndPoint.
    /// </summary>
    [AddComponentMenu("JEM/Object/JEM Translator")]
    [DisallowMultipleComponent]
    public class JEMTranslator : MonoBehaviour
    {
        [Header("Settings")]
        public Transform StartPoint;
        public Transform EndPoint;
        public float Speed = 5f;

        public bool State { get; private set; }

        private Transform _transform;
        private Coroutine _slave;

        private void OnDisable()
        {
            if (_transform == null)
                _transform = GetComponent<Transform>();

            // script or gameobject has been disabled
            // force new state
            _transform.position = State ? EndPoint.position : StartPoint.position;
        }

        /// <summary>
        /// Sets state of translator.
        /// </summary>
        public void SetState(bool state)
        {
            if (State == state)
                return;

            State = state;
            if (_slave != null)
                StopCoroutine(_slave);
            _slave = StartCoroutine(Slave(state));
        }

        /// <summary>
        ///     Force current state.
        /// </summary>
        public void ForceState()
        {
            if (_slave != null)
                StopCoroutine(_slave);

            _transform.position = State ? EndPoint.position : StartPoint.position;
        }

        // TODO: Move coroutine to JEMTranslatorScript
        private IEnumerator Slave(bool activeState)
        {
            if (_transform == null)
                _transform = GetComponent<Transform>();

            if (StartPoint != null && EndPoint != null)
            {
                var tPoint = activeState ? EndPoint : StartPoint;
                while (Vector3.Distance(_transform.position, tPoint.position) > 0.1f)
                {
                    var t = Time.deltaTime * Speed;
                    _transform.position = Vector3.Lerp(_transform.position, tPoint.position, t);
                    yield return new WaitForEndOfFrame();
                }
            }
            _slave = null;
        }

        private static JEMTranslatorScript Script
        {
            get
            {
                if (_script == null)
                    _script = JEMTranslatorScript.GetScript();

                return _script;
            }
        }

        private static JEMTranslatorScript _script;
    }
}
