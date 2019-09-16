//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Text;
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor.Locale
{
    public class JEMLocaleWindowKeyEdit : EditorWindow
    {
        private JEMLocaleGroup _group;
        private string _keyName;

        private string _keyRename;
        private string _oldKey;

        private string _value;
        private bool _keyIsChanging;

        private void OnGUI()
        {
            if (_group == null || !_group.Keys.ContainsKey(_keyName))
            {
                // Group is not set (probably recompile), close!
                Close();
                return;
            }

            GUILayout.Label("Edit Key of " + _group.GroupName, EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (_keyIsChanging)
            {
                _keyRename = EditorGUILayout.TextField("Key", _keyRename);
                if (GUILayout.Button("Apply", EditorStyles.miniButton, GUILayout.Width(40)))
                {
                    if (_keyRename== _oldKey)
                    {
                        _keyIsChanging = false;
                    }
                    else if (JEMLocaleEditor.IsKeyNameFree(_group.GroupName, _keyRename))
                    {
                        _keyName = _keyRename;
                        JEMLocaleEditor.RenameKey(_group.GroupName, _oldKey, _keyRename);
                        _keyIsChanging = false;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Oops." , $"Key of name {_keyRename} in " +
                                                              $"group {_group.GroupName} already exists.", "Ok");
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("Key", _keyName);
                if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(40)))
                {
                    _oldKey = _keyName;
                    _keyRename = _keyName;
                    _keyIsChanging = true;
                }
            }

            EditorGUILayout.EndHorizontal();

            if (!_keyIsChanging)
            {
                _value = _group.Keys[_keyName];
                _value = JEMLocaleEditor.RestoreSpecialCharacters(_value);
            }

            JEMBetterEditor.DrawProperty("Content", () =>
            {
                _value = EditorGUILayout.TextArea(_value, GUILayout.ExpandHeight(true));
            });

            if (!_keyIsChanging)
            {
                _value = JEMLocaleEditor.StoreSpecialCharacters(_value);
                _group.Keys[_keyName] = _value;
            }
        }

        public static void ShowWindow(JEMLocaleGroup group, string keyName)
        {
            var window = GetWindow<JEMLocaleWindowKeyEdit>(true, "Edit Locale Key", true);
            window._group = group;
            window._keyName = keyName;
            window.ShowPopup();
        }
    }
}
