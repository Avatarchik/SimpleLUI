//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

//
// Original Network simulation design and execution
//  by Damian 'Erdroy' Korczowski (https://github.com/Erdroy)
//

using UnityEngine.Profiling;

namespace JEM.QNet.UnityEngine.Simulation
{
    /// <summary>
    ///     A object snapshot broadcast manager.
    /// </summary>
    public static class QNetObjectSnapshotManager
    {
        internal static void Frame()
        {
            Profiler.BeginSample("QNetObjectSnapshotManager.Frame");
            if (QNetManager.Instance.IsServerActive)
            {
                for (var index = 0; index < QNetSimulableObject.SimulableObjects.Count; index++)
                {
                    var obj = QNetSimulableObject.SimulableObjects[index];
                    if (!obj.isActiveAndEnabled)
                        continue;

                    obj.CallOnSendSnapshot();
                }
            }

            Profiler.EndSample();
        }
    }
}
