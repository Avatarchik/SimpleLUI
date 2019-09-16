//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace JEM.UnityEditor
{
    /// <inheritdoc />
    /// <summary>
    ///     This class allows to draw new Unity Editor window by just calling JEMToolWindow.ShowWindow(title, onGUI).
    ///     Only one JEMToolWindow can be drawn at the time.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class JEMToolWindow : EditorWindow
    {
        /// <summary>
        ///     GUI draw delegate.
        /// </summary>
        public delegate void DrawGUI();

        private DrawGUI OnDrawGUI;

        private void OnGUI()
        {
            if (OnDrawGUI == null)
            {
                if (!CloseWindow())
                {
                    Close();
                }

                return;
            }

            if (focusedWindow != this)
                CloseWindow();

            OnDrawGUI.Invoke();
        }

        /// <summary>
        ///     Show tool window.
        /// </summary>
        public static void ShowWindow(string title, [NotNull] DrawGUI onDrawGui)
        {
            if (onDrawGui == null) throw new ArgumentNullException(nameof(onDrawGui));
            ShowWindow(title, onDrawGui, default(Rect));
        }

        /// <summary>
        ///     Show tool window.
        /// </summary>
        public static void ShowWindow(string title, [NotNull] DrawGUI onDrawGui, Rect position)
        {
            if (Current != null)
                Current.Focus();
            else
                Current = (JEMToolWindow) GetWindow(typeof(JEMToolWindow), true, title, true);

            if (!position.Equals(default(Rect)))
                Current.position = position;

            Current.titleContent = new GUIContent(title);
            Current.OnDrawGUI = onDrawGui ?? throw new ArgumentNullException(nameof(onDrawGui));
            Current.ShowTab();
        }

        /// <summary>
        ///     Closes current tool window instance.
        /// </summary>
        public static bool CloseWindow()
        {
            if (Current == null) return false;

            Current.Close();
            Current = null;
            return true;
        }

        private static JEMToolWindow Current;
    }
}