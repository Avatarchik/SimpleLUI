//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <inheritdoc />
    /// <summary>
    ///     Same as <see cref="UnityEventDrawer" /> but supported <see cref="JEMMultiPropertyAttribute" />.
    /// </summary>
    public class JEMUnityEventDrawer : JEMMultiPropertyAttribute
    {
#if UNITY_EDITOR
        public UnityEventDrawer Drawer { get; }
        public JEMUnityEventDrawer() => Drawer = new UnityEventDrawer();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label, float currentHeight) => 
            currentHeight + Drawer.GetPropertyHeight(property, label);

        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            Drawer.OnGUI(position, property, label);
            return true;
        }
#endif
    }
}
