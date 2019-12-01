//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JEM.UnityEngine.Audio.Interface.Controls
{
    /// <inheritdoc cref="JEMAudioForControl" />
    /// <summary>
    ///     Toggle audio controller.
    /// </summary>
    [RequireComponent(typeof(Toggle)), DisallowMultipleComponent]
    public sealed class JEMAudioForToggle : JEMAudioForControl, IPointerClickHandler
    {
        public Toggle Toggle { get; private set; }

        private void Awake()
        {
            Toggle = GetComponent<Toggle>();
        }

        /// <inheritdoc />
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Database == null) return;
            if (!Toggle.interactable)
                return;

            JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlCheck));
        }
    }
}
