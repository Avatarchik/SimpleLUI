//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <inheritdoc />
    /// <summary>
    ///     Attribute that will draw a EnumFlag editor for target Enum field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class JEMEnumFlagsAttribute : JEMMultiPropertyAttribute
    {
#if UNITY_EDITOR
        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumDisplayNames);
            EditorGUI.EndProperty();
            return true;
        }
#endif
    }
}
