//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <inheritdoc />
    /// <summary>
    ///     Same as <see cref="SpaceAttribute" /> but supports <see cref="JEMMultiPropertyAttribute" />.
    /// </summary>
    public class JEMSpaceAttribute : JEMMultiPropertyAttribute
    {
        /// <summary>
        ///     Height of space in pixels.
        /// </summary>
        public int Height = 5;

        public JEMSpaceAttribute() { }
        public JEMSpaceAttribute(int height) => Height = height;

#if UNITY_EDITOR
        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, float currentHeight) => currentHeight + Height;

        /// <inheritdoc />
        public override void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            if (WillSkipThisFrame)
            {
                return;
            }

            position.y += Height;
        }

        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label) => false;
#endif
    }
}
