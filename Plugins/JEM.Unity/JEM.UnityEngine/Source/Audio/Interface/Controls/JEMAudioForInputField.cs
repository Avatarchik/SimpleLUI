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
    ///     TextField audio controller.
    /// </summary>
    [RequireComponent(typeof(InputField)), DisallowMultipleComponent]
    public sealed class JEMAudioForInputField : JEMAudioForControl, IPointerEnterHandler, IPointerClickHandler
    {
        public InputField InputField { get; private set; }

        private void Awake()
        {
            InputField = GetComponent<InputField>();
            InputField.onEndEdit.AddListener(str =>
            {
                if (!InputField.interactable)
                    return;

                JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlEndEdit));
            });
        }

        /// <inheritdoc />
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Database == null) return;
            if (!InputField.interactable)
                return;

            JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlEnter));
        }

        /// <inheritdoc />
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Database == null) return;
            if (!InputField.interactable)
                return;

            JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlClick));
        }
    }
}
