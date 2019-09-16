//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JEM.UnityEngine.Audio.Interface
{
    /// <inheritdoc />
    /// <summary>
    ///     Button audio controller.
    /// </summary>
    [RequireComponent(typeof(Button)), DisallowMultipleComponent]
    internal sealed class JEMAudioForButton : JEMAudioForControl
    {
        internal Button Button { get; private set; }

        private void Awake()
        {
            Button = GetComponent<Button>();
        }

        /// <inheritdoc />
        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (!Button.interactable)
                return;
            JEMAudioManager.SpawnDefault(new JEMAudioData(JEMAudioInterfaceDatabase.Instance.ControlEnter) { Is2D = true });
        }

        /// <inheritdoc />
        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!Button.interactable)
                return;
            JEMAudioManager.SpawnDefault(new JEMAudioData(JEMAudioInterfaceDatabase.Instance.ButtonClick) { Is2D = true });
        }
    }
}
