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

namespace JEM.UnityEditor.Locale
{
    /// <summary>
    ///     JEM Locale Editor Configuration.
    /// </summary>
    [Serializable]
    public class JEMLocaleEditorConfiguration
    {
        /// <summary>
        ///     Directory to the all locale files.
        /// </summary>
        public string LocaleEditorDirectory = "Locale";

        /// <summary>
        ///     If true, exported .csv files will be binary serialized using JEMEncryptor utility.
        /// </summary>
        public bool ExportSerialized = false;

        /// <summary>
        ///     Resolves file name of internal cfg.
        /// </summary>
        private static string ResolveConfigurationFile()
        {
            var file = $@"{JEMConfiguration.CurrentDirectory}\JEM\JEMLocaleEditorConfiguration.json";
            var dir = Path.GetDirectoryName(file);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return file;
        }

        private static bool _isLoad;

        /// <summary>
        ///     Try to load current configuration.
        /// </summary>
        /// <remarks>
        ///     This method will work only once.
        /// </remarks>
        public static JEMLocaleEditorConfiguration TryLoad()
        {
            if (_isLoad)
            {
                return Configuration;
            }

            Load();

            _isLoad = true;
            return Configuration;
        }

        /// <summary>
        ///     Loads current configuration.
        /// </summary>
        public static JEMLocaleEditorConfiguration Load()
        {
            var file = ResolveConfigurationFile();
            if (File.Exists(file))
            {
                Configuration = JsonConvert.DeserializeObject<JEMLocaleEditorConfiguration>(File.ReadAllText(file));
                return Configuration;
            }

            Configuration = new JEMLocaleEditorConfiguration();
            Save();
            return Configuration;
        }

        /// <summary>
        ///     Saves current configuration.
        /// </summary>
        public static void Save()
        {
            var file = ResolveConfigurationFile();
            JEMLogger.Log($"Saving JEMEditorConfiguration data at {file}", "JEM");
            File.WriteAllText(file, JsonConvert.SerializeObject(Configuration, Formatting.Indented));
        }

        /// <summary>
        ///     Currently loaded configuration of JEMLocaleEditorConfiguration class.
        /// </summary>
        public static JEMLocaleEditorConfiguration Configuration { get; private set; }
    }
}
