//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Internal;
using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

namespace JEM.UnityEngine
{
    /// <summary>
    ///     Set of utility methods: Operations.
    /// </summary>
    public static class JEMOperation
    {
        /// <summary>
        ///     Wait given time and invoke target action.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static void InvokeAction(float sleep, [NotNull] Action targetAction)
        {
            if (targetAction == null) throw new ArgumentNullException(nameof(targetAction));

            RegenerateLocalScript();
            _script.StartCoroutine(_script.InternalInvokeAction(sleep, targetAction));
        }

        /// <summary>
        ///     Starts given coroutine on local JEM script.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static Coroutine StartCoroutine([NotNull] IEnumerator routine)
        {
            if (routine == null) throw new ArgumentNullException(nameof(routine));

            RegenerateLocalScript();
            return _script.StartCoroutine(routine);
        }

        /// <summary>
        ///     Stop given coroutine.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static void StopCoroutine([NotNull] Coroutine coroutine)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));

            RegenerateLocalScript();
            _script.StopCoroutine(coroutine);
        }

        internal static void RegenerateLocalScript()
        {
            if (_script != null)
                return;

            var obj = new GameObject(nameof(JEMOperationScript));
            global::UnityEngine.Object.DontDestroyOnLoad(obj);
            _script = obj.AddComponent<JEMOperationScript>();

            if (_script == null)
                throw new NullReferenceException(
                    $"System was unable to regenerate local script of {nameof(JEMObject)}@{nameof(JEMOperationScript)}");
        }

        private static JEMOperationScript _script;
    }
}