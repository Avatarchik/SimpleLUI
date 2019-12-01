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
    ///     A attribute that makes given field readonly in editor inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class JEMReadonlyAttribute : JEMMultiPropertyAttribute
    {
#if UNITY_EDITOR
        /// <inheritdoc />
        public override void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label) => GUI.enabled = false;

        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label) => false;

        /// <inheritdoc />
        public override void OnEndGUI(ref Rect position, SerializedProperty property, GUIContent label) => GUI.enabled = true;
#endif
    }
}