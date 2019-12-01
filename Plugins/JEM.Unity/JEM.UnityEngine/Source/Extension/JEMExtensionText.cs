//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Extension.Internal;
using JetBrains.Annotations;
using System;
using UnityEngine.UI;

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     Set of utility methods: UI.Text
    /// </summary>
    public static class JEMExtensionText
    {
        /// <summary>
        ///     Insert given text char by char.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textToInsert">Text to insert.</param>
        /// <param name="speed">Speed of inserting.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static void InsertText([NotNull] this Text text, [NotNull] string textToInsert, float speed) => 
            InsertText(text, textToInsert, speed, null);
       
        /// <summary>
        ///     Insert given text char by char.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textToInsert">Text to insert.</param>
        /// <param name="speed">Speed of inserting.</param>
        /// <param name="onComplete"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static void InsertText([NotNull] this Text text, [NotNull] string textToInsert, float speed,
            Action onComplete)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (textToInsert == null) throw new ArgumentNullException(nameof(textToInsert));
            Script.StartCoroutine(Script.InternalInsertText(text, textToInsert, speed, onComplete));
        }

        private static JEMExtensionTextScript Script
        {
            get
            {
                if (_script == null)
                    _script = JEMExtensionTextScript.GetScript();

                return _script;
            }
        }

        private static JEMExtensionTextScript _script;
    }
}