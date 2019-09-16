//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine;
using JEM.QNet.UnityEngine.Objects;
using UnityEditor;
using UnityEngine;

namespace JEM.QNet.UnityEditor
{
    [CustomEditor(typeof(QNetDatabase))]
    internal class QNetDatabaseEditor : Editor
    {
        internal SerializedProperty PlayerPrefab;
        internal SerializedProperty Prefabs;

        protected QNetDatabase Script;

        protected virtual void OnEnable()
        {
            PlayerPrefab = serializedObject.FindProperty(nameof(PlayerPrefab));
            Prefabs = serializedObject.FindProperty(nameof(Prefabs));

            InternalUpdateScript();
        }

        protected virtual bool InternalReady()
        {
            return Script != null;
        }

        protected virtual void InternalUpdateScript()
        {
            Script = target as QNetDatabase;
        }

        public override void OnInspectorGUI()
        {
            if (!InternalReady())
            {
                InternalUpdateScript();
                EditorGUILayout.HelpBox($"Unable to draw {nameof(QNetDatabase)} inspector gui. Target script is null.",
                    MessageType.Error);
                return;
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(PlayerPrefab, true);
            EditorGUILayout.PropertyField(Prefabs, true);

            EditorGUILayout.Space();
            if (GUILayout.Button("Refresh Prefabs", GUILayout.Height(25)))
            {
                EditorUtility.SetDirty(target);
                var prefabsGUIDs = AssetDatabase.FindAssets($"t:{nameof(QNetObjectPrefabPair)}");
                var loadedPrefabs = new QNetObjectPrefabPair[prefabsGUIDs.Length];
                for (var index = 0; index < prefabsGUIDs.Length; index++)
                    loadedPrefabs[index] =
                        (QNetObjectPrefabPair) AssetDatabase.LoadAssetAtPath(
                            AssetDatabase.GUIDToAssetPath(prefabsGUIDs[index]), typeof(QNetObjectPrefabPair));
                Script.Prefabs = loadedPrefabs;
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}