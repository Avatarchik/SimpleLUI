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
    ///     Toggle audio controller.
    /// </summary>
    [RequireComponent(typeof(Toggle)), DisallowMultipleComponent]
    internal sealed class JEMAudioForToggle : JEMAudioForControl
    {
        /// <inheritdoc />
        public override void OnPointerClick(PointerEventData eventData)
        {
            JEMAudioManager.SpawnDefault(new JEMAudioData(JEMAudioInterfaceDatabase.Instance.ToggleClick){Is2D = true});
        }
    }
}
