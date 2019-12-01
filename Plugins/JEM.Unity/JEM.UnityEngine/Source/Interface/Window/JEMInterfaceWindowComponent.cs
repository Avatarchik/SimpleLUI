//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine.Interface.Window
{
    /// <inheritdoc />
    /// <summary>
    ///     An base of every <see cref="T:JEM.UnityEngine.Interface.Window.JEMInterfaceWindow" /> component.
    /// </summary>
    public abstract class JEMInterfaceWindowComponent : MonoBehaviour
    {
        /// <summary>
        ///     <see cref="JEMInterfaceWindow"/> component reference.
        /// </summary>
        [Header("Base Component Settings")]
        public JEMInterfaceWindow Window;

        private void Awake()
        {
            if (Window == null)
            {
                Window = GetComponentInParent<JEMInterfaceWindow>();
            }

            Debug.Assert(Window != null, "Window is missing!", this);
        }

        private void Reset()
        {
            Window = GetComponentInParent<JEMInterfaceWindow>();
        }
    }
}
