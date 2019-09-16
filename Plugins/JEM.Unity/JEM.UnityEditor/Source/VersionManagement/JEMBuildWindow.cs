//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEditor.Configuration;
using System;
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor.VersionManagement
{
    internal class JEMBuildWindow : EditorWindow
    {
        private SavedBool _drawGlobalStats;
        private SavedBool _drawLocalStats;

        private Vector2 _scrollPosition;

        private void OnEnable()
        {
            _drawGlobalStats = new SavedBool($"{nameof(JEMBuildWindow)}.DrawGlobalStats", false);
            _drawLocalStats = new SavedBool($"{nameof(JEMBuildWindow)}.DrawLocalStats", true);

            // Load JEM editor resources
            JEMEditorResources.Load();

            // Apply Title
            titleContent = new GUIContent("JEM Build", JEMEditorResources.JEMIconTexture);

            // Try to refresh local data
            JEMBuildEditor.TryRefreshEditorData();
        }

        // Do we need this?
        private void OnInspectorUpdate() => Repaint();

        private void OnGUI()
        {
            // Check for contributor name
            if (!JEMBuildEditor.IsCurrentContributorNameFileExists())
            {
                EditorGUILayout.HelpBox("System was unable to resolve username.txt file. " +
                                        $"Please, create {$@"{Environment.CurrentDirectory}\username.txt"}",
                                        MessageType.Error, true);
                return;
            }

            var isDataLoaded = !(JEMBuildEditor.CurrentCompilation == null ||
                                 JEMBuildEditor.CurrentVersion == null);
            if (!isDataLoaded)
            {
                EditorGUILayout.HelpBox("Failed to initialize compilation or version resource.", MessageType.Error, true);
                if (GUILayout.Button("Refresh", GUILayout.Height(30)))
                {
                    JEMBuildEditor.RefreshEditorData();
                }

                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);  
            
            EditorGUILayout.Space();
            OnCompilationGUI();

            EditorGUILayout.Space();
            OnVersionGUI();

            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(GUILayout.Height(25));
            {
                if (GUILayout.Button("Save", GUILayout.ExpandHeight(true)))
                {
                    JEMBuildEditor.SaveEditorData();
                }

                if (GUILayout.Button("Refresh", GUILayout.Width(65), GUILayout.ExpandHeight(true)))
                {
                    JEMBuildEditor.RefreshEditorData();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal(GUILayout.Height(22));
            {
                var shouldBundleUpdate = JEMEditorConfiguration.Configuration?.UpdateBundleVersion ?? false;
                if (shouldBundleUpdate)
                {
                    GUI.enabled = false;
                    GUILayout.Button("Update Bundle Version (Auto)", GUILayout.ExpandHeight(true));
                    GUI.enabled = true;
                }
                else
                {
                    if (GUILayout.Button("Apply Bundle Version", GUILayout.ExpandHeight(true)))
                    {
                        JEMBuildEditor.UpdateBundleVersion();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();
        }

        private void OnCompilationGUI()
        {
            var sessionTime = TimeSpan.FromSeconds(JEMBuildEditor.CurrentSessionTime);
            var localSession = TimeSpan.FromSeconds(JEMBuildEditor.CurrentCompilation.SessionTime);

            var buildContribution = (float) JEMBuildEditor.CurrentCompilation.BuildNumber / JEMBuildEditor.CurrentBuildNumber * 100f;
            var compilationContribution = (float) JEMBuildEditor.CurrentCompilation.CompilationNumber / JEMBuildEditor.CurrentCompilationNumber * 100f;
            var sessionTimeContribution = (float) JEMBuildEditor.CurrentCompilation.SessionTime / JEMBuildEditor.CurrentSessionTime * 100f;

            _drawGlobalStats.value = EditorGUILayout.BeginFoldoutHeaderGroup(_drawGlobalStats.value, "Global Statistics");
            if (_drawGlobalStats.value)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                EditorGUILayout.HelpBox($"Build: {JEMBuildEditor.CurrentBuildNumber}", MessageType.Info, true);

                EditorGUILayout.HelpBox($"Compilation: {JEMBuildEditor.CurrentCompilationNumber}", MessageType.Info, true);

                EditorGUILayout.HelpBox($"Work time: {sessionTime.Days:D2}d" +
                                                  $":{sessionTime.Hours:D2}h" +
                                                  $":{sessionTime.Minutes:D2}m" +
                                                  $":{sessionTime.Seconds:D2}s", MessageType.Info, true);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            _drawLocalStats.value = EditorGUILayout.BeginFoldoutHeaderGroup(_drawLocalStats.value, "Local Statistics");
            if (_drawLocalStats.value)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUI.indentLevel++;

                EditorGUILayout.HelpBox($"Local Build: {JEMBuildEditor.CurrentCompilation.BuildNumber} " +
                                        $"({buildContribution:0.00}%)", MessageType.Info, true);

                EditorGUILayout.HelpBox($"Local Compilation: {JEMBuildEditor.CurrentCompilation.CompilationNumber} " +
                                        $"({compilationContribution:0.00}%)", MessageType.Info, true);

                EditorGUILayout.HelpBox($"Local Work Time: {localSession.Days:D2}d" +
                                                    $":{localSession.Hours:D2}h" +
                                                    $":{localSession.Minutes:D2}m" +
                                                    $":{localSession.Seconds:D2}s ({sessionTimeContribution:0.00}%)", MessageType.Info, true);

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void OnVersionGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.Label("Version", EditorStyles.boldLabel);
                JEMBuildEditor.CurrentVersion.VersionName = EditorGUILayout.TextField(JEMBuildEditor.CurrentVersion.VersionName);
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(35));
                {
                    var timeStyle = new GUIStyle(EditorStyles.helpBox) {alignment = TextAnchor.MiddleCenter};
                    GUILayout.Box($"{JEMBuildEditor.CurrentVersion.VersionRelease:yyyy-MM-dd HH:mm:ss}", timeStyle, GUILayout.ExpandHeight(true));
                    if (GUILayout.Button("Update", GUILayout.ExpandHeight(true), GUILayout.Width(50)))
                    {
                        JEMBuildEditor.CurrentVersion.VersionRelease = DateTime.UtcNow;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        ///     Show the JEM Build window.
        /// </summary>
        [MenuItem("JEM/JEM Build (Version Management)")]
        public static void ShowWindow()
        {
            var activeWindow = GetWindow<JEMBuildWindow>(true, "JEM Build", true);
            activeWindow.minSize = new Vector2(250, 240);
            activeWindow.Show();
        }
    }
}