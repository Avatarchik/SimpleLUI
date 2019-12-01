//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Attribute;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(JEMMultiPropertyAttribute), true)]
    internal class JEMMultiPropertyDrawer : PropertyDrawer
    {
        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attributeHeight = base.GetPropertyHeight(property, label);
            if (!(attribute is JEMMultiPropertyAttribute multiAttribute))
                return attributeHeight;

            // Find all attributes and sort them by order.
            // NOTE: As GetPropertyHeight is called before onGUI, we should collect attributes here.
            if (multiAttribute.CachedAttributes == null)
            {
                multiAttribute.SetCachedAttributes(fieldInfo.GetCustomAttributes(typeof(JEMMultiPropertyAttribute), false)
                    .OrderBy(s => ((JEMMultiPropertyAttribute)s).order).ToList());
            }

            // Get height of attribute.
            for (var index = 0; index < multiAttribute.CachedAttributes.Count; index++)
            {
                var obj = multiAttribute.CachedAttributes[index];
                if (!(obj is JEMMultiPropertyAttribute a)) continue;
                attributeHeight = a.GetPropertyHeight(property, label, attributeHeight);
                if (a.WillSkipThisFrame)
                {
                    if (a.WasSkipFrameShared && !a.IsAbsolutePropertyHeight)
                    {
                        // This was an shared state.
                        // Return zero to not drawn this property.
                        return 0;
                    }

                    // Drawer is skipping this frame, we shall ignore the rest of attributes.
                    return attributeHeight;
                }
            }

            return attributeHeight;
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!(attribute is JEMMultiPropertyAttribute multiAttribute))
                return;

            // Begin GUI.
            bool drawFrame = true;
            bool skipFrameShared = false;
            for (var index = 0; index < multiAttribute.CachedAttributes.Count; index++)
            {
                var obj = multiAttribute.CachedAttributes[index];
                if (!(obj is JEMMultiPropertyAttribute a)) continue;
                a.OnBeginGUI(ref position, property, label);

                // Check if this frame can be drawn
                if (a.WillSkipThisFrame)
                {
                    drawFrame = false;
                }

                // Update WillSkipNextFrame state.
                a.SetWillSkipThisFrame(a.DontDrawNextFrame, false);
                if (a.WillSkipThisFrame && !skipFrameShared)
                {
                    // Share state.
                    for (var i = 0; i < multiAttribute.CachedAttributes.Count; i++)
                    {
                        var obj2 = multiAttribute.CachedAttributes[i];
                        if (!(obj2 is JEMMultiPropertyAttribute a2)) continue;
                        a2.SetWillSkipThisFrame(true, true);
                    }

                    skipFrameShared = true;
                }

                // Reset dontDrawNextFrame state.
                a.SetDontDrawNextFrame(false);
            }

            if (!drawFrame)
            {
                return;
            }

            // Draw GUI.
            bool hasCustomGUI = false;
            for (var index = 0; index < multiAttribute.CachedAttributes.Count; index++)
            {
                var obj = multiAttribute.CachedAttributes[index];
                if (!(obj is JEMMultiPropertyAttribute a)) continue;
                if (a.OnGUI(ref position, property, label))
                {
                    hasCustomGUI = true;
                }
            }

            if (!hasCustomGUI)
            {
                // No custom GUI implementation detected.
                // Draw default property.
                if (property.propertyType == SerializedPropertyType.Generic)
                    EditorGUI.HelpBox(position, $"Generic property of type {property.type} detected. " +
                                                "This type of properties is currently not supported by MultiProperty :/", MessageType.Error);
                else
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }

            // End GUI.
            for (var index = 0; index < multiAttribute.CachedAttributes.Count; index++)
            {
                var obj = multiAttribute.CachedAttributes[index];
                if (!(obj is JEMMultiPropertyAttribute a)) continue;
                a.OnEndGUI(ref position, property, label);
            }
        }
    }
}
