//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Extension;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <inheritdoc />
    /// <summary>
    ///     And property attribute implementation of <see cref="EditorGUI.MinMaxSlider"/>.
    /// </summary>
    public class JEMMinMaxRangeAttribute : JEMMultiPropertyAttribute
    {
        public float MinValue { get; }
        public float MaxValue { get; }

        public string MinProperty { get; }
        public string MaxProperty { get; }

        public JEMMinMaxRangeAttribute(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public JEMMinMaxRangeAttribute(string minProperty, string maxProperty)
        {
            MinProperty = minProperty;
            MaxProperty = maxProperty;
        }

#if UNITY_EDITOR
        public float GetMinValue(SerializedProperty property) =>
            string.IsNullOrEmpty(MinProperty) ? MinValue : GetPropertyValue(property, MinProperty);
        
        public float GetMaxValue(SerializedProperty property) =>
            string.IsNullOrEmpty(MaxProperty) ? MaxValue : GetPropertyValue(property, MaxProperty);
        
        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var minValue = GetMinValue(property);
            var maxValue = GetMaxValue(property);
            switch (property.propertyType)
            {
                case SerializedPropertyType.Vector2:
                    var floatValue = property.vector2Value;

                    // Clamp value
                    floatValue.x = Mathf.Clamp(floatValue.x, minValue, maxValue);
                    floatValue.y = Mathf.Clamp(floatValue.y, floatValue.x, maxValue);
   
                    // Draw slider
                    var sliderRect = new Rect(position)
                    {
                        width = position.width - 110
                    };

                    EditorGUI.MinMaxSlider(sliderRect, label, ref floatValue.x, ref floatValue.y, minValue, maxValue);

                    // Draw fields
                    var fieldRect = new Rect(position)
                    {
                        x = position.x + position.width - 105,
                        width = 50
                    };

                    // Store and reset indentLevel
                    var pevLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;

                    floatValue.x = Mathf.Round(floatValue.x * 100f) / 100f;
                    floatValue.x = EditorGUI.FloatField(fieldRect, floatValue.x);

                    fieldRect.x += 55;
                    floatValue.y = Mathf.Round(floatValue.y * 100f) / 100f;
                    floatValue.y = EditorGUI.FloatField(fieldRect, floatValue.y);

                    // Restore indentLevel
                    EditorGUI.indentLevel = pevLevel;

                    property.vector2Value = floatValue;
                    break;
                case SerializedPropertyType.Vector2Int:
                    floatValue = property.vector2IntValue;

                    // Clamp value
                    floatValue.x = Mathf.Clamp(floatValue.x, minValue, maxValue);
                    floatValue.y = Mathf.Clamp(floatValue.y, floatValue.x, maxValue);

                    // Draw slider
                    sliderRect = new Rect(position)
                    {
                        width = position.width - 110
                    };

                    EditorGUI.MinMaxSlider(sliderRect, label, ref floatValue.x, ref floatValue.y, (int) minValue, (int) maxValue);

                    // Draw fields
                    fieldRect = new Rect(position)
                    {
                        x = position.x + position.width - 105,
                        width = 50
                    };

                    // Store and reset indentLevel
                    pevLevel = EditorGUI.indentLevel;
                    EditorGUI.indentLevel = 0;

                    floatValue.x = EditorGUI.IntField(fieldRect, (int) floatValue.x);

                    fieldRect.x += 55;
                    floatValue.y = EditorGUI.IntField(fieldRect, (int) floatValue.y);

                    // Restore indentLevel
                    EditorGUI.indentLevel = pevLevel;

                    property.vector2IntValue = floatValue.ToInt();
                    break;
                default:
                    EditorGUI.HelpBox(position, $"Property type {property.propertyType} " +
                                                "is not supported by JEMRangeAttribute.", MessageType.Error);
                    break;
            }

            EditorGUI.EndProperty();

            return true;
        }
#endif
    }
}
