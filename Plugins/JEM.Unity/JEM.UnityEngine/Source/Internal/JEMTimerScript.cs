//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections;
using UnityEngine;

namespace JEM.UnityEngine.Internal
{
    /// <inheritdoc />
    internal class JEMTimerScript : JEMRegenerableScript<JEMTimerScript>
    {
        /// <inheritdoc />
        protected override void OnAwake()
        {
            // ignore
        }

        /// <summary/>
        internal IEnumerator CuntDownTimer(int targetTime, Action<int> onTick)
        {
            var time = 0;
            while (time < targetTime)
            {
                onTick?.Invoke(time);
                time++;
                yield return new WaitForSeconds(1);
            }
        }
    }
}