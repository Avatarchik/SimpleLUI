//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Common;
using System.Collections.Generic;
using UnityEngine;

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
        private JEMSmartMethod _onUnsafeSimulate;

        /// <summary>
        ///      UnsafeSimulate is a Unity's LateUpdate equivalent.
        /// </summary>
        private JEMSmartMethod _onUnsafeLateSimulate;

        /// <summary>
        ///     OnBeginSimulate called at the beginning of object simulation.
        /// </summary>
        private JEMSmartMethod _onBeginSimulate;

        /// <summary>
        ///     UnsafeSimulate is a Unity's FixedUpdate equivalent.
        ///     Called before QNetBehaviour.SimulateWithResult bot after OnBeginSimulate.
        /// </summary>
        private JEMSmartMethod _onSimulate;

        /// <summary>
        ///     OnFinishSimulate called at the very end of the object simulation.
        /// </summary>
        private JEMSmartMethod _onFinishSimulate;

        /// <summary>
        ///     OnSendSnapshot called by snapshot manager to send a for ex. object interpolation snapshot.
        /// </summary>
        /// <remarks>
        ///     Called only on the server side.
        /// </remarks>
        private JEMSmartMethod _onSendSnapshot;

        /// <summary>
        ///     Load all JEMSmartMethod based methods in this class.
        /// </summary>
        protected virtual void LoadMethods()
        {
            _onUnsafeSimulate = new JEMSmartMethod(this, "UnsafeSimulate");
            _onUnsafeLateSimulate = new JEMSmartMethod(this, "UnsafeLateSimulate");

            _onBeginSimulate = new JEMSmartMethod(this, "OnBeginSimulate");
            _onSimulate = new JEMSmartMethod(this, "Simulate");
            _onFinishSimulate = new JEMSmartMethod(this, "OnFinishSimulate");

            _onSendSnapshot = new JEMSmartMethod(this, "OnSendSnapshot");

            SimulableObjects.Add(this);
        }

        protected virtual void OnDestroy() => SimulableObjects.Remove(this);

        internal abstract void InterpolateFrame();
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
