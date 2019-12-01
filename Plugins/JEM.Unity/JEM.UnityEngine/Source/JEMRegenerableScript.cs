//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Components;
using System;
using UnityEngine;

namespace JEM.UnityEngine
{
    /// <inheritdoc />
    /// <summary>
    ///     A base of every script that could be regenerated (if somehow lost).
    /// </summary>
    public abstract class JEMRegenerableScript<TScript> : JEMSingletonBehaviour<JEMRegenerableScript<TScript>> where TScript : JEMSingletonBehaviour<JEMRegenerableScript<TScript>>
    {
        /// <summary>
        ///     Regenerates the script if missing.
        /// </summary>
        public static void RegenerateScript()
        {
            if (Instance != null)
            {
                // Nothing to regenerate.
                return;
            }

            // Resolve root
            var root = ResolveScriptRoot();
            var scriptObject = new GameObject(typeof(TScript).Name);
            scriptObject.transform.SetParent(root.transform);
            Script = (TScript) scriptObject.AddComponent(typeof(TScript));
        }

        /// <summary>
        ///     Returns the reference to script of <see cref="TScript"/> type.
        ///     Creates new if missing.
        /// </summary>
        public static TScript GetScript()
        {
            if (Instance == null)
            {
                RegenerateScript();
            }

            if (Script == null)
                throw new NullReferenceException(nameof(Script));

            return Script;
        }

        /// <summary>
        ///     Resolves a Root <see cref="GameObject"/> that will never be destroyed.
        ///     Also helps to keep hierarchy clean.
        /// </summary>
        private static GameObject ResolveScriptRoot()
        {
            var parentName = "JEMRegenerableScripts";
            var parent = GameObject.Find(parentName);
            if (parent == null)
            {
                parent = new GameObject(parentName);
                parent.AddComponent<JEMObjectKeepOnScene>();
            }

            return parent;
        }

        /// <summary>
        ///     Reference to the generated script.
        /// </summary>
        public static TScript Script { get; private set; }
    }
}
