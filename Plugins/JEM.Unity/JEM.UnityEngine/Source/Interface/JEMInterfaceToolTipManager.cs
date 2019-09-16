//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.UnityEngine.Interface
{
    /// <inheritdoc />
    /// <summary>
    ///     JEM Interface ToolTip Manager.
    ///     Handles UI of the ToolTip.
    /// </summary>
    [AddComponentMenu("JEM/Interface/Tooltip/JEM Tooltip Manager")]
    [DisallowMultipleComponent]
    internal class JEMInterfaceToolTipManager : MonoBehaviour
    {
        /// <summary>
        ///     Canvas of the tooltip.
        ///     Used to keep the tooltip prefab in the canvas bounds.
        /// </summary>
        [Header("Settings")]
        [SerializeField] internal Canvas ToolTipCanvas;

        /// <summary>
        ///     Reference to our toolTip prefab.
        /// </summary>
        [SerializeField] internal JEMInterfaceToolTipPrefab ToolTipPrefab;

        private void Awake()
        {
            if (Instance != null)
                return;

            Instance = this;
        }

        private void Start()
        {
            ToolTipPrefab.RegisterWindowCanvas(ToolTipCanvas);
        }

        /// <summary>
        ///     Sets tooltip text.
        /// </summary>
        internal void SetToolTip(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                ToolTipPrefab.Panel.SetActive(false);
            }
            else
            {
                ToolTipPrefab.Panel.SetActive(true);
                ToolTipPrefab.Text.text = text;
            }
        }

        /// <summary>
        ///     Current instance of script.
        /// </summary>
        internal static JEMInterfaceToolTipManager Instance { get; private set; }
    }
}
