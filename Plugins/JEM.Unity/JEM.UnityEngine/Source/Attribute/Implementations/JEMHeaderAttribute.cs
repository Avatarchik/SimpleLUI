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
    ///     Same as <see cref="HeaderAttribute" /> but supports <see cref="JEMMultiPropertyAttribute" />.
    /// </summary>
    public class JEMHeaderAttribute : JEMMultiPropertyAttribute
    {
        /// <summary>
        ///     Text of the header.
        /// </summary>
        public string Text { get; }

        public JEMHeaderAttribute(string text)
        {
            Text = text;
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, float currentHeight) => currentHeight + 26;

        /// <inheritdoc />
        public override void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            if (WillSkipThisFrame)
            {
                return;
            }

            // BUG: 2019.3+ returns weird height for some properties, so we need to find better solution than code below.

            var headerStyle = EditorStyles.boldLabel;
            var headerSize = headerStyle.CalcHeight(new GUIContent(Text), position.width);
            var headerRect = new Rect(position);
            headerRect.y += 10;
            headerRect.height = headerSize;
            headerRect = EditorGUI.IndentedRect(headerRect);
            GUI.Label(headerRect, Text, EditorStyles.boldLabel);

            position = new Rect(position.x, position.y + headerRect.height * 2 - 5, position.width, position.height - headerRect.height - 10);
        }

        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label) => false;
#endif
    }
}
