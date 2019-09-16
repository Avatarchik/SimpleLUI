//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Configuration;
using JEM.Core.IO;
using JEM.Core.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

namespace JEM.UnityEditor.Locale
{
    public static class JEMLocaleEditor
    {
        /// <summary>
        ///     Imports locale from given directory.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void ImportLocale([NotNull] string directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            var directories = Directory.GetDirectories(directory, "*.*", SearchOption.TopDirectoryOnly);
            foreach (var dir in directories)
            {
                var dirName = Path.GetFileNameWithoutExtension(dir);
                var locale = new JEMLocaleData
                {
                    LocaleName = dirName
                };

                var files = Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var f in files)
                {
                    var fileName = Path.GetFileNameWithoutExtension(f);
                    if (locale.GetGroup(fileName) != null)
                    {
                        Debug.LogWarning($"Group of name {fileName} already exist so file {f} will be ignored.");
                        continue;
                    }

                    string str = string.Empty;
                    if (f.EndsWith(".csvb"))
                    {
                        str = JEMEncryptor.Load<string>(JEMEncryptor.DefaultEncryptorPassword, f);
                    }
                    else if (f.EndsWith(".csv"))
                    {
                        str = File.ReadAllText(f);
                    }
                    else continue;
   
                    var group = new JEMLocaleGroup
                    {
                        GroupName = fileName
                    };

                    var lines = str.Split('\n');
                    for (var index = 0; index < lines.Length; index++)
                    {
                        var l = lines[index];
                        var s = l.Split(';');
                        if (s.Length >= 2)
                        {
                            group.Keys.Add(s[0], s[1]);
                        }
                        else Debug.LogError($"Lane '{l}' is broken and can't be imported from target file.");
                    }

                    locale.Groups.Add(group);
                }

