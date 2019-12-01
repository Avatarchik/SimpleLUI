//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <inheritdoc />
    /// <summary>
    ///     A property attribute that will disable or hide property when target property is disabled or it's value is not set.
    /// </summary>
    public class JEMPropertyBasedAttribute : JEMMultiPropertyAttribute
    {
        /// <summary>
        ///     Name of property this attribute should look for.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        ///     When set to true, this property will be drawn when target is missing.
        /// </summary>
        public bool InvertedCondition { get; set; }

        public JEMPropertyBasedAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            // Resolve target property path.
            var fixedPath = Path.GetDirectoryName(property.propertyPath.Replace('.', '/'))?.Replace('\\', '.');
            string propPath;
            if (string.IsNullOrEmpty(fixedPath) || string.IsNullOrWhiteSpace(fixedPath))
                propPath = PropertyName;
            else
            {
                propPath = fixedPath + "." + PropertyName;
            }

            // Get target property.
            var target = property.serializedObject.FindProperty(propPath);
            if (target == null)
                throw new NullReferenceException($"Property of name {propPath} does not exist in serializedObject.");

            // Resolve key.
            int componentIndex = GetPropertyComponentIndex(property);
            var key = property.serializedObject.targetObject.GetType() + property.propertyPath + "." + componentIndex;
            if (!PropertyActiveStates.ContainsKey(key))
            {
                PropertyActiveStates.Add(key, false);
            }

            // Resolve active state.
            bool isActive;
            var targetKey = target.serializedObject.targetObject.GetType() + target.propertyPath + "." + componentIndex;
            if (PropertyActiveStates.ContainsKey(targetKey) && !PropertyActiveStates[targetKey])
                isActive = false;
            else
            {
                switch (target.propertyType)
                {
                    case SerializedPropertyType.Boolean:
                        isActive = target.boolValue;
                        break;
                    case SerializedPropertyType.ObjectReference:
                        isActive = target.objectReferenceValue != null;
                        break;
                    case SerializedPropertyType.ExposedReference:
                        isActive = target.exposedReferenceValue != null;
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Target property type {target.propertyType} is not supported.");
                }
            }

            // Cache active state.
            PropertyActiveStates[key] = isActive;

            // Apply active state.
            if (InvertedCondition)
            {
                if (isActive)
                {
                    SetDontDrawNextFrame(true);
                }
            }
            else
            {
                if (!isActive)
                {
                    SetDontDrawNextFrame(true);
                }
            }
        }

        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label) => false;

        private static Dictionary<string, bool> PropertyActiveStates { get; } = new Dictionary<string, bool>();
#endif
    }
}
