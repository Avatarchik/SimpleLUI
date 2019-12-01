//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// TODO: JEMBetterEditor need major refactor work.

using JEM.Core.Extension;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JEM.UnityEditor
{
    public static partial class JEMBetterEditor
    {
        /// <summary>
        ///     Better list is replace for unity's array/list property control.
        ///     Implements custom item editor and adds ability to move items in list.
        /// </summary>
        public static List<T1> List<T1, T2>([NotNull] List<T1> list, [NotNull] T2 target, ref Vector2 scroll, Func<T1, T1> onDraw) where T2 : Object =>
            List<T1, T2>(list, target, ref scroll, onDraw, new JEMBetterEditorStyle("Better List", typeof(T2).Name));
        
        /// <summary>
        ///     Better list is replace for unity's array/list property control.
        ///     Implements custom item editor and adds ability to move items in list.
        /// </summary>
        public static List<T1> List<T1, T2>([NotNull] List<T1> list, [NotNull] T2 target, ref Vector2 scroll, Func<T1, T1> onDraw, JEMBetterEditorStyle style) where T2 : Object =>
            List<T1, T2>(list, target, ref scroll, new JEMBetterEditorEvents<T1>(onDraw), style);
        
        /// <summary>
        ///     Better list is replace for unity's array/list property control.
        ///     Implements custom item editor and adds ability to move items in list.
        /// </summary>
        public static List<T1> List<T1, T2>([NotNull] List<T1> list, [NotNull] T2 target, ref Vector2 scroll, JEMBetterEditorEvents<T1> @event) where T2 : Object =>
            List<T1, T2>(list, target, ref scroll, @event, new JEMBetterEditorStyle("Better List", typeof(T2).Name));
        
        /// <summary>
        ///     Better list is replace for unity's array/list property control.
        ///     Implements custom item editor and adds ability to move items in list.
        /// </summary>
        public static List<T1> List<T1, T2>([NotNull] List<T1> list, [NotNull] T2 target, ref Vector2 scroll, JEMBetterEditorEvents<T1> @event, JEMBetterEditorStyle style) where T2 : Object
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (target == null) throw new ArgumentNullException(nameof(target));
            if (@event.OnDraw == null)
            {
                EditorGUILayout.HelpBox("BetterList error. Events OnDraw must be set.", MessageType.Error);
                return list;
            }

            var targetName = $"{style.EditorName}.{typeof(T1)}";
            var scroll1 = scroll;
            DrawBetterEditorGroup(style, () =>
            {
                var newList = list;
                EditorGUI.BeginChangeCheck();
                scroll1 = EditorGUILayout.BeginScrollView(scroll1);
                {
                    for (var index = 0; index < newList.Count; index++)
                    {
                        BeginFixedIndent();
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                var itemName = $"{targetName}.Item.{index}";
                                var headerIndex = style.ItemFixedIndex ? index + 1 : index;
                                var headerText = $"{style.ItemPrefix} #{headerIndex}";
                                @event.OnPreDraw?.Invoke(newList[index]);

                                EditorGUILayout.BeginHorizontal();
                                bool shouldShow = EditorPrefs.GetBool(itemName);
                                bool show = EditorGUILayout.Foldout(shouldShow, headerText);
                                var showMenu = GUILayout.Button(string.Empty, EditorStyles.foldoutHeaderIcon,
                                    GUILayout.Width(15));
                                if (showMenu)
                                {
                                    var menu = new GenericMenu();
                                    if (index - 1 >= 0)
                                    {
                                        var index1 = index;
                                        menu.AddItem(new GUIContent("Move up"), false, () =>
                                        {
                                            var obj = newList[index1];
                                            newList.RemoveAt(index1);
                                            newList.Insert(index1 - 1, obj);
                                            GUI.FocusControl(null);
                                        });
                                    }

                                    if (index + 1 < newList.Count)
                                    {
                                        var index1 = index;
                                        menu.AddItem(new GUIContent("Move down"), false, () =>
                                        {
                                            var obj = newList[index1];
                                            newList.RemoveAt(index1);
                                            newList.Insert(index1 + 1, obj);
                                            GUI.FocusControl(null);
                                        });
                                    }

                                    var index2 = index;
                                    menu.AddItem(new GUIContent("Remove"), false, () =>
                                    {
                                        var @continue = true;
                                        if (Event.current == null || Event.current.keyCode != KeyCode.LeftControl)
                                            if (!EditorUtility.DisplayDialog("Continue?",
                                                $"Do you really want to delete {style.ItemPrefix} #{index2}?", "Yes",
                                                "No"))
                                                @continue = false;

                                        if (@continue)
                                        {
                                            if (@event.OnRemove != null)
                                                @event.OnRemove.Invoke(newList[index2]);
                                            else
                                            {
                                                newList.RemoveAt(index2);
                                            }

                                            index--;
                                            GUI.FocusControl(null);
                                        }
                                    });
                                    menu.ShowAsContext();
                                }
                                EditorGUILayout.EndHorizontal();

                                if (show)
                                {
                                    BeginFixedIndent();
                                    {
                                        EditorGUILayout.BeginVertical();
                                        newList[index] = @event.OnDraw.Invoke(newList[index]);
                                        EditorGUILayout.Space();
                                        EditorGUILayout.EndVertical();
                                    }
                                    EndFixedIndent();
                                }

                                if (show != shouldShow)
                                {
                                    EditorPrefs.SetBool(itemName, show);
                                }
                                @event.OnPostDraw?.Invoke(newList[index]);
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EndFixedIndent();
                    }

                    BeginFixedIndent(2, true);
                    EditorGUILayout.BeginHorizontal(GUILayout.Height(14));
                    {
                        // GUILayout.FlexibleSpace();
                        GUI.color = Color.green;
                        if (GUILayout.Button($"Add new {style.DrawName}", EditorStyles.miniButton, GUILayout.Height(14)))
                        {
                            GUI.FocusControl(null);
                            if (@event.OnAdd != null)
                                @event.OnAdd.Invoke();
                            else
                            {
                                var itemName = $"{targetName}.Item.{newList.Count}";
                                EditorPrefs.SetBool(itemName, true);
                                var newInstance = FastObjectFactory<T1>.Instance();
                                newList.Add(newInstance);
                            }
                        }
                        GUILayout.FlexibleSpace();
                        GUI.color = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                    EndFixedIndent(2);
                }
                EditorGUILayout.EndScrollView();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(target, $"{targetName}.Change");
                    list = newList;
                }
            }, @event.OnDrawMenu);
            scroll = scroll1;
            return list;
        }

        /// <summary>
        ///     Content list is a simpler 'BetterList' version without ability to move objects and is designed to draw all items at once.
        /// </summary>
        public static List<T1> ExperimentalList<T1, T2>([NotNull] List<T1> list, [NotNull] T2 target, Func<T1, T1> onDraw) =>
            ExperimentalList<T1, T2>(list, target, onDraw, new JEMBetterEditorStyle("Content List", typeof(T2).Name));

        /// <summary>
        ///     Content list is a simpler 'BetterList' version without ability to move objects and is designed to draw all items at once.
        /// </summary>
        public static List<T1> ExperimentalList<T1, T2>([NotNull] List<T1> list, [NotNull] T2 target, Func<T1, T1> onDraw, JEMBetterEditorStyle style) =>
            ExperimentalList<T1, T2>(list, target, new JEMBetterEditorEvents<T1>(onDraw), style);

        /// <summary>
        ///     Content list is a simpler 'BetterList' version without ability to move objects and is designed to draw all items at once.
        /// </summary>
        public static List<T1> ExperimentalList<T1, T2>([NotNull] List<T1> list, [NotNull] T2 target, JEMBetterEditorEvents<T1> @event) =>
            ExperimentalList<T1, T2>(list, target, @event, new JEMBetterEditorStyle("Content List", typeof(T2).Name));

        /// <summary>
        ///     Content list is a simpler 'BetterList' version without ability to move objects and is designed to draw all items at once.
        /// </summary>
        public static List<T1> ExperimentalList<T1, T2>([NotNull] List<T1> list, [NotNull] T2 target, JEMBetterEditorEvents<T1> @event, JEMBetterEditorStyle style)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (@event.OnDraw == null)
            {
                EditorGUILayout.HelpBox("BetterList error. Events OnDraw must be set.", MessageType.Error);
                return list;
            }

            DrawBetterEditorGroup(style, () =>
            {
                var prevEnable = GUI.enabled;
                if (style.Readonly)
                    GUI.enabled = false;

                for (var index = 0; index < list.Count; index++)
                {
                    EditorGUILayout.BeginHorizontal("box");
                    {
                        list[index] = @event.OnDraw(list[index]);
                        GUILayout.FlexibleSpace();
                        GUI.color = Color.red;
                        if (GUILayout.Button("Remove", GUILayout.Width(70)))
                        {
                            if (@event.OnRemove != null)
                                @event.OnRemove(list[index]);
                            else
                            {
                                list.RemoveAt(index);
                            }

                            index--;
                        }

                        GUI.color = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal("box");
                {
                    GUILayout.FlexibleSpace();
                    GUI.color = Color.green;
                    if (GUILayout.Button("New", GUILayout.Width(70)))
                    {
                        if (@event.OnAdd != null)
                            @event.OnAdd.Invoke();
                        else
                        {
                            var newInstance = FastObjectFactory<T1>.Instance();
                            list.Add(newInstance);
                        }
                    }

                    GUI.color = Color.white;
                }
                EditorGUILayout.EndHorizontal();

                GUI.enabled = prevEnable;
            }, @event.OnDrawMenu);

            return list;
        }
    }
}