                LocaleLoaded.Add(locale);
            }
        }

        /// <summary>
        ///     Load locale data.
        /// </summary>
        public static void LoadLocale()
        {
            RegenerateDirectory();

            LocaleLoaded.Clear();

            var allFiles = Directory.GetFiles(JEMLocaleDirectory, "*.json", SearchOption.TopDirectoryOnly);
            foreach (var f in allFiles)
            {
                var localeData = JsonConvert.DeserializeObject<JEMLocaleData>(File.ReadAllText(f));
                if (localeData == null)
                    continue;

                LocaleLoaded.Add(localeData);
            }

            _defaultLocaleName = new SavedString($"{nameof(JEMLocaleEditor)}.DefaultLocaleName", string.Empty);
        }

        /// <summary>
        ///     Save locale data.
        /// </summary>
        public static void SaveLocale()
        {
            RegenerateDirectory(true); 

            foreach (var locale in LocaleLoaded)
            {
                var serializedJson = JsonConvert.SerializeObject(locale, Formatting.Indented);
                if (serializedJson == null)
                    continue;

                var filePath = $"{JEMLocaleDirectory}\\{locale.LocaleName}.json";
                File.WriteAllText(filePath, serializedJson);
            }
        }

        /// <summary>
        ///     Export locale data in to target directory.
        /// </summary>
        public static void ExportLocale()
        {
            RegenerateDirectory();

            foreach (var locale in LocaleLoaded)
            {
                var localeDirectory = $"{ExportLocaleDirectory}\\{locale.LocaleName}";
                if (!Directory.Exists(localeDirectory))
                {
                    Directory.CreateDirectory(localeDirectory);
                }

                foreach (var group in locale.Groups)
                {
                    var groupFile = $"{localeDirectory}\\{group.GroupName}.csv";
                    var str = string.Empty;
                    foreach (var k in group.Keys)
                    {
                        if (!string.IsNullOrEmpty(str))
                            str += "\n";
                        str += k.Key + ";" + k.Value;
                    }

                    if (JEMLocaleEditorConfiguration.Configuration.ExportSerialized)
                    {
                        groupFile += "b";
                        JEMEncryptor.Write(JEMEncryptor.DefaultEncryptorPassword, groupFile, str);
                    }
                    else File.WriteAllText(groupFile, str);
                }
            }

            Process.Start(ExportLocaleDirectory);
        }

        /// <summary>
        ///     Regenerates locale directories.
        /// </summary>
        public static void RegenerateDirectory(bool createNewJEM = false)
        {
            try
            {
                if (createNewJEM)
                {
                    Directory.Delete(JEMLocaleDirectory, true);
                }

                if (!Directory.Exists(JEMLocaleDirectory))
                {
                    Directory.CreateDirectory(JEMLocaleDirectory);
                }

                if (!Directory.Exists(ExportLocaleDirectory))
                {
                    Directory.CreateDirectory(ExportLocaleDirectory);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        ///     Adds new locale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void AddNewLocale([NotNull] string localeName, JEMLocaleData copy = null)
        {
            if (localeName == null) throw new ArgumentNullException(nameof(localeName));

            // Create new locale
            var newLocale = new JEMLocaleData {LocaleName = localeName};

            // Copy data
            if (copy != null)
            {
                foreach (var group in copy.Groups)
                {
                    var newGroup = new JEMLocaleGroup {GroupName = group.GroupName};
                    foreach (var key in group.Keys)
                    {
                        newGroup.Keys.Add(key.Key, key.Value);
                    }

                    newLocale.Groups.Add(newGroup);
                }
            }

            LocaleLoaded.Add(newLocale);
        }

        /// <summary>
        ///     Removes given locale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RemoveLocale([NotNull] JEMLocaleData locale)
        {
            if (locale == null) throw new ArgumentNullException(nameof(locale));
            if (!LocaleLoaded.Contains(locale))
                return;

            LocaleLoaded.Remove(locale);
        }

        /// <summary>
        ///     Adds a new group to all locales loaded.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void AddNewGroup(string localeGroup)
        {
            int i = 0;
            for (var index = 0; index < LocaleLoaded.Count; index++)
            {
                var locale = LocaleLoaded[index];
                if (locale.GetGroup(localeGroup) != null)
                    continue;

                locale.Groups.Add(new JEMLocaleGroup
                {
                    GroupName = localeGroup
                });
                i++;
            }

            Debug.Log($"Locale group {localeGroup} added to {i} locales.");
        }

        /// <summary>
        ///     Removes a group of given name from all the locales loaded.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RemoveGroup(string localeGroup)
        {
            if (localeGroup == null) throw new ArgumentNullException(nameof(localeGroup));
            int i = 0;
            for (var index = 0; index < LocaleLoaded.Count; index++)
            {
                var locale = LocaleLoaded[index];
                var group = locale.GetGroup(localeGroup);
                if (@group == null)
                    continue;

                locale.Groups.Remove(@group);
                i++;
            }

            Debug.Log($"Locale group {localeGroup} removed from {i} locales.");
        }

        /// <summary>
        ///     Try to rename selected group name.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RenameGroup([NotNull] string oldGroupName, [NotNull] string newGroupName)
        {
            if (oldGroupName == null) throw new ArgumentNullException(nameof(oldGroupName));
            if (newGroupName == null) throw new ArgumentNullException(nameof(newGroupName));
            if (oldGroupName == newGroupName) return;

            for (var index = 0; index < LocaleLoaded.Count; index++)
            {
                var locale = LocaleLoaded[index];
                var group = locale.GetGroup(oldGroupName);
                @group.GroupName = newGroupName;
            }
        }

        /// <summary>
        ///     Checks if given group name is free.
        /// </summary>
        /// <remarks>
        ///     Returns false if any of loaded locales contains group of given name.
        /// </remarks>
        /// <exception cref="ArgumentNullException"/>
        public static bool IsGroupNameFree([NotNull] string groupName)
        {
            if (groupName == null) throw new ArgumentNullException(nameof(groupName));
            for (var index = 0; index < LocaleLoaded.Count; index++)
            {
                var locale = LocaleLoaded[index];
                if (locale.GetGroup(groupName) != null)
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     Adds new locale key of locale group to all loaded locales.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void AddNewKey([NotNull] string localeGroup, [NotNull] string localeKey)
        {
            if (localeGroup == null) throw new ArgumentNullException(nameof(localeGroup));
            if (localeKey == null) throw new ArgumentNullException(nameof(localeKey));

            int i = 0;
            for (var index = 0; index < LocaleLoaded.Count; index++)
            {
                var locale = LocaleLoaded[index];
                var group = locale.GetGroup(localeGroup);
                if (@group == null)
                {
                    @group = new JEMLocaleGroup {GroupName = localeGroup};
                    if (DefaultLocale != null)
                    {
                        var defaultGroup = DefaultLocale.GetGroup(localeGroup);
                        if (defaultGroup != null)
                        {
                            foreach (var key in defaultGroup.Keys)
                            {
                                @group.Keys.Add(key.Key, key.Value);
                            }
                        }
                    }

                    locale.Groups.Add(@group);
                    i++;
                }

                @group.Keys.Add(localeKey, "You key content goes here.");
            }

            Debug.Log($"Locale key {localeKey} added to group {localeGroup}. Total of {i} groups has been modified.");
        }

        /// <summary>
        ///     Removes locale key from given locale group from all loaded locales.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RemoveKey([NotNull] string localeGroup, [NotNull] string localeKey)
        {
            if (localeGroup == null) throw new ArgumentNullException(nameof(localeGroup));
            if (localeKey == null) throw new ArgumentNullException(nameof(localeKey));

            int i = 0;
            for (var index = 0; index < LocaleLoaded.Count; index++)
            {
                var locale = LocaleLoaded[index];
                var group = locale.GetGroup(localeGroup);
                if (@group == null)
                {
                    if (DefaultLocale != null)
                    {
                        @group = new JEMLocaleGroup {GroupName = localeGroup};
                        var defaultGroup = DefaultLocale.GetGroup(localeGroup);
                        if (defaultGroup != null)
                        {
                            foreach (var key in defaultGroup.Keys)
                            {
                                @group.Keys.Add(key.Key, key.Value);
                            }
                        }

                        locale.Groups.Add(@group);
                    }
                    else continue;
                }

                if (@group.Keys.ContainsKey(localeKey))
                {
                    @group.Keys.Remove(localeKey);
                }
            }

            Debug.Log($"Locale key {localeKey} removed from group {localeGroup}. Total of {i} groups has been modified.");
        }

        /// <summary>
        ///     Try to rename a name of key in selected group.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void RenameKey([NotNull] string groupName, [NotNull] string oldKeyName, [NotNull] string newKeyName)
        {
            if (groupName == null) throw new ArgumentNullException(nameof(groupName));
            if (oldKeyName == null) throw new ArgumentNullException(nameof(oldKeyName));
            if (newKeyName == null) throw new ArgumentNullException(nameof(newKeyName));
            if (oldKeyName == newKeyName) return;

            for (var index = 0; index < LocaleLoaded.Count; index++)
            {
                var locale = LocaleLoaded[index];
                var localeGroup = locale.GetGroup(groupName);
                if (localeGroup == null)
                    continue;

                if (localeGroup.Keys.ContainsKey(newKeyName))
                {
                    // The new key name already exist in group...
                    continue;
                }

                if (localeGroup.Keys.ContainsKey(oldKeyName))
                {
                    var content = localeGroup.Keys[oldKeyName];
                    localeGroup.Keys.Add(newKeyName, content);
                    localeGroup.Keys.Remove(oldKeyName);
                }
            }
        }

        /// <summary>
        ///     Checks if given name of key in selected group is free.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static bool IsKeyNameFree([NotNull] string groupName, [NotNull] string keyName)
        {
            if (groupName == null) throw new ArgumentNullException(nameof(groupName));
            if (keyName == null) throw new ArgumentNullException(nameof(keyName));

            for (var index = 0; index < LocaleLoaded.Count; index++)
            {
                var locale = LocaleLoaded[index];
                var group = locale.GetGroup(groupName);
                if (@group != null)
                {
                    if (@group.Keys.ContainsKey(keyName))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Sets the default locale used by editor.
        /// </summary>
        /// <remarks>
        ///     The default locale is used to for ex. initialize missing groups in other locales.
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetDefaultLocale([NotNull] JEMLocaleData locale)
        {
            _defaultLocale = locale ?? throw new ArgumentNullException(nameof(locale));
            _defaultLocaleName.value = locale.LocaleName;
        }

        public static string StoreSpecialCharacters(string str)
        {
            str = str.Replace("\n", @"\n");
            str = str.Replace("\t", @"\t");
            return str;
        }

        public static string RestoreSpecialCharacters(string str)
        {
            str = str.Replace(@"\n", "\n");
            str = str.Replace(@"\t", "\t");
            return str;
        }

        /// <summary>
        ///     A reference to the default locale of editor.
        /// </summary>
        public static JEMLocaleData DefaultLocale
        {
            get
            {
                if (_defaultLocale == null)
                {
                    if (LocaleLoaded.Count == 0)
                        return null;

                    if (string.IsNullOrEmpty(_defaultLocaleName.value))
                    {
                        _defaultLocale = LocaleLoaded[0];
                        _defaultLocaleName.value = LocaleLoaded[0].LocaleName;                  
                    }
                    else
                    {
                        for (var index = 0; index < LocaleLoaded.Count; index++)
                        {
                            var l = LocaleLoaded[index];
                            if (l.LocaleName != _defaultLocaleName.value) continue;
                            _defaultLocale = l;
                            break;
                        }
                    }
                }

                return _defaultLocale;
            }
        }

        /// <summary>
        ///     List of all locale loaded in to editor.
        /// </summary>
        public static List<JEMLocaleData> LocaleLoaded { get; } = new List<JEMLocaleData>();

        /// <summary>
        ///     Path to the JEM's locale directory.
        /// </summary>
        public static string JEMLocaleDirectory => $"{JEMConfiguration.CurrentDirectory}\\JEM\\Locale";

        /// <summary>
        ///     Path to the locale export directory (final .csv files).
        /// </summary>
        public static string ExportLocaleDirectory => $"{JEMConfiguration.CurrentDirectory}\\{JEMLocaleEditorConfiguration.Configuration.LocaleEditorDirectory}";

        private static JEMLocaleData _defaultLocale;
        private static SavedString _defaultLocaleName;
    }
}
