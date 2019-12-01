﻿//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using JEM.Core.Configuration;
using JEM.Core.Extension;
using JEM.UnityEditor.AssetBundles;
using JEM.UnityEditor.Locale;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor.Configuration
{
    /// <inheritdoc />
    /// <summary>
    ///     JEM Configuration Window.
    /// </summary>
    public class JEMEditorConfigurationWindow : EditorWindow
    {
        private readonly string[] Tabs = { "About", "Core Settings", "Editor Settings", "AssetBuilder\nSettings", "Locale Editor" };

        private int SelectedTab;

        private void OnEnable()
        {
            // Load JEM editor resources
            JEMEditorResources.Load();

            // Apply Title
            titleContent = new GUIContent("JEM Configuration", JEMEditorResources.JEMIconTexture);

            // Load all configurations
            InternalJEMConfiguration.Load();
            JEMEditorConfiguration.Load();
            JEMAssetsBuilderConfiguration.Load();
        }

        // Do we need this?
        private void OnInspectorUpdate() => Repaint();
        
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(126));
            { 
                SelectedTab = GUILayout.SelectionGrid(SelectedTab, Tabs, 1);
                GUILayout.FlexibleSpace();

                var drawSave = false;
                switch (SelectedTab)
                {
                    case 0:
                        break;
                    case 1:
                        if (GUILayout.Button("Force Reload\nJEM Resources", GUILayout.Height(40)))
                            JEMEditorResources.Load(true);
                        drawSave = true;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        drawSave = true;
                        break;
                }

                if (drawSave)
                {
                    if (GUILayout.Button("Save"))
                        SaveConfigurationData();
                }

            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                switch (SelectedTab)
                {
                    case 0: // About
                        OnAboutGUI();
                        break;
                    case 1: // Core Settings
                        OnCoreSettingsGUI();
                        break;
                    case 2: // Editor Settings
                        OnEditorSettingsGUI();
                        break;
                    case 3: // AssetBundles Settings
                        OnAssetBundlesSettingsGUI();
                        break;
                    case 4: // Locale Editor Settings
                        OnLocaleEditorGUI();
                        break;
                    default:
                        EditorGUILayout.HelpBox("You are trying to draw page that not exist or is implemented yet :/",
                            MessageType.Error, true);
                        break;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void OnAboutGUI()
        {
            GUILayout.Label("Just Enough Methods", EditorStyles.boldLabel);
            GUILayout.Label("Library Extension for UnityEngine and UnityEditor");

            EditorGUILayout.Space();
            if (GUILayout.Button("GitHub", GUILayout.Height(25), GUILayout.Width(150)))
                Application.OpenURL("https://github.com/TylkoDemon/JEM.Unity");

            GUILayout.FlexibleSpace();
            GUILayout.Label("Copyright (c) 2017-2019 ADAM MAJCHEREK\nALL RIGHTS RESERVED");
        }

        private void OnCoreSettingsGUI()
        {
            var cfg = InternalJEMConfiguration.Configuration;

            GUILayout.Label("JEM Core Settings", EditorStyles.boldLabel);
            cfg.ConfigurationSaveMethod = (JEMConfigurationSaveMethod) EditorGUILayout.EnumPopup("Configuration Save Method", cfg.ConfigurationSaveMethod);
            if (cfg.ConfigurationSaveMethod == JEMConfigurationSaveMethod.UNKNOWN) cfg.ConfigurationSaveMethod = JEMConfigurationSaveMethod.JSON;
            cfg.ConfigurationAppSaveDirectory =  EditorGUILayout.TextField("App Save Directory", cfg.ConfigurationAppSaveDirectory);

            switch (cfg.ConfigurationSaveMethod)
            {
                case JEMConfigurationSaveMethod.UNKNOWN:
                    EditorGUILayout.HelpBox("UNKNOWN", MessageType.Error, true);
                    break;
                case JEMConfigurationSaveMethod.JSON:
                    EditorGUILayout.Space();
                    cfg.ConfigurationJsonFileExtension = EditorGUILayout.TextField("Json File Extension", cfg.ConfigurationJsonFileExtension);
                    cfg.JsonFormattingMethod = (Formatting)EditorGUILayout.EnumPopup("JSON Formatting Method", cfg.JsonFormattingMethod);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUILayout.Space();
            cfg.NoLogs = EditorGUILayout.Toggle("No Logs",  cfg.NoLogs);
        }

        private void OnEditorSettingsGUI()
        {
            var cfg = JEMEditorConfiguration.Configuration;

            GUILayout.Label("JEM Editor Settings", EditorStyles.boldLabel);
            cfg.UpdateBundleVersion = EditorGUILayout.Toggle("Update Bundle Version", cfg.UpdateBundleVersion);
            cfg.UpdateWorkTime = EditorGUILayout.Toggle("Update Work Time", cfg.UpdateWorkTime);
        }

        private void OnAssetBundlesSettingsGUI()
        {
            EditorGUIUtility.labelWidth += 25f;
            var cfg = JEMAssetsBuilderConfiguration.Configuration;

            GUILayout.Label("JEM AssetBuilder Settings", EditorStyles.boldLabel);
            cfg.PackageExtension = EditorGUILayout.TextField("Package Extension", cfg.PackageExtension);

            if (cfg.PackageExtension.Length != 0 && cfg.PackageExtension[0] == '.')
            {
                EditorGUILayout.HelpBox("Package extension starts with '.'!", MessageType.Warning, true);
            }

            EditorGUILayout.TextField("Directory", cfg.PackageDirectory);
            JEMBetterEditor.DrawProperty(" ", () =>
            {
                if (GUILayout.Button("Select"))
                {
                    var directory = EditorUtility.OpenFolderPanel("Select directory of package", cfg.PackageDirectory, "");
                    cfg.PackageDirectory = ExtensionPath.ResolveRelativeFilePath(directory);
                }
            });

            cfg.CompressAssetBundles = EditorGUILayout.Toggle("Asset Bundles Compression", cfg.CompressAssetBundles);
            if (cfg.CompressAssetBundles)
            {
                EditorGUI.indentLevel++;
                cfg.ChunkBasedAssetBundlesCompression = EditorGUILayout.Toggle("Chunk Based", cfg.ChunkBasedAssetBundlesCompression);
                if (cfg.ChunkBasedAssetBundlesCompression)
                {
                    EditorGUILayout.HelpBox("Chunk Compressed (LZ4) compression method.", MessageType.Info, true);
                }
                else
                {
                    EditorGUILayout.HelpBox("Stream Compressed (LZMA) compression method.", MessageType.Info, true);
                }

                EditorGUI.indentLevel--;
            }
            EditorGUIUtility.labelWidth -= 25f;
        }

        private void OnLocaleEditorGUI()
        {
            var cfg = JEMLocaleEditorConfiguration.Configuration;

            GUILayout.Label("JEM Locale Editor Settings", EditorStyles.boldLabel);
            cfg.LocaleEditorDirectory = EditorGUILayout.TextField("Locale Directory Name", cfg.LocaleEditorDirectory);
            cfg.ExportSerialized = EditorGUILayout.Toggle("Export Serialized", cfg.ExportSerialized);
            if (cfg.ExportSerialized)
            {
                EditorGUILayout.HelpBox("While ExportSerialized is active, the locale editor will export .csvb files (instead of .csv) " +
                                        "that are serialized using JEMEncryptor utility.", MessageType.Info, true);
            }
        }

        /// <summary>
        ///     Saves all the configurations!
        /// </summary>
        private static void SaveConfigurationData()
        {
            // save all configurations
            InternalJEMConfiguration.Save();
            JEMEditorConfiguration.Save();
            JEMAssetsBuilderConfiguration.Save();
        }

        /// <summary>
        ///     Shows the JEM Configuration Editor Window.
        /// </summary>
        [MenuItem("JEM/JEM Configuration")]
        public static void ShowWindow()
        {
            var activeWindow = GetWindow<JEMEditorConfigurationWindow>(true, "JEM Configuration", true);
            activeWindow.maxSize = new Vector2(480, 350);
            activeWindow.minSize = activeWindow.maxSize;
        }
    }
}