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
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor.VersionManagement
{
    /// <summary>
    ///     JEM Build Editor.
    ///     A set of methods for UnityEditor related to JEMBuild class.
    /// </summary>
    public static class JEMBuildEditor
    {
        private static bool _wasDataRefreshed;

        /// <summary>
        ///     Try to refresh local data. This method will only invoke once.
        /// </summary>
        public static void TryRefreshEditorData()
        {
            if (_wasDataRefreshed)
                return;

            // Load editor configuration data first
            JEMEditorConfiguration.Load();

            RefreshEditorData();
            _wasDataRefreshed = true;
        }

        /// <summary>
        ///     Refresh the editor data.
        /// </summary>
        public static void RefreshEditorData()
        {
            //
            // Load version data
            //
            var versionData = (TextAsset)AssetDatabase.LoadAssetAtPath(VersionFile, typeof(TextAsset));
            if (versionData == null)
            {
                // No JEMVersion data found
                // Initialize new
                CurrentVersion = new JEMBuildVersion();

                var dir = Path.GetDirectoryName(VersionFile);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir ?? throw new InvalidOperationException());
                }

                File.WriteAllText(VersionFile, JsonConvert.SerializeObject(CurrentVersion,
                    Formatting.Indented));
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                AssetDatabase.SaveAssets();

                JEMLogger.Log($"File {VersionFile} not exist. New file has been created.");
            }
            else CurrentVersion = JsonConvert.DeserializeObject<JEMBuildVersion>(versionData.text);
            if (CurrentVersion == null)
            {
                JEMLogger.LogError("Unable to resolve current version data.");
            }

            //
            // Load compilation data
            //
            var compilationData = !File.Exists(CompilationNumberFile) ? null : (TextAsset) AssetDatabase.LoadAssetAtPath(CompilationNumberFile, typeof(TextAsset));
            if (compilationData == null)
            {
                // Resolve current contributor name
                if (!ResolveCurrentContributorName(out var contributorName))
                {
                    return;
                }

                // No JEMBuildCompilation data found
                // Initialize new 
                CurrentCompilation = new JEMBuildCompilation();

                var dir = Path.GetDirectoryName(CompilationNumberFile);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir ?? throw new InvalidOperationException());
                }

                File.WriteAllText(CompilationNumberFile, JsonConvert.SerializeObject(CurrentCompilation, Formatting.Indented));
                File.WriteAllText(LastContributorFile, contributorName);
                AssetDatabase.Refresh(ImportAssetOptions.Default);
                AssetDatabase.SaveAssets();

                Debug.Log($"File {CompilationNumberFile} not exist. New file has been created.");
            }
            else
            {
                CurrentCompilation = JsonConvert.DeserializeObject<JEMBuildCompilation>(compilationData.text);
            }

            if (CurrentCompilation == null) Debug.LogError("Unable to resolve current compilation data.");

            // Apply compilation number
            CurrentCompilationNumber = JEMBuild.ResolveCurrentCompilationNumber();
            CurrentBuildNumber = JEMBuild.ResolveCurrentBuildNumber();
            CurrentSessionTime = JEMBuild.ResolveCurrentSessionTime();

            // Apply bundle version
            if (JEMEditorConfiguration.Configuration?.UpdateBundleVersion ?? false)
            {
                UpdateBundleVersion();
            }
        }

        /// <summary>
        ///     Saves data to local file.
        /// </summary>
        public static void SaveEditorData()
        {
            // Save version info
            if (CurrentVersion != null)
            {
                var json = JsonConvert.SerializeObject(CurrentVersion, Formatting.Indented);
                var dir = Path.GetDirectoryName(VersionFile);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(VersionFile, json);
            }

            // save compilation info
            if (CurrentCompilation != null)
            {
                // Resolve current contributor name
                if (!ResolveCurrentContributorName(out var contributorName))
                {
                    return;
                }

                var json = JsonConvert.SerializeObject(CurrentCompilation, Formatting.Indented);
                var dir = Path.GetDirectoryName(CompilationNumberFile);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                File.WriteAllText(CompilationNumberFile, json);
                File.WriteAllText(LastContributorFile, contributorName);
            }

            // Save asset database
            AssetDatabase.Refresh(ImportAssetOptions.Default);
            AssetDatabase.SaveAssets();

            // Refresh local data
            RefreshEditorData();
        }

        /// <summary>
        ///     Updates PlayerSettings.bundleVersion
        /// </summary>
        public static void UpdateBundleVersion()
        {
            PlayerSettings.bundleVersion = $"{CurrentVersion?.VersionName ?? "INTERNAL_VERSION_ERROR"} " +
                                           $"@{CurrentBuildNumber}.{CurrentCompilationNumber}";

            Debug.Log($"BundleVersion updated by JEMBuildEditor to {'"'}{PlayerSettings.bundleVersion}{'"'}");
        }

        /// <summary>
        ///     Increase build number.
        /// </summary>
        public static void IncreaseBuildNumber()
        {
            var compilationData = (TextAsset)AssetDatabase.LoadAssetAtPath(CompilationNumberFile, typeof(TextAsset));
            if (compilationData != null)
                CurrentCompilation = JsonConvert.DeserializeObject<JEMBuildCompilation>(compilationData.text);

            if (CurrentCompilation == null)
            {
                JEMLogger.LogError("Unable to load compilation data to update number.");
                return;
            }

            // Resolve current contributor name
            if (!ResolveCurrentContributorName(out var contributorName))
            {
                return;
            }

            CurrentCompilation.BuildNumber++;
            CurrentCompilation.BuildTime = DateTime.UtcNow;

            var json = JsonConvert.SerializeObject(CurrentCompilation, Formatting.Indented);
            File.WriteAllText(CompilationNumberFile, json);
            File.WriteAllText(LastContributorFile, contributorName);

            AssetDatabase.Refresh(ImportAssetOptions.Default);
            AssetDatabase.SaveAssets();

            RefreshEditorData();
        }

        /// <summary>
        ///     Check if username file exists.
        /// </summary>
        public static bool IsCurrentContributorNameFileExists() => File.Exists(UserNameFile);

        /// <summary>
        ///     Resolves a name of current contributor.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public static bool ResolveCurrentContributorName(out string userName)
        {
            userName = string.Empty;
            if (File.Exists(UserNameFile)) userName = File.ReadAllLines(UserNameFile)[0];

            return !string.IsNullOrEmpty(userName);
            // throw new NullReferenceException($"System was unable to resolve username.txt file. Please, create {UserNameFile}");
        }

        /// <summary>
        ///     Path to file of current contributor username.
        /// </summary>
        public static string UserNameFile => $@"{Environment.CurrentDirectory}\username.txt";

        /// <summary>
        ///     A current version data.
        /// </summary>
        public static JEMBuildVersion CurrentVersion { get; internal set; }

        /// <summary>
        ///     A current compilation data.
        /// </summary>
        public static JEMBuildCompilation CurrentCompilation { get; internal set; }

        /// <summary>
        ///     A current compilation number.
        /// </summary>
        public static int CurrentCompilationNumber { get; internal set; }

        /// <summary>
        ///     A current build number.
        /// </summary>
        public static int CurrentBuildNumber { get; internal set; }

        /// <summary>
        ///     A current session time (defined in seconds).
        /// </summary>
        public static int CurrentSessionTime { get; internal set; }

        /// <summary>
        ///     Patch to the file of compilation number of current contributor.
        /// </summary>
        public static string CompilationNumberFile
        {
            get
            {
                if (!ResolveCurrentContributorName(out var contributorName))
                    return "CONTRIBUTOR_FILE_ERROR";

                return $"Assets/Resources/{JEMBuild.CompilationDataDirectory}/compNum_{contributorName}.json";
            }
        }

        /// <summary>
        ///     Patch to file of name of last contributor.
        /// </summary>
        public static string LastContributorFile => $"Assets/Resources/{JEMBuild.LastContributorFileName}.txt";

        /// <summary>
        ///     Patch to file of version.
        /// </summary>
        public static string VersionFile => $"Assets/Resources/{JEMBuild.VersionFileName}.json";
    }
}