//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JEM.Core.Text
{
    /// <summary>
    ///     JEM Locale Group.
    ///     A locale group data.
    ///     Each group is defined by it's file name.
    /// </summary>
    [Serializable]
    public class JEMLocaleGroup
    {
        /// <summary>
        ///     A name of locale group.
        /// </summary>
        public string GroupName = string.Empty;

        /// <summary>
        ///     Keys loaded in to this locale group.
        /// </summary>
        public Dictionary<string, string> Keys = new Dictionary<string, string>();
    }

    /// <summary>
    ///     JEM Locale Data
    ///     A data of loaded locale.
    /// </summary>
    [Serializable]
    public class JEMLocaleData
    {
        /// <summary>
        ///     Unique locale name. For ex.: eng, pl, etc.
        /// </summary>
        public string LocaleName = string.Empty;

        /// <summary>
        ///     Groups loaded in to this locale.
        /// </summary>
        public List<JEMLocaleGroup> Groups = new List<JEMLocaleGroup>();

        /// <summary>
        ///     Gets group of given name.
        ///     NOTE: Group name is always upper case!
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public JEMLocaleGroup GetGroup(string groupName)
        {
            if (groupName == null) throw new ArgumentNullException(nameof(groupName));
            return Groups.FirstOrDefault(g => g.GroupName == groupName);
        }

        /// <summary>
        ///     Gets the default locale group.
        ///     The default locale group is always the first one loaded if <see cref="JEMLocale.DefaultLocaleGroup"/> not exist.
        /// </summary>
        /// <exception cref="IndexOutOfRangeException"/>
        public JEMLocaleGroup GetDefault()
        {
            for (var index = 0; index < Groups.Count; index++)
            {
                var g = Groups[index];
                if (g.GroupName == LocaleName)
                    return g;
            }

            return Groups[0];
        }

        /// <summary>
        ///     Load locale data from given directory.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NotSupportedException"></exception>
        public void LoadFromDirectory(string directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            var allFiles = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var localeFile in allFiles)
            {
                var isSerialized = localeFile.EndsWith(".csvb");
                if (!localeFile.EndsWith(".csv") && !isSerialized)
                    continue;
                var fileName = Path.GetFileNameWithoutExtension(localeFile);
                var localeGroup = GetGroup(fileName);
                if (localeGroup == null)
                {
                    localeGroup = new JEMLocaleGroup {GroupName = fileName};
                    Groups.Add(localeGroup);
                }

                string fileContent;
                if (isSerialized)
                {
                    fileContent = JEMEncryptor.Load<string>(JEMEncryptor.DefaultEncryptorPassword, localeFile);
                }
                else
                {
                    fileContent = File.ReadAllText(localeFile);
                }

                var lines = fileContent.Split('\n');
                foreach (var line in lines)
                {
                    var text = line.Split(';');
                    if (text.Length < 2) continue;
                    if (localeGroup.Keys.ContainsKey(text[0]))
                    {
                        JEMLogger.LogWarning($"Key that already exists in group({localeGroup.GroupName}) " +
                                             $"has been found in next locale file ({text[0]} in file {localeFile}).");
                        continue;
                    }

                    var c = text[1];
                    // Fix new line
                    c = c.Replace(@"\n", Environment.NewLine);
                    // Fix tab
                    c = c.Replace(@"\t", "\t");
                    localeGroup.Keys.Add(text[0], c);
                }
            }
        }

        /// <summary>
        ///     Returns true, if this a currently selected locale.
        /// </summary>
        public bool IsSelected() => JEMLocale.GetSelectedLocale() == this;
    }

    /// <summary>
    ///     JEM Locale.
    ///     Implements a simple locale system.
    /// </summary>
    /// <remarks>
    ///     The system utilizes .csv files (key;content).
    ///     This files can be binary formatted so the user wont be able to freely edit the locale content.
    ///     You can serialize files using jem's binary serializer. The files should have the .csvb extension.
    /// </remarks>
    public static class JEMLocale
    {
        /// <summary>
        ///     Loads all locale files from given directory to a given localName.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LoadLocale(string localeName, string directory, bool asNew = false)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (localeName == null) throw new ArgumentNullException(nameof(localeName));
            var locale = GetLocale(localeName);
            if (asNew && locale != null)
            {
                LoadedLocales.Remove(locale);
                locale = null;
            }

            if (locale == null)
            {
                locale = new JEMLocaleData
                {
                    LocaleName = localeName
                };

                LoadedLocales.Add(locale);
            }

            locale.LoadFromDirectory(directory);
        } 

        /// <summary>
        ///     Set the locale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void SetLocale(string localeName)
        {
            if (localeName == null) throw new ArgumentNullException(nameof(localeName));
            for (var index = 0; index < LoadedLocales.Count; index++)
            {
                var locale = LoadedLocales[index];
                if (locale.LocaleName == localeName)
                {
                    SelectedLocale = index;
                    return;
                }
            }

            JEMLogger.LogWarning($"Locale of name {localeName} not exist or has not been loaded. " +
                                 "Falling back to first locale loaded (if exist).", "JEM");
            SelectedLocale = 0;
        }

        /// <summary>
        ///     Try to resolve text from currently selected locale.
        ///     If there is no locale selected, <paramref name="key"/> will be returned instead.
        /// </summary>
        /// <param name="key">Key of locale.</param>
        /// <param name="args">Arguments to format in to resolved locale data.</param>
        public static string TryResolve(string key, params object[] args) =>
            GetSelectedLocale() == null ? key : Resolve(key, args);
        
        /// <summary>
        ///     Try to resolve text from currently selected locale.
        ///     If there is no locale selected, <paramref name="key"/> will be returned instead.
        /// </summary>
        /// <param name="groupName">Group in the locale. The group is defined by locale file name.</param>
        /// <param name="key">Key of locale.</param>
        /// <param name="args">Arguments to format in to resolved locale data.</param>
        public static string TryResolve(string groupName, string key, params object[] args) =>
            GetSelectedLocale() == null ? key : Resolve(groupName, key, args);
        
        /// <summary>
        ///     Resolves text from currently selected locale.
        /// </summary>
        /// <param name="key">Key of locale.</param>
        /// <param name="args">Arguments to format in to resolved locale data.</param>
        public static string Resolve(string key, params object[] args)
        {
            var locale = GetSelectedLocale();
            if (locale == null)
                throw new NullReferenceException("Failed to resolve locale data. You need to select locale first.");

            return Resolve(locale.GetDefault(), key, args);
        }

        /// <summary>
        ///     Resolves text from currently selected locale.
        /// </summary>
        /// <param name="groupName">Group in the locale. The group is defined by locale file name.</param>
        /// <param name="key">Key of locale.</param>
        /// <param name="args">Arguments to format in to resolved locale data.</param>
        public static string Resolve(string groupName, string key, params object[] args)
        {
            if (groupName == null) throw new ArgumentNullException(nameof(groupName));
            if (key == null) throw new ArgumentNullException(nameof(key));

            var locale = GetSelectedLocale();
            if (locale == null)
                throw new NullReferenceException("Failed to resolve locale data. You need to select locale first.");

            var localeGroup = locale.GetGroup(groupName);
            if (localeGroup == null)
            {
                JEMLogger.LogError($"Failed to resolve locale data. Locale group '{groupName}' does not exists in selected locale.");
                return key;
            }

            return Resolve(localeGroup, key, args);
        }

        /// <summary>
        ///     Resolves text from currently selected locale.
        /// </summary>
        /// <param name="localeGroup">Group in the locale. The group is defined by locale file name.</param>
        /// <param name="key">Key of locale.</param>
        /// <param name="args">Arguments to format in to resolved locale data.</param>
        public static string Resolve(JEMLocaleGroup localeGroup, string key, params object[] args)
        {
            if (localeGroup == null) throw new ArgumentNullException(nameof(localeGroup));
            if (key == null) throw new ArgumentNullException(nameof(key));

            // The key is null or empty, ignore.
            if (string.IsNullOrEmpty(key))
            { return key;}

            if (!localeGroup.Keys.ContainsKey(key))
            {
                // The given key does not exists in selected locale.
                if (!ResolveKeysSilently)
                {
                    JEMLogger.LogWarning($"Locale key({key}) does not exists " +
                                         $"in target locale group({localeGroup.GroupName})", "JEM");
                }

                return key;
            }

            var data = localeGroup.Keys[key];
            var resolved = args.Length == 0 ? data : string.Format(data, args);
            resolved = OnResolvingLocale?.Invoke(resolved);
            return resolved;
        }

        /// <summary>
        ///     Gets the locale of given name.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static JEMLocaleData GetLocale(string localeName)
        {
            if (localeName == null) throw new ArgumentNullException(nameof(localeName));
            foreach (var locale in LoadedLocales)
            {
                if (locale.LocaleName == localeName)
                    return locale;
            }
            return null;
        }

        /// <summary>
        ///     Gets the currently selected locale data.
        /// </summary>
        /// <exception cref="NullReferenceException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static JEMLocaleData GetSelectedLocale() => LoadedLocales.Count == 0 ? null : LoadedLocales[SelectedLocale];

        /// <summary>
        ///     Called at the end of locale resolving.
        ///     Thanks to this event you can apply some post-processing to resolved locale text.
        /// </summary>
        public static event Func<string, string> OnResolvingLocale;

        /// <summary>
        ///     List of all loaded locales.
        /// </summary>
        public static List<JEMLocaleData> LoadedLocales { get; } = new List<JEMLocaleData>();

        /// <summary>
        ///     A index (in <see cref="LoadedLocales"/>) of selected locale.
        /// </summary>
        public static int SelectedLocale { get; private set; } = 0;

        /// <summary>
        ///     Name of default group in all locales.
        /// </summary>
        public static string DefaultLocaleGroup { get; set; } = "SYSTEM";

        /// <summary>
        ///     If true, <see cref="JEMLogger"/> will not throw any warning message when target locale key was not found.
        /// </summary>
        public static bool ResolveKeysSilently { get; set; } = false;
    }
}