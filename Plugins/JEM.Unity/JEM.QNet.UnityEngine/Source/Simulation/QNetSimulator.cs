//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

//
// Original Network simulation design and execution
//  by Damian 'Erdroy' Korczowski (https://github.com/Erdroy)
//

using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace JEM.QNet.UnityEngine.Simulation
{
    /// <summary>
    ///     QNet Simulator class.
    /// </summary>
    public static class QNetSimulator
    {
        internal static void UnsafeSimulate()
        {
            Profiler.BeginSample("QNetSimulation.UnsafeSimulate");
            for (var index = 0; index < QNetSimulableObject.SimulableObjects.Count; index++)
            {
                var obj = QNetSimulableObject.SimulableObjects[index];
                if (!obj.isActiveAndEnabled)
                    continue;

                obj.CallUnsafeSimulate();
            }
            Profiler.EndSample();
        }

        internal static void UnsafeLateSimulate()
        {
            Profiler.BeginSample("QNetSimulation.UnsafeLateSimulate");
            for (var index = 0; index < QNetSimulableObject.SimulableObjects.Count; index++)
            {
                var obj = QNetSimulableObject.SimulableObjects[index];
                if (!obj.isActiveAndEnabled)
                    continue;

                obj.CallUnsafeLateSimulate();
            }
            Profiler.EndSample();
        }

        internal static void Simulate()
        {
            Profiler.BeginSample("QNetSimulation.Simulate");
            const int frameStepWarning = 20;

            if (_framesToStep > frameStepWarning)
            {
                QNetManager.PrintLogWarning($"Simulating over 20 frames ({_framesToStep})!");
            }

            while (_framesToStep > 0)
            {
                // Process frame
                QNetObjectManager.Frame();
                Physics.autoSimulation = false;
                Physics.Simulate(QNetTime.TickStep);

                // Increment frame count
                QNetTime.Frame++;
                EstimatedServerFrame++;

                _framesToStep--;
            }

            Profiler.EndSample();
        }

        internal static void AdjustFrames()
        {
            if (QNetManager.Instance.IsServerActive)
            {
                _framesToStep = 1;
                return;
            }

            if (!QNetManager.Instance.IsClientActive || !QNetManager.Instance.IsNetworkActive)
                return;

            if (AdjustServerFrames)
            {
                _framesToStep = 1;
                QNetTime.Frame += Math.Max(0u, ReceivedServerFrame - EstimatedServerFrame);

#if DEBUG
                QNetManager.PrintLogMsc($"Frame forward ({_framesToStep} frames, local={EstimatedServerFrame}, server={ReceivedServerFrame}))");
#endif

                EstimatedServerFrame = ReceivedServerFrame;
                AdjustServerFrames = false;
            }
            else
            {
                _framesToStep = 1;
            }
        }

        /// <summary>
        ///     Estimated server frame.
        /// </summary>
        public static uint EstimatedServerFrame { get; set; }

        /// <summary>
        ///     Received server frame.
        /// </summary>
        public static uint ReceivedServerFrame { get; set; }

        /// <summary>
        ///     Adjusted server frame.
        /// </summary>
        public static bool AdjustServerFrames { get; set; }

        private static uint _serverFrameDiff;
        private static uint _framesToStep;
    }
}
