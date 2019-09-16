//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Extension;
using JEM.QNet.UnityEngine.Objects;
using JEM.QNet.UnityEngine.Simulation;
using JetBrains.Annotations;
using System;

namespace JEM.QNet.UnityEngine.Interpolation
{
    public delegate void FinalObjectInterpolation<in TSnapshot>(TSnapshot newState, float smooth)
        where TSnapshot : ISnapshot;

    public delegate void ObjectInterpolationReset<in TSnapshot>(TSnapshot newState, bool isFirst)
        where TSnapshot : ISnapshot;

    /// <summary>
    ///     Object interpolation helper!
    ///     It contains all the soul needs to properly? interpolate any state of object.
    /// </summary>
    public class ObjectInterpolationHelper<TState, TResult, TSnapshot, TInterpolator>
        where TState : IState<TSnapshot>
        where TResult : IResult<TState>
        where TSnapshot : ISnapshot
        where TInterpolator : BaseInterpolator<TState, TResult, TSnapshot>
    {
        public float InterpolationTime { get; set; }

        private readonly QNetObject _obj;
        private readonly TInterpolator _interpolator;
        private TState _currentState;
        private TState _previousState;
        private FinalObjectInterpolation<TSnapshot> _finalInterpolation;
        private ObjectInterpolationReset<TSnapshot> _stateReset;

        public ObjectInterpolationHelper(QNetObject obj)
        {
            if (obj == null)
                throw new ArgumentException(null, nameof(obj));

            _obj = obj;
            _interpolator = FastObjectFactory<TInterpolator>.Instance();
            if (_interpolator == null)
                throw new NullReferenceException(nameof(_interpolator));
        }

        /// <summary>
        ///     Registers a final interpolation event with is called to apply final interpolation results to target object.
        /// </summary>
        public void RegisterFinalInterpolation([NotNull] FinalObjectInterpolation<TSnapshot> finalInterpolation)
        {
            _finalInterpolation = finalInterpolation ?? throw new ArgumentNullException(nameof(finalInterpolation));
        }

        /// <summary>
        ///     Registers state reset event with is called to restart current state of target object by directly applying received snapshot. (No interpolation should be used)
        /// </summary>
        public void RegisterStateReset([NotNull] ObjectInterpolationReset<TSnapshot> stateReset)
        {
            _stateReset = stateReset ?? throw new ArgumentNullException(nameof(stateReset));
        }

        /// <summary>
        ///     Interpolate.
        /// </summary>
        /// <remarks>
        ///     Should be called via InterpolateFrame/Simulate method.
        /// </remarks>
        public void Interpolate()
        {
            if ((!_interpolator?.Enabled ?? true) || QNetObject.IsServer) return;
            if (_obj.IsOwner && QNetSettings.ClientSidePrediction)
                return;

            var interpolation = QNetSettings.Interpolation * 0.001f;
            var extrapolation = QNetSettings.Extrapolation * 0.001f;

            // Calculate interpolated state
            _previousState = _currentState;
            _currentState = _interpolator.Interpolate(QNetTime.Time, interpolation, extrapolation).Interpolated;
        }

        /// <summary>
        ///     Apply the interpolated state.
        /// </summary>
        /// <remarks>
        ///     Should be called via UnsafeSimulate method.
        /// </remarks>
        public void Apply(float objectSmoothing)
        {
            if ((!_interpolator?.Enabled ?? true) || QNetObject.IsServer) return;
            if (_obj.IsOwner && QNetSettings.ClientSidePrediction)
                return;

            // Interpolate frame state changes.
            _interpolator.InterpolateState(QNetTime.FrameAlpha, ref _previousState, ref _currentState, out var state);

            // Calculate server time
            InterpolationTime = state.Frame * QNetTime.TickStep;

            // Apply.
            var smooth = objectSmoothing;
            if (smooth > 0f || _stateReset == null)
                _finalInterpolation.Invoke(state.Snapshot, smooth);
            else
            {
                _stateReset.Invoke(state.Snapshot, false);
            }
        }

        /// <summary>
        ///     Add snapshot to interpolate.
        /// </summary>
        public void AddSnapshot(TSnapshot snapshotData, uint serverFrame, bool reset = false)
        {
            if (_interpolator == null) return;
            if (reset) _interpolator.Dispose();
            if (_interpolator.Enabled)
                _interpolator.AddState(snapshotData, QNetTime.Time, serverFrame);

            if (_interpolator.Enabled && _interpolator.States.Count > 1)
                return;

            _stateReset?.Invoke(snapshotData, true);
            InterpolationTime = QNetTime.ServerTime;
        }
    }
}
