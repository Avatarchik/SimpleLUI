//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace JEM.UnityEngine.Attribute
{
    /// <inheritdoc />
    /// <summary>
    ///     Attribute that allows you to select the scene asset and set target string with path.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class JEMSceneAssetAttribute : JEMMultiPropertyAttribute
    {
        /// <summary>
        ///     If true, instead of <see cref="ObjectField"/> the editor will
        ///     draw a <see cref="UnityEditor.EditorGUI.Popup"/> with scenes that are only added in to build settings.
        /// </summary>
        public bool OnlyFromBuildSettings;

#if UNITY_EDITOR
        /// <inheritdoc />
        public override bool OnGUI(ref Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var scenePath = property.stringValue + ".unity";
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (OnlyFromBuildSettings)
            {
                var sceneIndex = 0;
                var scenes = new GUIContent[EditorBuildSettings.scenes.Length];
                for (var index = 0; index < EditorBuildSettings.scenes.Length; index++)
                {
                    var scene = EditorBuildSettings.scenes[index];
                    scenes[index] = new GUIContent((scene.enabled ? "" : "(Disabled) ") + Path.GetFileNameWithoutExtension(scene.path));
                    if (scene.path.Replace("/", "\\") == scenePath)
                    {
                        sceneIndex = index;
                    }
                }

                var newIndex = EditorGUI.Popup(position, label, sceneIndex, scenes);
                if (EditorBuildSettings.scenes[newIndex].enabled)
                    sceneIndex = newIndex;

                sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[sceneIndex].path);
            }
            else
            {
                sceneAsset = (SceneAsset)EditorGUI.ObjectField(position, label, sceneAsset, typeof(SceneAsset), false);
            }

            if (sceneAsset == null)
                property.stringValue = string.Empty;
            else
            {
                scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                property.stringValue = $"{Path.GetDirectoryName(scenePath)}\\" +
                                       $"{Path.GetFileNameWithoutExtension(scenePath)}";
            }

            EditorGUI.EndProperty();

            return true;
        }
#endif
    }
}
