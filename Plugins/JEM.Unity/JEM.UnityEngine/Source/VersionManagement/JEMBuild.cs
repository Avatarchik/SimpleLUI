//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;

namespace JEM.UnityEngine.VersionManagement
{
    /// <summary>
    ///     JEM Build.
    ///     It defined current build number and major game version that is later used by
    ///         for example QNet to validate if clients have the same game version.
    ///     It also has some statistics data about total compilations or total time spend in unity.
    /// </summary>
    public static class JEMBuild
    {
        /// <summary>
        ///     Loads JEM Build data from game's resources.
        /// </summary>
        public static void Load()
        {
            try
            {
                // Load version
                var versionData = (TextAsset) Resources.Load(VersionFileName, typeof(TextAsset));
                if (versionData != null)
                    Version = JsonConvert.DeserializeObject<JEMBuildVersion>(versionData.text);

                if (Version == null)
                    JEMLogger.Log("Failed to load JEMBuildVersion data.");

                // Load last contributor
                if (!ResolveLastContributorName(out var contributorName))
                    return;

                // Last contributor is used to get the last time of e build
                var compilationData = (TextAsset) Resources.Load($"{CompilationDataDirectory}/compNum_{contributorName}", typeof(TextAsset));
                if (compilationData != null)
                    LastCompilation = JsonConvert.DeserializeObject<JEMBuildCompilation>(compilationData.text);
                if (LastCompilation == null)
                    JEMLogger.Log("Failed to load JEMBuildCompilation data of last contributor.");

                // Apply data
                CompilationNumber = ResolveCurrentCompilationNumber();
                BuildNumber = ResolveCurrentBuildNumber();
                SessionTime = ResolveCurrentSessionTime();
            }
            catch (Exception ex)
            {
                JEMLogger.LogException("System was unable to initialize JEM Build. " + ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        ///     Resolves compilation number from all compilation datas in project.
        /// </summary>
        public static int ResolveCurrentCompilationNumber()
        {
            return (from TextAsset c in Resources.LoadAll(CompilationDataDirectory, typeof(TextAsset))
                where c.name.StartsWith("compNum_")
                select JsonConvert.DeserializeObject<JEMBuildCompilation>(c.text)
                into compilation
                where compilation != null
                select compilation.CompilationNumber).Sum();
        }

        /// <summary>
        ///     Resolves build number from all compilation datas in project.
        /// </summary>
        public static int ResolveCurrentBuildNumber()
        {
            return (from TextAsset c in Resources.LoadAll(CompilationDataDirectory, typeof(TextAsset))
                where c.name.StartsWith("compNum_")
                select JsonConvert.DeserializeObject<JEMBuildCompilation>(c.text)
                into compilation
                where compilation != null
                select compilation.BuildNumber).Sum();
        }

        /// <summary>
        ///     Resolves session time from all compilation datas in project.
        /// </summary>
        public static int ResolveCurrentSessionTime()
        {
            return (from TextAsset c in Resources.LoadAll(CompilationDataDirectory, typeof(TextAsset))
                where c.name.StartsWith("compNum_")
                select JsonConvert.DeserializeObject<JEMBuildCompilation>(c.text)
                into compilation
                where compilation != null
                select compilation.SessionTime).Sum();
        }

        /// <summary>
        ///     Resolves name of last user that contributes to this project.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public static bool ResolveLastContributorName(out string contributorName)
        {
            contributorName = string.Empty;
            var t = Resources.Load(LastContributorFileName, typeof(TextAsset)) as TextAsset;
            if (t != null)
            {
                contributorName = t.text;
                return true;
            }

            Debug.LogWarning($"JEMBuild was unable to resolve last contribution username. File '{LastContributorFileName}' not exist or is broken.");

            return false;
        }

        /// <summary>
        ///     File name of version data.
        /// </summary>
        public static string VersionFileName = "Editor/Version";

        /// <summary>
        ///    Directory name where all compilation datas are stored
        /// </summary>
        public static string CompilationDataDirectory = "Editor/Compilation";

        /// <summary>
        ///     File name of last contributor asset. Does not contains file extension.
        /// </summary>
        public static string LastContributorFileName => $"{CompilationDataDirectory}/Last";

        /// <summary>
        ///     Reference to compilation data of last contributor.
        /// </summary>
        public static JEMBuildCompilation LastCompilation { get; private set; }

        /// <summary>
        ///     Data contains info about game's version.
        /// </summary>
        public static JEMBuildVersion Version { get; private set; }

        /// <summary>
        ///     Total compilation number of this unity project.
        /// </summary>
        public static int CompilationNumber { get; private set; }

        /// <summary>
        ///     Total build number of this unity project.
        /// </summary>
        public static int BuildNumber { get; private set; }

        /// <summary>
        ///     Total unity project work time defined in seconds.
        /// </summary>
        public static int SessionTime { get; private set; }

        /// <summary>
        ///     Returns a build text ready to use in for ex. UI
        /// </summary>
        public static string BuildText => $"{BuildVersion} - {BuildRelease}\n{BuildCompilationText}";

        /// <summary>
        ///     Returns a rich ready build text ready to use in for ex. UI
        /// </summary>
        public static string BuildTextRich => $"<size=17>{BuildVersion} - {BuildRelease}</size>\n{BuildCompilationText}";

        /// <summary>
        ///     Returns a text that represents a current version release DateTime.
        /// </summary>
        public static string BuildRelease =>
            Version == null ? "INTERNAL_LOAD_ERROR" : $"{Version.VersionRelease:yyyy-MM-dd}";

        /// <summary>
        ///     Text of current compilation.
        ///     Returns a text that represents current BuildNumber with Branch name and LastCompilation Time.
        /// </summary>
        public static string BuildCompilationText => LastCompilation == null
            ? "INTERNAL_LOAD_ERROR" : $"{BuildNumber}/{BranchName} {LastCompilation.BuildTime:yyyy.MM.dd HH:mm:ss}";

        /// <summary>
        ///     Name of current version.
        /// </summary>
        public static string BuildVersion => Version == null ? "INTERNAL_LOAD_ERROR" : Version.VersionName;

        /// <summary>
        ///     Text that defines the name of current branch.
        /// </summary>
        public static string BranchName = "MAIN";
    }
}