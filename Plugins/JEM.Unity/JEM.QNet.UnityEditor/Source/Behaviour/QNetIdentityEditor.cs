//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine.Objects;
using JEM.UnityEditor;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JEM.QNet.UnityEditor.Behaviour
{
    [CustomEditor(typeof(QNetIdentity), true)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class QNetIdentityEditor : Editor
    {
        private SavedBool _drawBase;
        private SavedBool _drawObjects;

        private QNetIdentity _target;

        private SerializedProperty CustomIdentity;

        private bool _work;

        private void OnEnable()
        {       
            _drawBase = new SavedBool($"{nameof(QNetIdentity)}.DrawBase", false);
            _drawObjects = new SavedBool($"{nameof(QNetIdentityEditor)}.DrawObjects", false);

            _target = (QNetIdentity) target;

            // Get properties.
            CustomIdentity = serializedObject.FindProperty(nameof(CustomIdentity));

            // Check if the object is on scene or in assets.
            CheckIfCanWork();
        }

        /// <summary>
        ///     Checks if script can currently work.
        /// </summary>
        /// <remarks>
        ///     Component should only work if on scene.
        /// </remarks>
        private void CheckIfCanWork()
        {
            // TODO
            // var assetType = PrefabUtility.GetPrefabAssetType(target);
            // var instanceStatus = PrefabUtility.GetPrefabInstanceStatus(target);

            var type = PrefabUtility.GetPrefabType(target);
            _work = type == PrefabType.None || type == PrefabType.PrefabInstance ||
                    type == PrefabType.MissingPrefabInstance || type == PrefabType.ModelPrefabInstance ||
                    type == PrefabType.DisconnectedPrefabInstance || type == PrefabType.DisconnectedModelPrefabInstance;

            if (!_work)
            {
                _target.CustomIdentity = 0;
            }
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            // Draw base inspector GUI.
            base.OnInspectorGUI();

            if (!_work)
            {
                EditorGUILayout.HelpBox("QNetObject preview work only for objects in scene.", MessageType.Info, true);
                return;
            }

            serializedObject.Update();

            var hasPrefab = _target.Prefab != null || _target.PrefabIdentity != 0;
            if (!hasPrefab)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Predefined Object", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(CustomIdentity, new GUIContent("Identity"));
                JEMBetterEditor.DrawProperty(" ", () =>
                {
                    if (EditorApplication.isPlaying)
                        GUI.enabled = false;
                    if (GUILayout.Button("Regenerate"))
                    {
                        CustomIdentity.intValue = FixPredefinedIdentity(CustomIdentity.intValue);
                    }
                    GUI.enabled = true;
                });

                if (_target.CustomIdentity == 0)
                {
                    EditorGUILayout.HelpBox("Custom Identity need to be regenerated!", MessageType.Warning, true);
                }

                EditorGUILayout.EndVertical();
            }

            if (EditorApplication.isPlaying)
            {
                // Draw base GUI.
                DrawBaseGUI();

                // Draw objects GUI.
                DrawObjectsGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBaseGUI()
        {
            _drawBase.value = EditorGUILayout.BeginFoldoutHeaderGroup(_drawBase.value, "Base Info");
            if (_drawBase.value)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Spawned Object", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Identity", _target.Identity.ToString());
                EditorGUILayout.ObjectField("Prefab", _target.Prefab, typeof(QNetIdentity), false);
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Prefab (Identity)", _target.PrefabIdentity.ToString());
                EditorGUI.indentLevel--;
                if (!_target.HasOwner)
                    EditorGUILayout.LabelField("Owner", "N\\A");
                else
                {
                    EditorGUILayout.LabelField("Owner", _target.Owner.ToString());
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("Owner (Name)", _target.OwnerConnection.ToStringExtended());
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Label("Local Object State", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Is Owner", _target.IsOwner() ? "Yes" : "No");
                EditorGUILayout.LabelField("Is Server Object", _target.IsServerObject ? "Yes" : "No");
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawObjectsGUI()
        {
            _drawObjects.value = EditorGUILayout.BeginFoldoutHeaderGroup(_drawObjects.value, "Draw Components Info");
            if (_drawObjects.value)
            {
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;
                for (var index = 0; index < _target.Objects.Count; index++)
                {
                    var obj = _target.Objects[index];
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.LabelField("Component Index", obj.ComponentIndex.ToString());
                    if (QNetObject.GetQNetObjectIndex(obj.GetType(), out var typeIndex))
                        EditorGUILayout.LabelField("Component Type Index",  typeIndex.ToString());
                    else
                    {
                        EditorGUILayout.LabelField("Component Type Index", "Failed to resolve.");
                    }

                    EditorGUILayout.ObjectField(obj, typeof(QNetObject), false);

                    if (obj is QNetBehaviour b)
                    {
                        var drawMessages = new SavedBool($"{nameof(QNetIdentity)}.DrawBehaviourMessages.{index}", false);
                        drawMessages.value = EditorGUILayout.Foldout(drawMessages.value, "Behaviour Messages");
                        if (drawMessages.value)
                        {
                            EditorGUI.indentLevel++;
                            for (var i = 0; i < b.MessagePointers.Count; i++)
                            {
                                var msg = b.MessagePointers[i];
                                EditorGUILayout.LabelField("Name", msg.Delegate.Method.Name);
                                EditorGUI.indentLevel++;
                                EditorGUILayout.LabelField("Index", msg.Index.ToString());
                                EditorGUILayout.LabelField("Is Generic", msg.Delegate.Method.IsGenericMethod ? "Yes" : "No");
                                if (msg.Delegate.Method.IsGenericMethod)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.LabelField("Generic Arguments",
                                        msg.Delegate.Method.GetGenericArguments().Length.ToString());
                                    EditorGUI.indentLevel--;
                                }

                                EditorGUI.indentLevel--;
                            }

                            EditorGUI.indentLevel--;
                        }
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static int FixPredefinedIdentity(int identity)
        {
            var objectsOnScene = FindObjectsOfType<QNetIdentity>();
            if (identity == 0)
            {
                identity = (ushort) Random.Range(ushort.MinValue, ushort.MaxValue);
            }

            var b = true;
            while (b)
            {
                var count = objectsOnScene.Count(obj => obj.CustomIdentity == identity);
                if (count >= 2)
                    identity = (ushort) Random.Range(ushort.MinValue, ushort.MaxValue);
                else
                    b = false;
            }

            return identity;
        }
    }
}