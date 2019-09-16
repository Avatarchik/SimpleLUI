//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.EventSystems;

namespace JEM.UnityEngine.Audio.Interface
{
    /// <inheritdoc cref="MonoBehaviour" />
    /// <summary>
    ///     Base of audio controller for interface control.
    /// </summary>
    internal abstract class JEMAudioForControl : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {
        /// <inheritdoc />
        public virtual void OnPointerEnter(PointerEventData eventData) { }

        /// <inheritdoc />
        public virtual void OnPointerClick(PointerEventData eventData) { }
    }
}
