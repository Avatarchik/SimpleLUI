//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Interface.Window;
using UnityEngine;
using UnityEngine.UI;

namespace JEM.UnityEngine.Interface
{
    /// <inheritdoc />
    /// <summary>
    ///     JEM Interface ToolTip Prefab.
    ///     A helper component for <see cref="JEMInterfaceToolTipManager" /> that handles UI of drawn toolTip data.
    /// </summary>
    [AddComponentMenu("JEM/Interface/Tooltip/JEM Tooltip Prefab")]
    [DisallowMultipleComponent]
    internal class JEMInterfaceToolTipPrefab : JEMInterfaceWindow
    {
        /// <summary>
        ///     Reference to the fade animation panel of tooltip.
        /// </summary>
        [Header("Settings")]
        [SerializeField] internal JEMInterfaceFadeAnimation Panel;

        /// <summary>
        ///     Text component reference.
        /// </summary>
        [SerializeField] internal Text Text;

        /// <summary>
        ///     Fixed offset of drawn tooltip.
        /// </summary>
        [SerializeField] internal Vector2 Offset;

        /// <summary>
        ///     Defines enable state of tooltip smoothing.
        /// </summary>
        [SerializeField] internal bool UseSmoothing = true;

        /// <summary>
        ///     Smoothing speed.
        /// </summary>
        [Range(5f, 30f)]
        [SerializeField] internal float SmoothMod = 22f;

        internal RectTransform RectTransform { get; private set; }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            RectTransform.anchoredPosition = (Vector2) Input.mousePosition + Offset;
        }

        private void LateUpdate()
        {
            if (Panel.TargetActive)
            {
                Offset.y = RectTransform.sizeDelta.y / 2f;
                var point = (Vector2) Input.mousePosition + Offset;

                if (UseSmoothing)
                {
                    var t = Time.deltaTime * SmoothMod;
                    RectTransform.anchoredPosition = Vector2.LerpUnclamped(RectTransform.anchoredPosition, point, t);
                }
                else
                {
                    RectTransform.anchoredPosition = point;
                }
            }

            UpdateDisplay();
        }
    }
}
