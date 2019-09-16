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
    ///     Dropdown audio controller.
    /// </summary>
    [RequireComponent(typeof(Dropdown)), DisallowMultipleComponent]
    internal sealed class JEMAudioForDropdown : JEMAudioForControl
    {
        /// <inheritdoc />
        public override void OnPointerEnter(PointerEventData eventData)
        {
            JEMAudioManager.SpawnDefault(new JEMAudioData(JEMAudioInterfaceDatabase.Instance.ControlEnter) { Is2D = true });
        }

        /// <inheritdoc />
        public override void OnPointerClick(PointerEventData eventData)
        {
            JEMAudioManager.SpawnDefault(new JEMAudioData(JEMAudioInterfaceDatabase.Instance.DropdownClick) { Is2D = true });
        }
    }
}
