//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using Newtonsoft.Json;
using System;
using System.IO;

#if DEBUG
using JEM.Core.Debugging;
#endif

namespace JEM.Core.Configuration
{
    /// <summary>
    ///     Internal JEM Configuration data used by JEMConfiguration.
    /// </summary>
    [Serializable]
    public class InternalJEMConfiguration
    {
        /// <summary>
        ///     Base configuration save path.
        /// </summary>
        public string ConfigurationAppSaveDirectory = "Config";

        /// <summary>
        ///     Base configuration save path.
        /// </summary>
        public string ConfigurationJEMSaveDirectory = "JEM";

        /// <summary>
        ///     Extension of json configuration file.
        /// </summary>
        public string ConfigurationJsonFileExtension = ".json";

        /// <summary>
        ///     Configuration save method.
        /// </summary>
        public JEMConfigurationSaveMethod ConfigurationSaveMethod = JEMConfigurationSaveMethod.JSON;

        /// <summary>
        ///     Json formatting method.
        /// </summary>
        public Formatting JsonFormattingMethod = Formatting.Indented;

        /// <summary>
        ///     Of set to true, JEMLogger will ignore every single log received.
        /// </summary>
        public bool NoLogs = false;

        /// <summary>
        ///     Currently loaded configuration of JEMConfiguration class.
        /// </summary>
        public static InternalJEMConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Resolves file name of internal cfg.
        /// </summary>
        private static string ResolveConfigurationFile()
        {
            var file =
                $@"{JEMConfiguration.CurrentDirectory}{JEMProgram.DirectorySeparator}JEM{JEMProgram.DirectorySeparator}JEMConfiguration.json";
            var dir = Path.GetDirectoryName(file);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return file;
        }

        /// <summary>
        ///     Loads current configuration.
        /// </summary>
        public static InternalJEMConfiguration Load()
        {
            var file = ResolveConfigurationFile();
            if (File.Exists(file))
            {
                Configuration = JsonConvert.DeserializeObject<InternalJEMConfiguration>(File.ReadAllText(file));
#if DEBUG
                if (Configuration != null)
                    JEMLogger.Log($"InternalJEMConfiguration loaded data from {file}", "JEM");
                else
                    JEMLogger.Log($"Unable to load InternalJEMConfiguration from file {file}", "JEM");
#endif
                return Configuration;
            }

            Configuration = new InternalJEMConfiguration();
            Save();
            return Configuration;
        }

        /// <summary>
        ///     Resolves JEM configuration.
        /// </summary>
        public static InternalJEMConfiguration Resolve() => Configuration ?? Load();
        
        /// <summary>
        ///     Saves current configuration.
        /// </summary>
        public static void Save()
        {
            var file = ResolveConfigurationFile();
#if DEBUG
            JEMLogger.Log($"Saving InternalJEMConfiguration data to file {file}", "JEM");
#endif
            File.WriteAllText(file, JsonConvert.SerializeObject(Configuration, Formatting.Indented));
        }
    }
}