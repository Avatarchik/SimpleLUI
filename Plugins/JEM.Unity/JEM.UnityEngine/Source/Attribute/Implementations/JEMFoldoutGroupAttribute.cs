//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <inheritdoc />
    /// <summary>
    ///     Attribute that defines the beginning of <see cref="EditorGUI.BeginFoldoutHeaderGroup"/>.
    /// </summary>
    public class JEMFoldoutBeginAttribute : JEMMultiPropertyAttribute
    {
        public string GroupName { get; }

        public JEMFoldoutBeginAttribute(string groupName)
        {
            GroupName = groupName;
#if UNITY_EDITOR
            order = -1;
            SetAbsolutePropertyHeight(true);
#endif
        }

#if UNITY_EDITOR

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, float currentHeight)
        {
            // Update draw state of active group
            GroupState = GetGroupDrawnState(property);

            // Return the height
            return (WillSkipThisFrame ? 0 : currentHeight) + 18;
        }

        /// <inheritdoc />
        public override void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw foldout header
            var propertyHeight = position.height - 18;
            position.height = 18;
            var newState = EditorGUI.BeginFoldoutHeaderGroup(position, GroupState, GroupName);

            // Apply new state.
            if (GroupState != newState)
            {
                SetGroupDrawnState(property, newState);
            }

            // Fix GUI position
            position.height = propertyHeight;

            // Draw by using previous group state to math property height.
            if (GroupState)
            {
                EditorGUI.indentLevel++;
                position.y += 18;
            }
            else SetDontDrawNextFrame(true);
        }

        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label) => false;

        /// <summary>
        ///     Checks if this group could be drawn.
        /// </summary>
        internal static bool GetGroupDrawnState(SerializedProperty property)
        {
            var key = GetPropertyKey(property);
            if (!IsGroupDrawnList.ContainsKey(key))
            {
                IsGroupDrawnList.Add(key, EditorPrefs.GetBool(key, false));
            }

            return IsGroupDrawnList[key];
        }

        /// <summary>
        ///     Sets the drawn state of group.
        /// </summary>
        internal static void SetGroupDrawnState(SerializedProperty property, bool newState)
        {
            var key = GetPropertyKey(property);

            // Apply new state.
            if (IsGroupDrawnList[key] == newState) return;
            EditorPrefs.SetBool(key, newState);
            IsGroupDrawnList[key] = newState;
        }

        /// <summary>
        ///     Defines draw state of currently active group.
        /// </summary>
        internal static bool GroupState { get; set; }
        internal static Dictionary<string, bool> IsGroupDrawnList { get; } = new Dictionary<string, bool>();
#endif
    }

    /// <inheritdoc />
    /// <summary>
    ///     Attribute that defines an item of foldout group.
    /// </summary>
    public class JEMFoldoutItemAttribute : JEMMultiPropertyAttribute
    {
#if UNITY_EDITOR
        public JEMFoldoutItemAttribute()
        {
            order = -1;
        }

        /// <inheritdoc />
        public override void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            if (JEMFoldoutBeginAttribute.GroupState)
            {
                EditorGUI.indentLevel++;
            }
            else SetDontDrawNextFrame(true);
        }

        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label) => false;
#endif
    }

    /// <inheritdoc />
    /// <summary>
    ///     Attribute that defines an end of foldout group.
    /// </summary>
    public class JEMFoldoutEndAttribute : JEMFoldoutItemAttribute
    {
#if UNITY_EDITOR
        public JEMFoldoutEndAttribute()
        {
            order = -1;
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, float currentHeight) => currentHeight + 5;

        /// <inheritdoc />
        public override void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            // Invoke base method
            base.OnBeginGUI(ref position, property, label);

            if (WillSkipThisFrame)
            {
                // As next frame will not be drawn,
                // We need to end foldout group here.
                EditorGUI.EndFoldoutHeaderGroup();
            }
        }

        /// <inheritdoc />
        public override void OnEndGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {  
            EditorGUI.indentLevel--;
            EditorGUI.EndFoldoutHeaderGroup();
        }
#endif
    }
}
