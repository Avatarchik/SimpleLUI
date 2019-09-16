//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Internal;
using JetBrains.Annotations;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JEM.UnityEngine
{
    /// <summary>
    ///     Set of utility methods: Object.
    /// </summary>
    public static class JEMObject
    {
        /// <summary>
        ///     Removes object, component or asset in background.
        /// </summary>
        /// <param name="obj"/>
        /// <param name="onDone">Event called at end of this background operation.</param>
        /// <exception cref="NullReferenceException"/>
        public static void LiteDestroy(Object obj, Action onDone = null)
        {
            if (obj == null)
                return;

            RegenerateLocalScript();
            _script.StartCoroutine(_script.InternalLiteDestroy(obj, onDone));
        }

        /// <summary>
        ///     Clones the object original and returns the clone.
        /// </summary>
        /// <param name="original">An existing object that you want to make copy of.</param>
        /// <param name="position">Position of the new object.</param>
        /// <param name="orientation">Orientation of the new object.</param>
        /// <param name="onDone">Event called at end of this background operation.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        public static void LiteInstantiate<TObject>([NotNull] TObject original, Vector3 position, Quaternion orientation, Action<TObject> onDone = null)
            where TObject : Object
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            RegenerateLocalScript();
            _script.StartCoroutine(_script.InternalLiteInstantiate(original, position, orientation, onDone));
        }

        /// <summary>
        ///     Clones the object original and returns the clone as parent of given GameObject.
        ///     The new instance position/rotation/scale is always set to zero/identity/one.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="parent"></param>
        /// <param name="onDone">Event called at end of this background operation.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static void LiteInstantiate([NotNull] GameObject original, [NotNull] GameObject parent, Action<GameObject> onDone = null)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            LiteInstantiate(original, Vector3.zero, Quaternion.identity, go =>
            {
                go.transform.SetParent(parent.transform);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                onDone?.Invoke(go);
            });
        }


        /// <summary>
        ///     Clones the object original and returns the clone as parent of given transform.
        ///     The new instance position/rotation/scale is always set to zero/identity/one.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="parent"></param>
        /// <param name="onDone">Event called at end of this background operation.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public static void LiteInstantiate([NotNull] GameObject original, [NotNull] Transform parent, Action<GameObject> onDone = null)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            LiteInstantiate(original, Vector3.zero, Quaternion.identity, go =>
            {
                go.transform.SetParent(parent);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                onDone?.Invoke(go);
            });
        }

        /// <summary>
        ///     Clones the object original and returns the clone as parent of given transform.
        ///     The new instance position/rotation/scale is always set to zero/identity/one.
        /// </summary>
        /// <param name="original"/>
        /// <param name="parent"/>
        /// <param name="onDone">Event called at end of this background operation.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        public static void LiteInstantiate([NotNull] RectTransform original, [NotNull] Transform parent, Action<RectTransform> onDone = null)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            LiteInstantiate(original, Vector3.zero, Quaternion.identity, t =>
            {
                t.SetParent(parent);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                onDone?.Invoke(t);
            });
        }

        /// <summary>
        ///     Clones the object original and returns the clone as parent of given transform.
        ///     The new instance position/rotation/scale is always set to zero/identity/one.
        /// </summary>
        /// <param name="original"/>
        /// <param name="parent"/>
        /// <param name="onDone">Event called at end of this background operation.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        public static void LiteInstantiate([NotNull] Transform original, [NotNull] Transform parent,
            Action<Transform> onDone = null)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            LiteInstantiate(original, Vector3.zero, Quaternion.identity, o =>
            {
                var t = o as Transform;
                if (t == null)
                    throw new NullReferenceException(nameof(t));

                t.SetParent(parent);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                onDone?.Invoke(t);
            });
        }

        /// <summary>
        ///     Clones the object original and returns the clone as parent of given GameObject with local position/rotation set to zero.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        public static GameObject Instantiate([NotNull] GameObject original, [NotNull] GameObject parent)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            return Instantiate(original.transform, parent.transform).gameObject;
        }

        /// <summary>
        ///     Clones the object original and returns the clone as parent of given transform with local position/rotation set to zero.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        public static GameObject Instantiate([NotNull] GameObject original, [NotNull] Transform parent)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            return Instantiate(original.transform, parent).gameObject;
        }

        /// <summary>
        ///     Clones the object original and returns the clone as parent of given transform.
        ///     The new instance position/rotation/scale is always set to zero/identity/one.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        public static RectTransform Instantiate([NotNull] RectTransform original, [NotNull] Transform parent)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            return Instantiate((Transform) original, parent).GetComponent<RectTransform>();
        }

        /// <summary>
        ///     Clones the object original and returns the clone as parent of given transform with local position/rotation set to zero.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NullReferenceException"/>
        public static Transform Instantiate([NotNull] Transform original, [NotNull] Transform parent)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var t = Object.Instantiate(original, Vector3.zero, Quaternion.identity);
            if (t == null)
                throw new NullReferenceException(nameof(t));

            t.SetParent(parent);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            return t;
        }

        private static void RegenerateLocalScript()
        {
            if (_script != null)
                return;

            var obj = new GameObject(nameof(JEMObjectScript));
            Object.DontDestroyOnLoad(obj);
            _script = obj.AddComponent<JEMObjectScript>();

            if (_script == null)
                throw new NullReferenceException("System was unable to regenerate local script of " +
                                                 $"{nameof(JEMObject)}@{nameof(JEMObjectScript)}");
        }

        private static JEMObjectScript _script;
    }
}