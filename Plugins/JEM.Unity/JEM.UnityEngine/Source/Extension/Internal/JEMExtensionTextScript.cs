//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.UnityEngine.Extension.Internal
{
    /// <inheritdoc />
    internal class JEMExtensionTextScript : JEMRegenerableScript<JEMExtensionTextScript>
    {
        /// <inheritdoc />
        protected override void OnAwake()
        {
            // ignore
        }

        /// <summary/>
        internal IEnumerator InternalInsertText(Text text, string textToInset, float speed, Action onComplete)
        {
            text.text = string.Empty;
            var i = 0;
            while (i < textToInset.Length && text != null)
            {
                text.text += textToInset[i];
                i++;
                yield return new WaitForSeconds(speed);
            }

            if (text != null)
            onComplete?.Invoke();
        }
    }
}