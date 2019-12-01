//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <inheritdoc />
    /// <summary>
    ///     Implements support for multiple property attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class JEMMultiPropertyAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public List<object> CachedAttributes { get; private set; }

        /// <summary>
        ///     When set to true, content of target property will not be drawn for next frame.
        /// </summary>
        /// <remarks>
        ///     Note that frame when this property was updated will be still drawn.
        /// </remarks>
        public bool DontDrawNextFrame { get; private set; }

        /// <summary>
        ///     Set to true when <see cref="DontDrawNextFrame"/> was true in last frame.
        ///     When true, GUI of property will not be drawn in active frame.
        /// </summary>
        /// <remarks>
        ///     <see cref="OnBeginGUI"/> will is called even if <see cref="WillSkipThisFrame"/> is true.
        /// </remarks>
        public bool WillSkipThisFrame { get; private set; }

        /// <summary>
        ///     True when <see cref="WillSkipThisFrame"/> was shared from other property drawer.
        /// </summary>
        public bool WasSkipFrameShared { get; private set; }

        /// <summary>
        ///     When true, this attribute <see cref="GetPropertyHeight"/> will be always included even if <see cref="WillSkipThisFrame"/> is true.
        /// </summary>
        public bool IsAbsolutePropertyHeight { get; private set; }

        public void SetCachedAttributes(List<object> cachedAttributes) => CachedAttributes = cachedAttributes;
        public void SetDontDrawNextFrame(bool dontDrawNextFrame) => DontDrawNextFrame = dontDrawNextFrame;
        public void SetWillSkipThisFrame(bool willSkipNextFrame, bool shared)
        {
            WillSkipThisFrame = willSkipNextFrame;
            WasSkipFrameShared = shared;
        }

        public void SetAbsolutePropertyHeight(bool absolutePropertyHeight) => IsAbsolutePropertyHeight = absolutePropertyHeight;

        public virtual float GetPropertyHeight(SerializedProperty property, GUIContent label, float currentHeight) =>
            currentHeight;

        public virtual void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label) { }
        public abstract bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label);
        public virtual void OnEndGUI(ref Rect position, SerializedProperty property, GUIContent label) { }

        /// <summary>
        ///     Gets int of float value from another property of serialized object of given property.
        /// </summary>
        internal static float GetPropertyValue(SerializedProperty property, string name)
        {
            var target = property.serializedObject.FindProperty(name);
            if (target == null)
                throw new NullReferenceException($"Property of target name '{name}' does not exists in serialized object.");

            switch (target.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return target.intValue;
                case SerializedPropertyType.Float:
                    return target.floatValue;
                default:
                    throw new InvalidOperationException("Type of target property is " +
                                                        $"invalid and not supported {target.propertyType}");
            }
        }

        /// <summary>
        ///     Returns unique key/path to the given property.
        /// </summary>
        internal static string GetPropertyKey(SerializedProperty property)
        {
            int componentIndex = GetPropertyComponentIndex(property);
            return property.serializedObject.targetObject.GetType().FullName + "." + property.propertyPath + "." +
                      componentIndex;
        }

        /// <summary>
        ///     Returns index of component given property is from.
        /// </summary>
        internal static int GetPropertyComponentIndex(SerializedProperty property)
        {
            int componentIndex = -1;
            if (property.serializedObject.targetObject is Component c)
            {
                var objs = c.GetComponents<Component>();
                for (var index = 0; index < objs.Length; index++)
                {
                    var obj = objs[index];
                    if (obj == c)
                    {
                        componentIndex = index;
                        break;
                    }
                }
            }

            return componentIndex;
        }

        ///// <summary>
        /////     Try to get unique property identity.
        ///// </summary>
        ///// <returns>Returns false when we failed to get static unique identity and GetInstanceID was returned instead.</returns>
        //internal static bool GetPropertyID([NotNull] SerializedProperty property, out int identity)
        //{
        //    if (property == null) throw new ArgumentNullException(nameof(property));
        //    var targetObject = property.serializedObject.targetObject;
        //    if (targetObject == null)
        //        throw new NullReferenceException(nameof(targetObject));

        //    identity = targetObject.GetInstanceID();
        //    if (targetObject is Component component)
        //    {

        //    }

        //    return false;
        //}

#endif
    }
}
