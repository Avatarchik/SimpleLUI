//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Configuration;
using JEM.Core.Debugging;
using Newtonsoft.Json;
using System;
using System.IO;

namespace JEM.UnityEditor.Configuration
{
    /// <summary>
    ///     JEM Editor Configuration.
    /// </summary>
    [Serializable]
    public class JEMEditorConfiguration
    {
        /// <summary>
        ///     Defines whether the PlayerSettings.bundleVersion should be updated by system.
        /// </summary>
        public bool UpdateBundleVersion = true;

        /// <summary>
        ///     Update work time.
        /// </summary>
        public bool UpdateWorkTime = true;

        /// <summary>
        ///     Resolves file name of internal cfg.
        /// </summary>
        private static string ResolveConfigurationFile()
        {
            var file = $@"{JEMConfiguration.CurrentDirectory}\JEM\JEMEditorConfiguration.json";
            var dir = Path.GetDirectoryName(file);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return file;
        }

        /// <summary>
        ///     Loads current configuration.
        /// </summary>
        public static JEMEditorConfiguration Load()
        {
            var file = ResolveConfigurationFile();
            if (File.Exists(file))
            {
                Configuration = JsonConvert.DeserializeObject<JEMEditorConfiguration>(File.ReadAllText(file));
                return Configuration;
            }

            Configuration = new JEMEditorConfiguration();
            Save();
            return Configuration;
        }

        /// <summary>
        ///     Saves current configuration.
        /// </summary>
        public static void Save()
        {
            var file = ResolveConfigurationFile();
            JEMLogger.InternalLog($"Saving JEMEditorConfiguration data at {file}");
            File.WriteAllText(file, JsonConvert.SerializeObject(Configuration, Formatting.Indented));
        }

        /// <summary>
        ///     Currently loaded configuration of JEMEditorConfiguration class.
        /// </summary>
        public static JEMEditorConfiguration Configuration { get; private set; }
    }
}