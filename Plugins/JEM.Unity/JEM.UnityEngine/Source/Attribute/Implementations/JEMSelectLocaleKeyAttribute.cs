//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Collections.Generic;
using JEM.Core.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <inheritdoc />
    /// <summary>
    ///     A property attribute that allows to select between keys of target locale group.
    /// </summary>
    public class JEMSelectLocaleKeyAttribute : JEMMultiPropertyAttribute
    {
        public string Group { get; }
        public string GroupProperty { get; set; }

        public JEMSelectLocaleKeyAttribute(string @group) => Group = @group;

#if UNITY_EDITOR
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, float currentHeight)
        {
            return currentHeight + 50 + EditorGUIUtility.standardVerticalSpacing;
        }

        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                // Supports only string type properties.
                return false;
            }

            var valueRect = new Rect(position);
            valueRect.width -= 60;
            valueRect.height -= 50;

            EditorGUI.Popup(valueRect, label, 0, new[] {new GUIContent("Hello World")});

            var refreshRect = new Rect(position);
            refreshRect.x += valueRect.width;
            refreshRect.width = 55;
            refreshRect.height -= 53;

            if (GUI.Button(refreshRect, "Refresh", EditorStyles.miniButton))
            {

            }

            var previewRect = new Rect(position);
            previewRect.y += (position.height - 50);
            previewRect.y += EditorGUIUtility.standardVerticalSpacing;
            previewRect.height = 50 - EditorGUIUtility.standardVerticalSpacing;

            GUI.enabled = false;
            EditorGUI.TextField(previewRect, new GUIContent(label.text + " (Prev)"), "Hello, Preview!");
            GUI.enabled = true;

            return true;
        }

        public static void TryRefreshKeys(string group)
        {
            if (_wasRefreshed) return;
            _wasRefreshed = true;
            RefreshKeys(group);
        }

        public static void RefreshKeys(string group)
        {
            // TODO: Add default locale to the JEM configuration window.

            // Load and set the active locale.
            JEMLocale.LoadLocale("ENG", "Locale/eng", true);
            JEMLocale.SetLocale("ENG");

            var locale = JEMLocale.GetSelectedLocale();

            KeysList = new List<GUIContent>(KeysArray);
        }

        public static GUIContent[] KeysArray { get; set; } = new GUIContent[0];
        public static List<GUIContent> KeysList { get; set; } = new List<GUIContent>();

        private static bool _wasRefreshed;
#endif
    }
}
