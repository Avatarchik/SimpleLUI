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
    ///     Dropdown audio controller.
    /// </summary>
    [RequireComponent(typeof(Dropdown)), DisallowMultipleComponent]
    public sealed class JEMAudioForDropdown : JEMAudioForControl, IPointerEnterHandler, IPointerClickHandler
    {
        public Dropdown Dropdown { get; private set; }

        private void Awake()
        {
            Dropdown = GetComponent<Dropdown>();
            Dropdown.onValueChanged.AddListener(value =>
            {
                if (!Dropdown.interactable)
                    return;

                JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlEndEdit));
            });
        }

        /// <inheritdoc />
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Database == null) return;
            if (!Dropdown.interactable)
                return;

            // Do not spawn SFX if dropDown list is active
            if (Dropdown.transform.Find("Dropdown List"))
                return;

            JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlEnter));
        }

        /// <inheritdoc />
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Database == null) return;
            if (!Dropdown.interactable)
                return;

            JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlExpand));
        }
    }
}
