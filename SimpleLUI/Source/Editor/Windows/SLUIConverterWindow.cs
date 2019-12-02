//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEditor;
using System;
using UnityEditor;
using UnityEngine;

namespace SimpleLUI.Editor.Windows
{
    public class SLUIConverterWindow : EditorWindow
    {
        private Canvas Canvas;
        private SavedString File;
        private SavedBool PrettyPrint;

        private void OnEnable()
        {
            File = new SavedString($"{GetType()}.File", string.Empty);
            PrettyPrint = new SavedBool($"{GetType()}.PrettyPrint", true);
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Generate SLUI script file from selected Canvas. (Custom Sprite export included)", MessageType.Info);

            Canvas = (Canvas) EditorGUILayout.ObjectField("Canvas (Root)", Canvas, typeof(Canvas), true);

            EditorGUILayout.BeginHorizontal();
            {
                GUI.enabled = false;
                File.value = EditorGUILayout.TextField(File.value);
                GUI.enabled = true;
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    var file = EditorUtility.SaveFilePanel("Save Script", $"{Environment.CurrentDirectory}\\LUI", "lui_script", "lua");
                    if (!string.IsNullOrEmpty(file))
                        File.value = file;
                }
            }
            EditorGUILayout.EndHorizontal();

            PrettyPrint.value = EditorGUILayout.Toggle("Pretty Print", PrettyPrint.value);

            if (GUILayout.Button("Convert"))
            {
                if (string.IsNullOrEmpty(File.value))
                    File.value = EditorUtility.SaveFilePanel("Save Script", $"{Environment.CurrentDirectory}\\LUI", "lui_script", "lua");

                if (!string.IsNullOrEmpty(File.value))
                {
                    SLUIEngineToScriptConverter.Convert(Canvas, File.value, PrettyPrint.value);
                }
            }
        }

        [MenuItem("SLUI/SLUI Converter")]
        internal static void ShowConverter()
        {
            GetWindow<SLUIConverterWindow>(true, "UI To Lua Convert", true);
        }
    }
}
