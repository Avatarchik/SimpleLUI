//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using UnityEngine;

namespace JEM.UnityEngine.Internal
{
    /// <inheritdoc />
    [AddComponentMenu("HIDDEN/_JEM _OPERATION _SCRIPT")]
    internal class JEMOperationScript : JEMRegenerableScript<JEMOperationScript>
    {
        /// <inheritdoc />
        protected override void OnAwake()
        {
            // Ignore
        }

        /// <summary/>
        internal IEnumerator InternalInvokeAction(float sleep, Action targetAction)
        {
            yield return new WaitForSeconds(sleep);
            targetAction?.Invoke();
        }
    }
}