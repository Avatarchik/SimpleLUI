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
    ///     A property attribute that allows to select between all exported locale groups.
    /// </summary>
    public class JEMSelectLocaleGroupAttribute : JEMMultiPropertyAttribute
    {
#if UNITY_EDITOR
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

            TryRefreshGroups();
            var index = GroupsList.FindIndex(g => g.text == property.stringValue);
            if (index < 0 && GroupsList.Count != 0)
                index = 0;

            index = EditorGUI.Popup(valueRect, label, index, GroupsArray);
            property.stringValue = index < 0 ? string.Empty : GroupsArray[index].text;

            var refreshRect = new Rect(position);
            refreshRect.x += valueRect.width;
            refreshRect.width = 55;
            refreshRect.height -= 1;

            if (GUI.Button(refreshRect, "Refresh", EditorStyles.miniButton))
            {
                RefreshGroups();
            }

            return true;
        }

        public static void TryRefreshGroups()
        {
            if (_wasRefreshed) return;
            _wasRefreshed = true;
            RefreshGroups();
        }

        public static void RefreshGroups()
        {
            // TODO: Add default locale to the JEM configuration window.

            // Load and set the active locale.
            JEMLocale.LoadLocale("ENG", "Locale/eng", true);
            JEMLocale.SetLocale("ENG");

            var locale = JEMLocale.GetSelectedLocale();
            GroupsArray = new GUIContent[locale.Groups.Count];
            for (int index = 0; index < GroupsArray.Length; index++)
            {
                GroupsArray[index] = new GUIContent(locale.Groups[index].GroupName);
            }

            GroupsList = new List<GUIContent>(GroupsArray);
        }

        public static GUIContent[] GroupsArray { get; set; } = new GUIContent[0];
        public static List<GUIContent> GroupsList { get; set; } = new List<GUIContent>();

        private static bool _wasRefreshed;
#endif
    }
}
