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
    ///     Simple attribute that updates the <see cref="P:UnityEditor.EditorGUI.indentLevel" /> of next drawn property.
    /// </summary>
    public class JEMIndentLevelAttribute : JEMMultiPropertyAttribute
    {
        public int LevelAdd { get; set; } = 1;

        public JEMIndentLevelAttribute() { }
        public JEMIndentLevelAttribute(int levelAdd) => LevelAdd = levelAdd;
#if UNITY_EDITOR
        /// <inheritdoc />
        public override void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            if (WillSkipThisFrame)
            {
                return;
            }

            EditorGUI.indentLevel += LevelAdd;
        }

        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label) => false;

        /// <inheritdoc />
        public override void OnEndGUI(ref Rect position, SerializedProperty property, GUIContent label) =>
            EditorGUI.indentLevel -= LevelAdd;
#endif
    }
}
