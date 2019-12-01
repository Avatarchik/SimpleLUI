//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.UnityEditor.Configuration;
using JEM.UnityEngine.VersionManagement;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Timers;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace JEM.UnityEditor.VersionManagement
{
    /// <summary>
    ///     A simple class that will help us with starting up the session time calc process.
    /// </summary>
    [InitializeOnLoad]
    internal class JEMBuildOnLoad
    {
        static JEMBuildOnLoad()
        {
            if (Timer != null) return;

            // Try to refresh local data
            JEMBuildEditor.TryRefreshEditorData();

            const int space = 1000;

            // Setup timer
            Timer = new Timer(space);
            Timer.Elapsed += (sender, args) =>
            {
                if (!JEMEditorConfiguration.Configuration.UpdateWorkTime) return;
                if (JEMBuildEditor.CurrentCompilation == null || !Application.isFocused) return;
                JEMBuildEditor.CurrentSessionTime += space / 1000;
                JEMBuildEditor.CurrentCompilation.SessionTime += space / 1000;

                if (_shallSave >= 5)
                {
                    _shallSave = 0;
                    var serializedCompilationStr =
                        JsonConvert.SerializeObject(JEMBuildEditor.CurrentCompilation, Formatting.Indented);
                    File.WriteAllText(JEMBuildEditor.CompilationNumberFile, serializedCompilationStr);
                }
                else _shallSave++;
            };
            Timer.Start();

            // Hook the assembly compilation start event 
            CompilationPipeline.assemblyCompilationStarted += OnAssemblyCompilationStarted;
        }

        public static bool UpdateCompilationNumberOnce;
        private static void OnAssemblyCompilationStarted(string str)
        {
            if (UpdateCompilationNumberOnce)
            {
                return;
            }

            UpdateCompilationNumberOnce = true;
    
            // Load current contributor compilation data
            var compilationData = (TextAsset) AssetDatabase.LoadAssetAtPath(JEMBuildEditor.CompilationNumberFile, typeof(TextAsset));
            if (compilationData != null)
                JEMBuildEditor.CurrentCompilation = JsonConvert.DeserializeObject<JEMBuildCompilation>(compilationData.text);

            if (JEMBuildEditor.CurrentCompilation == null)
            {
                JEMLogger.LogError("Unable to load compilation data of current contributor. Update of the compilation number failed.");
                return;
            }

            // Update the compilation numver
            JEMBuildEditor.CurrentCompilation.CompilationNumber++;
            JEMBuildEditor.CurrentCompilation.CompilationTime = DateTime.UtcNow;

            // Serialize and Save
            var json = JsonConvert.SerializeObject(JEMBuildEditor.CurrentCompilation, Formatting.Indented);
            File.WriteAllText(JEMBuildEditor.CompilationNumberFile, json);

            if (!JEMBuildEditor.ResolveCurrentContributorName(out var contributorName))
            {
                JEMLogger.LogError($"Current contributor name resolve failed. Update of the compilation number failed.");
                return;
            }

            File.WriteAllText(JEMBuildEditor.LastContributorFile, contributorName);
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            AssetDatabase.SaveAssets();

            // RefreshLocalData();
        }

        private static int _shallSave;
        private static readonly Timer Timer;
    }
}
