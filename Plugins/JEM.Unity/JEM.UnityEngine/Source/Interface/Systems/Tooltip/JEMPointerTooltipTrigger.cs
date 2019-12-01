//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;
using UnityEngine.EventSystems;

namespace JEM.UnityEngine.Interface.Systems
{
    /// <inheritdoc cref="JEMTooltipTrigger{TTriggerData}" />
    /// <summary>
    ///     A tooltip component that implements <see cref="T:UnityEngine.EventSystems.IPointerEnterHandler" /> and <see cref="T:UnityEngine.EventSystems.IPointerExitHandler" /> to trigger active tooltip.
    /// </summary>
    public abstract class JEMPointerTooltipTrigger<TTriggerData> : JEMTooltipTrigger<TTriggerData>,
        IPointerEnterHandler, IPointerExitHandler where TTriggerData : ITooltipTriggerData
    {
        [Header("Pointer Settings")]
        public bool TriggerUsingPointerEvents = true;

        /// <inheritdoc />
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!TriggerUsingPointerEvents)
            {
                return;
            }

            Trigger();
        }

        /// <inheritdoc />
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!TriggerUsingPointerEvents)
            {
                return;
            }

            Controller.Disable();
        }
    }
}
