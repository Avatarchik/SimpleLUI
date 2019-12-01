﻿//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Configuration;
using System;

namespace JEM.UnityEditor.AssetBundles
{
    /// <summary>
    ///     JEM Assets Builder configuration.
    /// </summary>
    [Serializable]
    public class JEMAssetsBuilderConfiguration
    {
        /// <summary>
        ///     Current extension of packages.
        /// </summary>
        public string PackageExtension = "bundle";

        /// <summary>
        ///     Relative directory to all exported packages.
        /// </summary>
        public string PackageDirectory = "Exported";

        /// <summary>
        ///     If false, the asset bundles will receive no compression at all.
        /// </summary>
        public bool CompressAssetBundles = true;

        /// <summary>
        ///     If false, the Stream Compressed (LZMA) method will be used instead of Chuck Compressed (LZ4) method.
        /// </summary>
        public bool ChunkBasedAssetBundlesCompression = true;

        /// <summary>
        ///     Gets the file extension of AssetBundles.
        /// </summary>
        public static string GetExtension() => $".{Configuration.PackageExtension}";

        /// <summary>
        ///     Gets the target directory of exported files.
        /// </summary>
        public static string GetDirectory() => Environment.CurrentDirectory + "\\" + Configuration.PackageDirectory;

        /// <summary>
        ///     Try to load configuration.
        ///     If the configuration is already loaded, nothing will happen.
        /// </summary>
        public static void TryLoadConfiguration()
        {
            if (_isConfigurationLoaded)
                return;

            Load();
            _isConfigurationLoaded = true;
        }

        /// <summary>
        ///     Loads configuration.
        /// </summary>
        public static void Load()
        {
            Configuration = JEMConfiguration.LoadData<JEMAssetsBuilderConfiguration>(ConfigurationFile, JEMConfigurationSaveMethod.JSON);
        }

        /// <summary>
        ///     Saves current configuration.
        /// </summary>
        public static void Save()
        {
            if (Configuration == null)
            {
                return;
            }

            JEMConfiguration.WriteData(ConfigurationFile, Configuration, JEMConfigurationSaveMethod.JSON);
        }

        /// <summary>
        ///     Loaded configuration data.
        /// </summary>
        public static JEMAssetsBuilderConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Patch to asset builder configuration
        /// </summary>
        public static string ConfigurationFile => JEMConfiguration.ResolveJEMFilePath("AssetBuilderConfiguration");

        private static bool _isConfigurationLoaded;
    }
}