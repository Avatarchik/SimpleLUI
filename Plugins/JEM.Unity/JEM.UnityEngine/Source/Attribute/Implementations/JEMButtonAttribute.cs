//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using JEM.Core.Common;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <summary>
    ///     Defines there the <see cref="JEMButtonAttribute"/> should be drawn.
    /// </summary>
    public enum JEMButtonAttributePosition
    {
        /// <summary>
        ///     Button will be drawn to the left of property.
        /// </summary>
        Left,

        /// <summary>
        ///     Button will be drawn below property.
        /// </summary>
        Bottom
    }

    /// <inheritdoc />
    /// <summary>
    ///     An attribute that draws button with target property.
    ///     You can define button draw type via <see cref="JEMButtonAttributePosition" />.
    /// </summary>
    /// <remarks>
    ///     The button event is send via <see cref="Component.SendMessage"/> method.
    /// </remarks>
    public class JEMButtonAttribute : JEMMultiPropertyAttribute
    {
        /// <summary>
        ///     Position of the button.
        /// </summary>
        public JEMButtonAttributePosition Position { get; set; } = JEMButtonAttributePosition.Left;

        /// <summary>
        ///     Width of button when drawn to the left of property.
        /// </summary>
        public int LeftButtonWidth { get; set; } = 50;

        /// <summary>
        ///     Height of button when drawn to the bottom of property.
        /// </summary>
        public int BottomButtonHeight { get; set; } = 25;

        /// <summary>
        ///     Text of the button.
        /// </summary>
        public string Text { get; }

        /// <summary>
        ///     Name of the method button should trigger.
        /// </summary>
        public string MethodName { get; }

        public JEMButtonAttribute(string text, string methodName)
        {
            Text = text;
            MethodName = methodName;
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, float currentHeight)
        {
            switch (Position)
            {
                case JEMButtonAttributePosition.Left:
                    return currentHeight;
                case JEMButtonAttributePosition.Bottom:
                    return currentHeight + BottomButtonHeight + 5;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc />
        public override void OnBeginGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            if (WillSkipThisFrame)
            {
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            switch (Position)
            {
                case JEMButtonAttributePosition.Left:
                    position.width -= LeftButtonWidth + 5;

                    var buttonRect = new Rect(position)
                    {
                        width = LeftButtonWidth,
                        height = 18
                    };
                    buttonRect.x += position.width + 5;
                    if (GUI.Button(buttonRect, Text))
                    {
                        TriggerMethod(property);
                    }
                    break;
                case JEMButtonAttributePosition.Bottom:
                    buttonRect = new Rect(position)
                    {
                        x = EditorGUIUtility.labelWidth + 15,
                        width = position.width - EditorGUIUtility.labelWidth - 5,
                        height = BottomButtonHeight
                    };

                    buttonRect.y += BottomButtonHeight - 5;

                    if (GUI.Button(buttonRect, Text))
                    {
                        TriggerMethod(property);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUI.EndProperty();
        }

        private void TriggerMethod(SerializedProperty property)
        {
            var obj = property.serializedObject.targetObject;
            if (obj == null)
                throw new NullReferenceException("TargetObject of serializedObject is missing.");

            var monoBehaviour = obj as Component;
            if (monoBehaviour == null)
                throw new NotSupportedException($"TargetObject of serializedObject is not Component based ({obj.GetType().FullName}).");

            var method = new JEMSmartMethodS(monoBehaviour, MethodName);
            if (!method.IsValid())
                throw new NullReferenceException($"Method {MethodName} not exist in target type.");

            method.Invoke();
        }

        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label) => false;
#endif
    }
}
