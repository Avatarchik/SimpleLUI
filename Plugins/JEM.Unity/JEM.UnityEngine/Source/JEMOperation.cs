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
    ///     Set of utility methods: Coroutine Operations.
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
            Script.StartCoroutine(Script.InternalInvokeAction(sleep, targetAction));
        }

        /// <summary>
        ///     Starts given coroutine on local JEM script.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static Coroutine StartCoroutine([NotNull] IEnumerator routine)
        {
            if (routine == null) throw new ArgumentNullException(nameof(routine));
            return Script.StartCoroutine(routine);
        }

        /// <summary>
        ///     Stop given coroutine.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static void StopCoroutine([NotNull] Coroutine coroutine)
        {
            if (coroutine == null) throw new ArgumentNullException(nameof(coroutine));
            Script.StopCoroutine(coroutine);
        }

        private static JEMOperationScript Script
        {
            get
            {
                if (_script == null)
                    _script = JEMOperationScript.GetScript();

                return _script;
            }
        }

        private static JEMOperationScript _script;
    }
}