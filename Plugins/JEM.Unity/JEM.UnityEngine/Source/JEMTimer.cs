//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Internal;
using System;
using UnityEngine;

namespace JEM.UnityEngine
{
    // TODO: Allow to use more that one timer at once.
    /// <summary>
    ///     Utility timer update event.
    /// </summary>
    /// <param name="time">Current time.</param>
    /// <param name="targetTime">Target time.</param>
    public delegate void JEMTimerUpdate(int time, int targetTime);

    /// <summary>
    ///     Timer class.
    /// </summary>
    [Obsolete]
    public class JEMTimer
    {
        /// <summary>
        ///     Timer update event.
        /// </summary>
        public JEMTimerUpdate OnUpdate;

        /// <summary>
        ///     Timer running state.
        /// </summary>
        public bool IsRunning { get; private set; }

        private JEMTimer()
        {
            //var rootGameObject = GameObject.Find(nameof(JEMTimer)) ?? new GameObject(nameof(JEMTimer));
            //_script = rootGameObject.CollectComponent<JEMTimerScript>();
        }

        /// <summary>
        ///     Destroys timer.
        /// </summary>
        public void DestroyTimer()
        {
            //if (_script == null)
            //    return;

            //Object.Destroy(_script);
        }

        /// <summary>
        ///     Starts timer.
        /// </summary>
        /// <param name="targetTime">Target time of timer.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public void StartTimer(int targetTime)
        {
            if (targetTime <= 0) throw new ArgumentOutOfRangeException(nameof(targetTime));
            if (IsRunning)
                throw new NotSupportedException("Can't run another timer task, while other is already running.");
            if (_script == null)
                throw new NullReferenceException(nameof(_script));
            if (OnUpdate == null)
                Debug.LogWarning("OnUpdate event of UtilityInstance is equals null.");

            IsRunning = true;
            Script.StartCoroutine(Script.CuntDownTimer(targetTime, time => { OnUpdate?.Invoke(time, targetTime); }));
        }

        /// <summary>
        /// </summary>
        public void StopTimer()
        {
            IsRunning = false;
            Script.StopCoroutine(nameof(Script.CuntDownTimer));
        }

        /// <summary>
        ///     Creates new timer instance.
        /// </summary>
        public static JEMTimer CreateTimer()
        {
            var timer = new JEMTimer();
            return timer;
        }

        private static JEMTimerScript Script
        {
            get
            {
                if (_script == null)
                    _script = JEMTimerScript.GetScript();

                return _script;
            }
        }

        private static JEMTimerScript _script;
    }
}