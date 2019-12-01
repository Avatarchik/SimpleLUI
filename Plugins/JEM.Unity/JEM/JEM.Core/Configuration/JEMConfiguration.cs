//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.Core.Extension;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace JEM.Core.Configuration
{
    /// <summary>
    ///     Configuration manager.
    /// </summary>
    public static class JEMConfiguration
    {
        /// <summary>
        ///     Current directory of JEM Configuration system.
        /// </summary>
        public static string CurrentDirectory = Environment.CurrentDirectory;

        /// <summary>
        ///     Current configuration.
        /// </summary>
        private static InternalJEMConfiguration Configuration =>
            InternalJEMConfiguration.Configuration ?? InternalJEMConfiguration.Load();

        /// <summary>
        ///     Resolves fixed save method.
        /// </summary>
        private static JEMConfigurationSaveMethod ResolveSaveMethod(JEMConfigurationSaveMethod userMethod)
        {
            var method = userMethod;
            if (method == JEMConfigurationSaveMethod.UNKNOWN)
                method = Configuration.ConfigurationSaveMethod;
            if (method == JEMConfigurationSaveMethod.UNKNOWN)
                method = JEMConfigurationSaveMethod.JSON;
            return method;
        }

        /// <summary>
        ///     Writes data to file.
        /// </summary>
        /// <param name="path">Path to file.</param>
        /// <param name="value">Save data.</param>
        /// <param name="forcedMethod">Forced save method</param>
        public static void WriteData(string path, object value,
            JEMConfigurationSaveMethod forcedMethod = JEMConfigurationSaveMethod.UNKNOWN)
        {
            switch (ResolveSaveMethod(forcedMethod))
            {
                case JEMConfigurationSaveMethod.JSON:
#if DEBUG
                    JEMLogger.Log($"Writing configuration file of type {value.GetType().Name} using JSON at {path}. " +
                                  $"Formatting is {Configuration.JsonFormattingMethod}.", "JEM");
#endif
                    File.WriteAllText(path, JsonConvert.SerializeObject(value, Configuration.JsonFormattingMethod));
                    break;
                case JEMConfigurationSaveMethod.UNKNOWN:
                    throw new InvalidOperationException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Loads data from file.
        /// </summary>
        /// <typeparam name="T">Type of save.</typeparam>
        /// <param name="path">Path to file.</param>
        /// <param name="forcedMethod">Forced save method</param>
        public static T LoadData<T>(string path,
            JEMConfigurationSaveMethod forcedMethod = JEMConfigurationSaveMethod.UNKNOWN)
        {
            return LoadData<T>(path, false, forcedMethod);
        }

        /// <summary>
        ///     Loads data from file.
        /// </summary>
        /// <typeparam name="T">Type of save.</typeparam>
        /// <param name="path">Path to file.</param>
        /// <param name="disallowDefaultInstance">Disallows to create default instance of given type when file not exists.</param>
        /// <param name="forcedMethod">Forced save method</param>
        public static T LoadData<T>(string path, bool disallowDefaultInstance,
            JEMConfigurationSaveMethod forcedMethod = JEMConfigurationSaveMethod.UNKNOWN)
        {
            if (File.Exists(path))
            {
                switch (ResolveSaveMethod(forcedMethod))
                {
                    case JEMConfigurationSaveMethod.JSON:
#if DEBUG
                        JEMLogger.Log($"Loading configuration file using JSON from {path}.", "JEM");
#endif
                        return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
                    case JEMConfigurationSaveMethod.UNKNOWN:
                        throw new InvalidOperationException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (disallowDefaultInstance)
            {
#if  DEBUG
                JEMLogger.Log("System is trying to load configuration data " +
                             $"from file that not exists. File path is {path}", "JEM");
#endif
                return default(T);
            }

#if DEBUG
            JEMLogger.Log("System is trying to load configuration data " +
                          "from file that not exists. System will create and save default one." +
                          $" File path is {path}", "JEM");
#endif
            var obj = FastObjectFactory<T>.Instance();
            WriteData(path, obj, forcedMethod);
            return obj;
        }

        /// <summary>
        ///     Resolves path to configuration file of given name.
        /// </summary>
        public static string ResolveFilePath(string fileName,
            JEMConfigurationSaveMethod forcedMethod = JEMConfigurationSaveMethod.UNKNOWN)
        {
            var filePath =
                $@"{CurrentDirectory}{JEMProgram.DirectorySeparator}{Configuration.ConfigurationAppSaveDirectory}{JEMProgram.DirectorySeparator}{fileName}";
            var method = forcedMethod == JEMConfigurationSaveMethod.UNKNOWN
                ? Configuration.ConfigurationSaveMethod
                : forcedMethod;
            switch (method)
            {
                case JEMConfigurationSaveMethod.JSON:
                    filePath += Configuration.ConfigurationJsonFileExtension;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var fileDirectory = Path.GetDirectoryName(filePath);
            if (fileDirectory != null && !Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);

            return filePath;
        }

        /// <summary>
        ///     Resolves path to configuration file of given name.
        /// </summary>
        public static string ResolveJEMFilePath(string fileName)
        {
            var filePath = $@"{CurrentDirectory}{JEMProgram.DirectorySeparator}{Configuration.ConfigurationJEMSaveDirectory}{JEMProgram.DirectorySeparator}{fileName}";
            switch (Configuration.ConfigurationSaveMethod)
            {
                case JEMConfigurationSaveMethod.JSON:
                    filePath += Configuration.ConfigurationJsonFileExtension;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var fileDirectory = Path.GetDirectoryName(filePath);
            if (fileDirectory != null && !Directory.Exists(fileDirectory))
                Directory.CreateDirectory(fileDirectory);

            return filePath;
        }
    }
}