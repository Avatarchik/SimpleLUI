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
    ///     Slider audio controller.
    /// </summary>
    [RequireComponent(typeof(Slider)), DisallowMultipleComponent]
    public sealed class JEMAudioForSlider : JEMAudioForControl, IPointerClickHandler
    {
        [Header("Settings")]
        public float AudioSensitivity = 1f;

        public Slider Slider { get; private set; }

        private float prev;
        private float next;
        private void Awake()
        {
            Slider = GetComponent<Slider>();
            Slider.onValueChanged.AddListener(value =>
            {
                if (!Slider.interactable)
                    return;

                var diff = prev / value;
                prev = value;

                next += diff;
                if (!(next > AudioSensitivity)) return;
                next = 0f;
                JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlEndEdit));
            });
        }

        /// <inheritdoc />
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Database == null) return;
            if (!Slider.interactable)
                return;

            JEMAudioManager.SpawnDefault(JEMAudioData.CreateUI(Database.ControlCheck));
        }
    }
}
