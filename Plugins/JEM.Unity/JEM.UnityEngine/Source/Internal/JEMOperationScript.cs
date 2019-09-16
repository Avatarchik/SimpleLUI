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
    /// <summary />
    internal class JEMOperationScript : MonoBehaviour
    {
        /// <summary/>
        internal IEnumerator InternalInvokeAction(float sleep, Action targetAction)
        {
            yield return new WaitForSeconds(sleep);
            targetAction?.Invoke();
        }
    }
}