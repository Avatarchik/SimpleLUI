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
    /// <summary>
    ///     And condition type used by <see cref="JEMPropertyInfoAttribute"/>.
    /// </summary>
    public enum JEMPropertyInfoCondition
    {
        /// <summary>
        ///     Info will be draw when property value is negative.
        /// </summary>
        NegativeValue
    }

    /// <inheritdoc />
    /// <summary>
    ///     An property attribute that will draw some info if target property math requested state.
    /// </summary>
    public class JEMPropertyInfoAttribute : JEMMultiPropertyAttribute
    {
        public JEMPropertyInfoCondition Condition { get; }
        public string Text { get; }
        public JEMMessageType MessageType { get; set; } = JEMMessageType.Info;

        public JEMPropertyInfoAttribute(JEMPropertyInfoCondition condition, string text)
        {
            Condition = condition;
            Text = text;
        }

#if UNITY_EDITOR
        private bool _draw;
        private bool ShouldDraw(SerializedProperty property)
        {
            switch (Condition)
            {
                case JEMPropertyInfoCondition.NegativeValue:
                    switch (property.propertyType)
                    {
                        case SerializedPropertyType.Integer:
                            return property.intValue < 0;
                        case SerializedPropertyType.Boolean:
                            return !property.boolValue;
                        case SerializedPropertyType.Float:
                            return property.floatValue < 0f;
                        case SerializedPropertyType.String:
                            return !string.IsNullOrEmpty(property.stringValue);
                        case SerializedPropertyType.ObjectReference:
                            return property.objectReferenceValue == null;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, float currentHeight)
        {
            _draw = ShouldDraw(property);

            return currentHeight;
        }

        /// <inheritdoc />
        public override void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_draw)
            {
                return;
            }

            var infoContent = new GUIContent(Text);
            var infoSize = EditorStyles.helpBox.CalcSize(infoContent);
            infoSize.x += 20; // Add 20px to make space for helpBox icon.
            position.width -= infoSize.x + 5;

            var infoRect = new Rect(position);
            infoRect.x += position.width + 5;
            infoRect.size = infoSize;

            // GUI.Box(infoRect, infoContent, EditorStyles.helpBox);
            EditorGUI.HelpBox(infoRect, Text, (MessageType) (int) MessageType);
        }

        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label) => false;
#endif
    }
}
