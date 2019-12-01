//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Common;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor.CustomEditors
{
    [CustomEditor(typeof(JEMDatabaseItem), true, isFallback = true)]
    public class JEMDatabaseItemEditor : Editor
    {
        private SavedBool _drawDatabaseItemContent;
        private SavedBool _drawDatabaseItemSettings;

        protected virtual void OnEnable()
        {
            _drawDatabaseItemContent = new SavedBool($"{GetType().Name}.DrawItemContent", false);
            _drawDatabaseItemSettings = new SavedBool($"{nameof(JEMDatabaseItem)}.DrawItemSettings", false);
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            _drawDatabaseItemContent.value = EditorGUILayout.BeginFoldoutHeaderGroup(_drawDatabaseItemContent.value,
                $"{JEMBetterEditor.GetPropertyName(target.GetType().Name)} Content");
            if (_drawDatabaseItemContent.value)
            {
                EditorGUI.indentLevel++;
                // invoke base method
                base.OnInspectorGUI();
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            // And draw the content...
            DrawDatabaseItemGUI();
        }

        protected void DrawDatabaseItemGUI()
        {
            var item = (JEMDatabaseItem) target;

            _drawDatabaseItemSettings.value =
                EditorGUILayout.BeginFoldoutHeaderGroup(_drawDatabaseItemSettings.value, "Database Item Settings");
            if (_drawDatabaseItemSettings.value)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Unique Identity", item.Identity.ToString());
                JEMBetterEditor.DrawProperty(" ", () =>
                {
                    if (GUILayout.Button("Regenerate"))
                    {
                        bool canRegenerate = true;
                        if (item.Identity != 0)
                        {
                            canRegenerate = EditorUtility.DisplayDialog("Regenerate?",
                                "Are you sure you want to regenerate the identity of this object? All references will be lost!",
                                "Yes", "No");
                        }

                        if (canRegenerate)
                            RegenerateIdentity();
                    }
                });

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            // Draw extras
            JEMBetterEditor.DrawTargetExtras(target);
        }

        /// <summary>
        ///     Regenerates the identity of this item.
        /// </summary>
        internal void RegenerateIdentity()
        {
            JEMDatabaseItem item = (JEMDatabaseItem) target;
            EditorUtility.SetDirty(target);

            string[] itemsGuiDs = AssetDatabase.FindAssets($"t:{nameof(JEMDatabaseItem)}");
            JEMDatabaseItem[] loadedItems = new JEMDatabaseItem[itemsGuiDs.Length];
            for (var index = 0; index < itemsGuiDs.Length; index++)
                loadedItems[index] =
                    (JEMDatabaseItem) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(itemsGuiDs[index]),
                        typeof(JEMDatabaseItem));

            ushort identity = 0;
            bool any = true;
            while (any)
            {
                identity = GetRandomUnsignedInt16();
                any = loadedItems.Any(p => p != null && p.Identity == identity);
            }

            item.Identity = identity;

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static ushort GetRandomUnsignedInt16() => (ushort) Random.Range(ushort.MinValue, ushort.MaxValue);
    }
}
