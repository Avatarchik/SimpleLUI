//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Text;
using JEM.UnityEditor.Configuration;
using System;
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor.Locale
{
    /// <inheritdoc />
    /// <summary>
    ///     The JEM Locale Editor Window.
    /// </summary>
    public class JEMLocaleWindow : EditorWindow
    {
        private SavedVector2 _localePosition;
        private SavedVector2 _localeKeysPosition;
        private SavedInt _selectedLocale;

        private void OnEnable()
        {
            _localePosition = new SavedVector2($"{nameof(JEMLocaleWindow)}.LocalePosition", Vector2.zero);
            _localeKeysPosition = new SavedVector2($"{nameof(JEMLocaleWindow)}.LocaleKeysPosition", Vector2.zero);
            _selectedLocale = new SavedInt($"{nameof(JEMLocaleWindow)}.SelectedLocale", 0);

            // Load editor configuration
            JEMEditorConfiguration.Load();
            JEMLocaleEditorConfiguration.TryLoad();

            // Regenerate directories
            JEMLocaleEditor.RegenerateDirectory();

            // Load all locales
            JEMLocaleEditor.LoadLocale();
        }

        private void OnGUI()
        {
            //
            // DRAW: General Options
            //
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(140));
            {
                GUILayout.Label("General Options", EditorStyles.boldLabel);
                if (GUILayout.Button("Add New Locale", GUILayout.Height(25)))
                {
                    JEMLocaleEditor.AddNewLocale($"New locale{JEMLocaleEditor.LocaleLoaded.Count+1}", JEMLocaleEditor.DefaultLocale);
                }

                if (GUILayout.Button("Add New Group", GUILayout.Height(25)))
                {
                    JEMToolWindow.ShowWindow("Add Group", () =>
                    {
                        NewGroupName = EditorGUILayout.TextField("Group Name", NewGroupName);
                        JEMBetterEditor.DrawProperty(" ", () =>
                        {
                            if (GUILayout.Button("Add"))
                            {
                                JEMLocaleEditor.AddNewGroup(NewGroupName);
                                JEMToolWindow.CloseWindow();
                            }
                        });
                    });
                }

                GUILayout.FlexibleSpace();
                GUILayout.Label("File Options", EditorStyles.boldLabel);
                if (GUILayout.Button("Import Locale", GUILayout.Height(25)))
                {
                    var directory = EditorUtility.OpenFolderPanel("Select locale to import", Environment.CurrentDirectory, string.Empty);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        JEMLocaleEditor.ImportLocale(directory);
                    }
                }

                if (GUILayout.Button("Reload All Locales", GUILayout.Height(25)))
                {
                    JEMLocaleEditor.LoadLocale();
                }

                if (GUILayout.Button("Save All Locales", GUILayout.Height(25)))
                {
                    JEMLocaleEditor.SaveLocale();
                }

                if (GUILayout.Button("Export All Locales", GUILayout.Height(25)))
                {
                    JEMLocaleEditor.ExportLocale();
                }
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(4);

            //
            // DRAW: Locale List
            //
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(120));
            {
                GUILayout.Label("Locales:", EditorStyles.boldLabel);
                _localePosition.value = EditorGUILayout.BeginScrollView(_localePosition.value);
                for (var index = 0; index < JEMLocaleEditor.LocaleLoaded.Count; index++)
                {
                    var locale = JEMLocaleEditor.LocaleLoaded[index];
                    var localeSelected = locale.IsSelected();

                    if (localeSelected)
                    {
                        GUI.enabled = false;
                        GUI.color = new Color(0.3f, 0.3f, 1.0f, 1.0f);
                    }

                    if (GUILayout.Button(locale.LocaleName, GUILayout.Height(22), GUILayout.ExpandWidth(true)))
                    {
                        _selectedLocale.value = index;
                    }

                    if (localeSelected)
                    {
                        GUI.enabled = true;
                        GUI.color = Color.white;
                    }
                }

                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            //
            // DRAW: Selected locale content
            //
            var selectedLocale = GetSelectedLocale();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                if (selectedLocale == null)
                {
                    EditorGUILayout.HelpBox("No locale selected.", MessageType.Info, true);
                }
                else
                {
                    GUILayout.Label(selectedLocale.LocaleName, EditorStyles.boldLabel);
                    _localeKeysPosition.value = EditorGUILayout.BeginScrollView(_localeKeysPosition.value);
                    for (var index = 0; index < selectedLocale.Groups.Count; index++)
                    {
                        var localeGroup = selectedLocale.Groups[index];
                        var drawGroup = new SavedBool($"{nameof(JEMLocaleWindow)}.Group.{localeGroup.GroupName}.Draw", false);

                        void GroupMenu(Rect pos)
                        {
                            var menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Rename Group"), false, () =>
                            {
                                RenameGroupTarget = localeGroup.GroupName;
                                RenameGroupName = localeGroup.GroupName;
                                JEMToolWindow.ShowWindow("Rename", () =>
                                {
                                    RenameGroupName = EditorGUILayout.TextField("Group Name", RenameGroupName);
                                    JEMBetterEditor.DrawProperty(" ", () =>
                                    {
                                        if (GUILayout.Button("Apply"))
                                        {
                                            if (JEMLocaleEditor.IsGroupNameFree(RenameGroupName))
                                            {
                                                JEMLocaleEditor.RenameGroup(RenameGroupTarget, RenameGroupName);
                                                JEMToolWindow.CloseWindow();
                                            }
                                            else
                                            {
                                                EditorUtility.DisplayDialog("Oops.", $"Group of name {RenameGroupName} already exists!", "Ok");
                                            }
                                        }
                                    });
                                });
                            });
                            menu.AddItem(new GUIContent("Remove Group"), false, () =>
                            {
                                var delete = EditorUtility.DisplayDialog("Delete?",
                                    "Do you really want to delete group " + localeGroup.GroupName + "?", "Yes", "No");
                                if (delete)
                                {
                                    JEMLocaleEditor.RemoveGroup(localeGroup.GroupName);
                                }
                            });
                            menu.DropDown(pos);
                        }

                        drawGroup.value = EditorGUILayout.BeginFoldoutHeaderGroup(drawGroup.value, localeGroup.GroupName, menuAction:GroupMenu);
                        if (drawGroup.value)
                        {
                            EditorGUI.indentLevel++;
                            string keyDelete = string.Empty;
                            foreach (var key in localeGroup.Keys)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUIUtility.labelWidth += 100;
                                EditorGUILayout.LabelField(key.Key, key.Value);
                                EditorGUIUtility.labelWidth -= 100;
                                if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(40)))
                                {
                                    JEMLocaleWindowKeyEdit.ShowWindow(localeGroup, key.Key);
                                }

                                if (GUILayout.Button("Delete", EditorStyles.miniButton, GUILayout.Width(45)))
                                {
                                    var delete = EditorUtility.DisplayDialog("Delete?", $"Are you sure you want delete key {key.Key}" +
                                                                                        $" from group {localeGroup.GroupName}?", "Yes", "No");
                                    if (delete)
                                    {
                                        keyDelete = key.Key;
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }

                            if (!string.IsNullOrEmpty(keyDelete))
                            {
                                JEMLocaleEditor.RemoveKey(localeGroup.GroupName, keyDelete);
                            }

                            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(" ", " ");
                            if (GUILayout.Button("Add New", EditorStyles.miniButton, GUILayout.Width(90)))
                            {
                                JEMToolWindow.ShowWindow("Add Key", () =>
                                    {
                                        NewKeyName = EditorGUILayout.TextField("Key Name", NewKeyName);
                                        JEMBetterEditor.DrawProperty(" ", () =>
                                        {
                                            if (GUILayout.Button("Add"))
                                            {
                                                if (JEMLocaleEditor.IsKeyNameFree(localeGroup.GroupName, NewKeyName))
                                                {
                                                    JEMLocaleEditor.AddNewKey(localeGroup.GroupName, NewKeyName);
                                                    JEMToolWindow.CloseWindow();
                                                }
                                                else
                                                {
                                                    EditorUtility.DisplayDialog("Oops.", "Key of name " + NewKeyName + " already exist in group of name " + localeGroup.GroupName, "Ok");
                                                }
                                            }
                                        });
                                    });
                            }

                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel--;
                            EditorGUILayout.Space();
                        }
                        EditorGUILayout.EndFoldoutHeaderGroup();
                    }

                    EditorGUILayout.EndScrollView();
                }
            }
            EditorGUILayout.EndVertical();

            //
            // DRAW: Selected locale options
            //
            if (selectedLocale != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(140));
                {
                    GUILayout.Label("Locale Options", EditorStyles.boldLabel);
                    selectedLocale.LocaleName = EditorGUILayout.TextField(selectedLocale.LocaleName);
                    if (JEMLocaleEditor.DefaultLocale == selectedLocale)
                    {
                        EditorGUILayout.HelpBox("Default Locale", MessageType.Info, true);
                    }
                    else
                    {
                        if (GUILayout.Button("Set As Default", GUILayout.Height(25)))
                        {
                            JEMLocaleEditor.SetDefaultLocale(selectedLocale);
                        }
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Danger Zone", EditorStyles.boldLabel);
                    if (GUILayout.Button("Delete Locale", GUILayout.Height(25)))
                    {
                        var delete = EditorUtility.DisplayDialog("Delete?",
                            $"Are you sure you want do delete {selectedLocale.LocaleName} locale?", "Yes", "No");
                        if (delete)
                        {
                            JEMLocaleEditor.RemoveLocale(selectedLocale);
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
        }

        private JEMLocaleData GetSelectedLocale()
        {
            if (JEMLocaleEditor.LocaleLoaded.Count == 0)
            {
                _selectedLocale.value = 0;
                return null;
            }

            if (_selectedLocale.value >= JEMLocaleEditor.LocaleLoaded.Count)
            {
                _selectedLocale.value = JEMLocaleEditor.LocaleLoaded.Count - 1;
            }

            return JEMLocaleEditor.LocaleLoaded[_selectedLocale.value];
        }

        [MenuItem("JEM/JEM Locale Editor")]
        public static void ShowWindow()
        {
            GetWindow<JEMLocaleWindow>(true, "JEM Locale Editor", true);
        }

        private static string RenameGroupTarget;
        private static string RenameGroupName;
        private static string NewGroupName = "New Group";
        private static string NewKeyName = "New Key";
    }
}
