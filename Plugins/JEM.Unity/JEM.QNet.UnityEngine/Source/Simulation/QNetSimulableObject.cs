//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace JEM.QNet.UnityEngine.Simulation
{
    /// <inheritdoc />
    /// <summary>
    ///     A base class of every object that cloud be simulated by <see cref="QNetSimulator" />
    /// </summary>
    public abstract class QNetSimulableObject : MonoBehaviour
    {
        /// <summary>
        ///     A last snapshot send time.
        /// </summary>
        public float LastSnapshotTime { get; private set; }

        /// <summary>
        ///      UnsafeSimulate is a Unity's Update equivalent.
        /// </summary>
        private JEMSmartMethodS _onUnsafeSimulate;

        /// <summary>
        ///      UnsafeSimulate is a Unity's LateUpdate equivalent.
        /// </summary>
        private JEMSmartMethodS _onUnsafeLateSimulate;

        /// <summary>
        ///     OnBeginSimulate called at the beginning of object simulation.
        /// </summary>
        private JEMSmartMethodS _onBeginSimulate;

        /// <summary>
        ///     UnsafeSimulate is a Unity's FixedUpdate equivalent.
        ///     Called before QNetBehaviour.SimulateWithResult but after OnBeginSimulate.
        /// </summary>
        private JEMSmartMethodS _onSimulate;

        /// <summary>
        ///     OnFinishSimulate called at the very end of the object simulation.
        /// </summary>
        private JEMSmartMethodS _onFinishSimulate;

        /// <summary>
        ///     OnSendSnapshot called by snapshot manager to send a for ex. object interpolation snapshot.
        /// </summary>
        /// <remarks>
        ///     Called only on the server side.
        /// </remarks>
        private JEMSmartMethodS _onSendSnapshot;

        /// <summary>
        ///     Load all JEMSmartMethod based methods in this class.
        /// </summary>
        protected virtual void LoadMethods()
        {
            Profiler.BeginSample("QNetSimulableObject.LoadMethods");

            _onUnsafeSimulate = new JEMSmartMethodS(this, "UnsafeSimulate");
            _onUnsafeLateSimulate = new JEMSmartMethodS(this, "UnsafeLateSimulate");

            _onBeginSimulate = new JEMSmartMethodS(this, "OnBeginSimulate");
            _onSimulate = new JEMSmartMethodS(this, "Simulate");
            _onFinishSimulate = new JEMSmartMethodS(this, "OnFinishSimulate");

            _onSendSnapshot = new JEMSmartMethodS(this, "OnSendSnapshot");

            SimulableObjects.Add(this);

            Profiler.EndSample();
        }

        protected virtual void OnDestroy() => SimulableObjects.Remove(this);

        public abstract void InterpolateFrame();
        internal abstract void SimulateFrame();

        internal void CallUnsafeSimulate() => _onUnsafeSimulate.Invoke();
        internal void CallUnsafeLateSimulate() => _onUnsafeLateSimulate.Invoke();
        internal void CallBeginSimulate() => _onBeginSimulate.Invoke();
        internal void CallSimulate() => _onSimulate.Invoke();    
        internal void CallFinishSimulate() =>_onFinishSimulate.Invoke();
        
        internal void CallOnSendSnapshot()
        {
            _onSendSnapshot.Invoke();
            LastSnapshotTime = QNetTime.ServerTime;
        }

        internal static List<QNetSimulableObject> SimulableObjects { get; } = new List<QNetSimulableObject>();
    }
}
