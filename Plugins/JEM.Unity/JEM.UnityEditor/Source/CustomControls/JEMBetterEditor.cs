//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// TODO: JEMBetterEditor need major refactor work.

using JetBrains.Annotations;
using System;
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor
{
    /// <summary>
    ///     Implements better or missing controls for unity editor custom windows/editors.
    /// </summary>
    public static partial class JEMBetterEditor
    {
        public static void DrawBetterEditorGroup(JEMBetterEditorStyle style, [NotNull] Action content,
            Action<Rect> onDrawMenu)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            string editorName = style.EditorName;
            bool draw = true;
            bool shouldDraw = true;
            string drawName = $"{GetPropertyName(style.DrawName)}";
            switch (style.FoldoutHeaderType)
            {
                case JEMBetterEditorHeaderType.HeaderGroup:
                    shouldDraw = EditorPrefs.GetBool($"{editorName}.Draw");
                    draw = EditorGUILayout.BeginFoldoutHeaderGroup(shouldDraw, drawName, menuAction: onDrawMenu);
                    break;
                case JEMBetterEditorHeaderType.Classic:
                    shouldDraw = EditorPrefs.GetBool($"{editorName}.Draw");
                    draw = EditorGUILayout.Foldout(shouldDraw, drawName);
                    break;
                case JEMBetterEditorHeaderType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (draw)
            {
                if (!string.IsNullOrEmpty(style.HeaderInfo))
                {
                    EditorGUILayout.HelpBox(style.HeaderInfo, MessageType.Info);
                }

                EditorGUILayout.BeginVertical();
                {
                    content.Invoke();
                }
                EditorGUILayout.EndVertical();
                if (style.FoldoutHeaderType != JEMBetterEditorHeaderType.None)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                }
            }

            if (style.FoldoutHeaderType == JEMBetterEditorHeaderType.HeaderGroup)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            switch (style.FoldoutHeaderType)
            {
                case JEMBetterEditorHeaderType.HeaderGroup:
                case JEMBetterEditorHeaderType.Classic:
                    if (draw != shouldDraw) EditorPrefs.SetBool($"{editorName}.Draw", draw);
                    break;
                case JEMBetterEditorHeaderType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Draws (fixed) property element.
        /// </summary>
        public static void DrawProperty([NotNull] string label, [NotNull] Action content)
        {
            if (label == null) throw new ArgumentNullException(nameof(label));
            if (content == null) throw new ArgumentNullException(nameof(content));
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(label);
                content.Invoke();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        /// <summary>
        ///     Draws (fixed) property element.
        /// </summary>
        public static void DrawProperty([NotNull] Action label, [NotNull] Action content)
        {
            if (label == null) throw new ArgumentNullException(nameof(label));
            if (content == null) throw new ArgumentNullException(nameof(content));
            EditorGUILayout.BeginHorizontal();
            {
                label.Invoke();
                content.Invoke();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        /// <summary>
        ///     Begins fixed indent group.
        /// </summary>
        public static void BeginFixedIndent(int indentLevel = 1, bool @space = false)
        {
            EditorGUILayout.BeginHorizontal();
            FixedIndentLevel += indentLevel;
            if (space)
                GUILayout.Space(FixedIndentLevel * 8);
            EditorGUI.indentLevel++;
            _fixedIndentGroups++;
        }

        /// <summary>
        ///     Ends fixed indent group.
        /// </summary>
        public static void EndFixedIndent(int indentLevel = 1)
        {
            if (_fixedIndentGroups <= 0)
            {
                EditorGUILayout.HelpBox("You are trying to end fixed indent without begin", MessageType.Error, true);
                return;
            }

            FixedIndentLevel -= indentLevel;
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
            _fixedIndentGroups--;
        }

        /// <summary>
        ///     Fixed indent level.
        /// </summary>
        public static int FixedIndentLevel
        {
            get => _fixedIndentLevel;
            private set
            {
                //EditorGUI.indentLevel = value;
                _fixedIndentLevel = value;
            }
        }

        private static int _fixedIndentLevel = 1;
        private static int _fixedIndentGroups;
    }
}