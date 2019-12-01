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
    ///     Same as <see cref="RangeAttribute" /> but supports <see cref="JEMMultiPropertyAttribute" />.
    /// </summary>
    public class JEMRangeAttribute : JEMMultiPropertyAttribute
    {
        public float MinValue { get; }
        public float MaxValue { get; }

        public string MinProperty { get; }
        public string MaxProperty { get; }

        public JEMRangeAttribute(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public JEMRangeAttribute(string minProperty, string maxProperty)
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
                case SerializedPropertyType.Integer:
                    property.intValue = EditorGUI.IntSlider(position, label, property.intValue, (int) minValue, (int) maxValue);
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = EditorGUI.Slider(position, label, property.floatValue, minValue, maxValue);
                    break;
                default:
                    EditorGUI.HelpBox(position, $"Property type {property.propertyType} is not supported by JEMRangeAttribute.", MessageType.Error);
                    break;
            }
            EditorGUI.EndProperty();

            return true;
        }
#endif
    }
}
