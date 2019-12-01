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
    ///     Button audio controller.
    /// </summary>
    [RequireComponent(typeof(Button)), DisallowMultipleComponent]
    public sealed class JEMAudioForButton : JEMAudioForControl, IPointerEnterHandler, IPointerClickHandler
    {
        public Button Button { get; private set; }

        private void Awake()
        {
            Button = GetComponent<Button>();
        }

        /// <inheritdoc />
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Database == null) return;      
            if (!Button.interactable)
                return;

            JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlEnter));
        }

        /// <inheritdoc />
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Database == null) return;
            if (!Button.interactable)
                return;

            JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlClick));
        }
    }
}
