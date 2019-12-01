//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine.Profiling;

namespace JEM.QNet.UnityEngine.Simulation
{
    /// <summary>
    ///     A object simulation manager.
    /// </summary>
    public static class QNetObjectManager
    {
        internal static void Frame()
        {
            Profiler.BeginSample("QNetObjectManager.Frame");
            var objects = QNetSimulableObject.SimulableObjects;

            Profiler.BeginSample("QNetObjectManager.Frame (Simulation)");
            // Begin simulation.
            for (var index = 0; index < objects.Count; index++)
            {
                var obj = objects[index];
                if (!obj.isActiveAndEnabled)
                    continue;

                obj.CallBeginSimulate();
            }

            // Simulate entities.
            for (var index = 0; index < objects.Count; index++)
            {
                var obj = objects[index];
                if (!obj.isActiveAndEnabled)
                    continue;

                obj.CallSimulate();
            }

            // Interpolate entities.
            for (var index = 0; index < objects.Count; index++)
            {
                var obj = objects[index];
                if (!obj.isActiveAndEnabled)
                    continue;

                obj.InterpolateFrame();
            }

            // Simulate commands.
            for (var index = 0; index < objects.Count; index++)
            {
                var obj = objects[index];
                if (!obj.isActiveAndEnabled)
                    continue;

                obj.SimulateFrame();
            }

            // End simulation.
            for (var index = 0; index < objects.Count; index++)
            {
                var obj = objects[index];
                if (!obj.isActiveAndEnabled)
                    continue;

                obj.CallFinishSimulate();
            }
            Profiler.EndSample();

            // Update the snapshot
            QNetObjectSnapshotManager.Frame();
            Profiler.EndSample();
        }
    }
}
