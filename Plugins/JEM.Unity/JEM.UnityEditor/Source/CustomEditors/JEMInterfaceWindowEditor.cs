﻿//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Interface.Window;
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor.CustomEditors
{
    [CustomEditor(typeof(JEMInterfaceWindow))]
    internal sealed class JEMInterfaceWindowEditor : Editor
    {
        private JEMInterfaceWindow _script;

        private void OnEnable() => _script = target as JEMInterfaceWindow;
        
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            // Do base GUI.
            base.OnInspectorGUI();

            if (_script.WindowTransform == null)
                return;

            if (_script.WindowTransform.pivot != new Vector2(0.5f, 0.5f))
            {
                EditorGUILayout.HelpBox("It's looks like you are using other pivot than (0.5f, 0.5f). " +
                                        "In current version, InterfaceWindow.UpdateDisplay " +
                                        "may not work properly.", MessageType.Error, true);

                if (GUILayout.Button("Reset"))
                {
                    _script.WindowTransform.pivot = new Vector2(0.5f, 0.5f);
                }
            }
        }
    }
}