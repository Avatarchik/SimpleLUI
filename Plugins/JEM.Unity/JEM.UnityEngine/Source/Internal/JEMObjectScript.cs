//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JEM.UnityEngine.Internal
{
    /// <inheritdoc />
    internal class JEMObjectScript : JEMRegenerableScript<JEMObjectScript>
    {
        /// <inheritdoc />
        protected override void OnAwake()
        {
            // ignore
        }

        /// <summary/>
        internal IEnumerator InternalLiteDestroy(Object obj, Action onDone)
        {
            Destroy(obj);
            yield return new WaitForEndOfFrame();
            onDone?.Invoke();
        }

        /// <summary/>
        internal IEnumerator InternalLiteInstantiate<TObject>(Object original, Vector3 position, Quaternion orientation,
            Action<TObject> onDone) where TObject : Object
        {
            var obj = Instantiate(original, position, orientation);
            yield return obj;
            onDone?.Invoke((TObject) obj);
        }
    }
}