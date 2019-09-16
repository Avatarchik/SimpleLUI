//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JEM.UnityEngine.Interface
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary>
    ///     JEM Interface ToolTip Element.
    ///     A component that tells what UI element should show what tooltip data on the screen.
    /// </summary>
    [AddComponentMenu("JEM/Interface/Tooltip/JEM Tooltip Element")]
    [DisallowMultipleComponent]
    public class JEMInterfaceToolTipElement : MonoBehaviour, ITooltipElement, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        ///     Defines the enable state of the tooltip.
        /// </summary>
        [Header("Settings")]
        public bool Enabled = true;

        /// <summary>
        ///     Text content to drawn of this tooltip.
        ///     NOTE: If localeKey is not empty, this value will be always overwritten by JEMLocale.Resolve method.
        /// </summary>
        public string Text = "Undefined text";

        /// <summary>
        ///     Locale group of this tooltip.
        /// </summary>
        [Space]
        public string LocaleGroup = "SYSTEM";

        /// <summary>
        ///     Locale key of the tooltip.
        /// </summary>
        public string LocaleKey = "";

        private bool _set;

        private void OnDisable()
        {
            if (!_set) return;
            _set = false;
            JEMInterfaceToolTipManager.Instance.SetToolTip(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Enabled) return;
            _set = true;
            Refresh();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_set) return;
            _set = false;
            JEMInterfaceToolTipManager.Instance.SetToolTip(null);
        }

        /// <inheritdoc />
        public void Refresh()
        {
            if (_set)
            {
                if (!string.IsNullOrEmpty(LocaleKey))
                {
                    Text = JEMLocale.Resolve(LocaleGroup, LocaleKey.ToUpper());
                    Text = Text.Replace(@"\t", "\t");
                }

                JEMInterfaceToolTipManager.Instance.SetToolTip(Text);
            }
        }
    }
}
