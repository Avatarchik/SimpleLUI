//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine.Objects;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JEM.QNet.UnityEditor.Behaviour
{
    [CustomEditor(typeof(QNetObjectPrefabPair))]
    internal class QNetObjectPrefabPairEditor : Editor
    {
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            // invoke base method
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            if (GUILayout.Button("Regenerate Identity", GUILayout.Height(25)))
            {
                EditorUtility.SetDirty(target);
                var prefab = (QNetObjectPrefabPair) target;

                var prefabsGUIDs = AssetDatabase.FindAssets($"t:{nameof(QNetObjectPrefabPair)}");
                var loadedPrefabs = new QNetObjectPrefabPair[prefabsGUIDs.Length];
                for (var index = 0; index < prefabsGUIDs.Length; index++)
                    loadedPrefabs[index] =
                        (QNetObjectPrefabPair) AssetDatabase.LoadAssetAtPath(
                            AssetDatabase.GUIDToAssetPath(prefabsGUIDs[index]), typeof(QNetObjectPrefabPair));

                var identity = (ushort) Random.Range(ushort.MinValue, ushort.MaxValue);
                while (loadedPrefabs.Any(p => p.PrefabIdentity == identity))
                    identity = (ushort) Random.Range(ushort.MinValue, ushort.MaxValue);

                prefab.PrefabIdentity = identity;

                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
        }
    }
}