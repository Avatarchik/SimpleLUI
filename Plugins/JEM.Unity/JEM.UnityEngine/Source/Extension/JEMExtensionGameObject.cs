//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Extension.Internal;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     Set of utility extensions to GameObject class.
    /// </summary>
    public static class JEMExtensionGameObject
    {
        /// <summary>
        ///     Invoke operation on all children GameObjects.
        /// </summary>
        /// <param name="gameObject">Parent of GameObjects.</param>
        /// <param name="operation">Operation to invoke.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ChildOperation<T1>([NotNull] this GameObject gameObject, [NotNull] Action<T1> operation)
            where T1 : Component
        {
            if (gameObject == null) throw new ArgumentNullException(nameof(gameObject));
            if (operation == null) throw new ArgumentNullException(nameof(operation));

            var elements = gameObject.GetComponentsInChildren<T1>();
            foreach (var e in elements)
                operation.Invoke(e);
        }

        /// <summary>
        ///     Creates child GameObject.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public static GameObject CreateChild([NotNull] this GameObject go, [NotNull] string childName)
        {
            if (go == null) throw new ArgumentNullException(nameof(go));
            if (childName == null) throw new ArgumentNullException(nameof(childName));
            return JEMObject.Instantiate(new GameObject(childName), go);
        }

        /// <summary>
        ///     Activates/Deactivates the GameObject in background.
        /// </summary>
        /// <param name="go"/>
        /// <param name="activeState">Active state.</param>
        /// <param name="onDone">Event called at end of this background operation.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        public static void LiteSetActive([NotNull] this GameObject go, bool activeState, Action onDone = null)
        {
            if (go == null) throw new ArgumentNullException(nameof(go));
            if (go.activeSelf == activeState)
            {
                onDone?.Invoke();
                return;
            }

            Script.StartCoroutine(Script.InternalSetActiveEasy(go, activeState, onDone));
        }

        /// <summary>
        ///     Gets path from grand parent to this gameobject.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static string GetParentPath([NotNull] this GameObject go)
        {
            if (go == null) throw new ArgumentNullException(nameof(go));
            var path = string.Empty;
            var p = go.transform.parent;
            while (p != null)
            {
                path = $@"\{p.name}{path}";
                p = p.parent;
            }

            path = path + $@"\{go.name}";
            path = path.Remove(0, 1);
            return path;
        }

        /// <summary>
        ///     Gets or adds(if not exists) new component to the object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static T CollectComponent<T>([NotNull] this GameObject g) where T : Component
        {
            if (g == null) throw new ArgumentNullException(nameof(g));
            var i = g.GetComponent<T>();
            if (i == null)
                i = g.AddComponent<T>();

            return i;
        }

        /// <summary>
        ///     Gets or adds(if not exists) new component to the object.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static Component CollectComponent([NotNull] this GameObject g, [NotNull] Type t)
        {
            if (g == null) throw new ArgumentNullException(nameof(g));
            if (t == null) throw new ArgumentNullException(nameof(t));
            if (!t.IsSubclassOf(typeof(Component)))
                throw new ArgumentException($"Type {t.FullName} is not a subclass of Component.");

            var i = g.GetComponent(t);
            if (i == null)
                i = g.AddComponent(t);

            return i;
        }

        private static JEMExtensionGameObjectScript Script
        {
            get
            {
                if (_script == null)
                    _script = JEMExtensionGameObjectScript.GetScript();

                return _script;
            }
        }

        private static JEMExtensionGameObjectScript _script;
    }
}