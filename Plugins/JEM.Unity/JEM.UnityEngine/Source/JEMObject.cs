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

            Script.StartCoroutine(Script.InternalLiteDestroy(obj, onDone));
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
            Script.StartCoroutine(Script.InternalLiteInstantiate(original, position, orientation, onDone));
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

            LiteInstantiate(original, parent.transform, go =>
            {
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

            LiteInstantiate(original, parent, go =>
            {
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

            LiteInstantiate(original, parent, t =>
            {
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

            LiteInstantiate(original, parent, o =>
            {
                var t = o as Transform;
                if (t == null)
                    throw new NullReferenceException(nameof(t));

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

            var t = Object.Instantiate(original, parent);
            if (t == null)
                throw new NullReferenceException(nameof(t));

            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            return t;
        }

        private static JEMObjectScript Script
        {
            get
            {
                if (_script == null)
                    _script = JEMObjectScript.GetScript();

                return _script;
            }
        }

        private static JEMObjectScript _script;
    }
}