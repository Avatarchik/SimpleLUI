//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

//
// Original Network simulation design and implementation
//  by Damian 'Erdroy' Korczowski (https://github.com/Erdroy)
//

using JEM.Core.Extension;
using JEM.QNet.UnityEngine.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Interpolation
{
    public interface ISnapshot
    {
        bool IsValid { get; set; }
    }

    public interface IState<TSnapshot> where TSnapshot : ISnapshot
    {
        TSnapshot Snapshot { get; set; }

        float Time { get; set; }
        uint Frame { get; set; }

        bool IsValid { get; set; }
    }

    public interface IResult<TStateType>
    {
        TStateType Interpolated { get; set; }
        TStateType Prev { get; set; }
        TStateType Next { get; set; }

        float Amount { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Base class of every interpolator.
    /// </summary>
    public abstract class BaseInterpolator<TState, TResult, TSnapshot> : IDisposable where TState 
        : IState<TSnapshot> where TResult : IResult<TState> where TSnapshot : ISnapshot
    {
        /// <summary>
        ///     Defines whether the interpolator is enabled.
        /// </summary>
        public bool Enabled => QNetSettings.Interpolation > 0;

        public IReadOnlyList<TState> States => _stateList;

        private readonly int _maxStates;
        private List<TState> _stateList;
        private TState _lastState;

        protected BaseInterpolator()
        {
            _maxStates = QNetTime.TickRate;
            Reset();
        }

        public void Dispose()
        {
            _stateList.Clear();
            _stateList.Capacity = 0;
        }

        public void Reset()
        {
            _lastState = default;
            _stateList = new List<TState>(_maxStates);
        }

        public TResult Interpolate(float time, float interpolation, float extrapolation)
        {
            var result = FastObjectFactory<TResult>.Instance();

            if (!_stateList.Any())
                return result;

            var renderTime = time - interpolation;
            var shouldInterpolate = _lastState.Time > renderTime;

            if (!shouldInterpolate)
            {
                result.Prev = _stateList.First();
                result.Interpolated = _stateList.Last();
                return result;
            }

            // Find previous state
            var prev = _stateList.LastOrDefault(x => x.Time < renderTime);

            // Find next state
            var next = _stateList.FirstOrDefault(x => x.Time >= renderTime);

            var canInterpolate = prev.IsValid && next.IsValid;
            if (canInterpolate)
            {
                var timeDiff = next.Time - prev.Time;

                if (timeDiff < float.Epsilon)
                {
#if DEBUG
                    QNetManager.PrintLogWarning("Interpolation failed! " +
                                                $"Prev time={prev.Time} Next time={next.Time} diff={timeDiff}");
#endif
                    result.Prev = prev;
                    result.Interpolated = next;
                    return result;
                }

                var t = (renderTime - prev.Time) / timeDiff;

                result.Prev = prev;
                result.Next = next;
                result.Amount = t;
                InterpolateState(t, ref prev, ref next, out var interpolationResult);
                result.Interpolated = interpolationResult;
            }
            else
            {
                // TODO: Extrapolate
            }

            return result;
        }

        /// <summary>
        ///     Add state to interpolate.
        /// </summary>
        public void AddState(TSnapshot snapshot, float time, uint serverFrame)
        {
            var state = FastObjectFactory<TState>.Instance();
            state.Snapshot = snapshot;
            state.Time = time;
            state.Frame = serverFrame;
            state.IsValid = true;

            _lastState = state;
            _stateList.Add(state);

            // Do not exceed max capacity, as this would be
            // waste of memory and cycles. We don't really need that
            // much states.
            while (_stateList.Count >= _maxStates)
                _stateList.RemoveAt(0);

            // Simple dejitter.
            _stateList.Sort((a, b) => a.Frame.CompareTo(b.Frame));
        }

        /// <summary>
        ///     Interpolate states from to.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InterpolateState(float t, ref TState from, ref TState to, out TState result)
        {
            result = default;

            // Update interpolated snapshot valid state.;
            var snap = result.Snapshot;
            if (from.Snapshot.IsValid && to.Snapshot.IsValid)
                snap.IsValid = true;
            else snap.IsValid = false;
            result.Snapshot = snap;

            if (!result.Snapshot.IsValid)
            {
                // When snapshot is invalid, do not interpolate anything.
                return;
            }

            if (t < float.Epsilon)
            {
                ResetState(ref result, ref from);
                return;
            }

            if (result != null)
            {
                result.Frame = from.Frame + (uint) Mathf.RoundToInt((to.Frame - from.Frame) * t);
                InternalInterpolateState(t, ref from, ref to, ref result);
            }
        }

        protected abstract void ResetState(ref TState result, ref TState from);
        protected abstract void InternalInterpolateState(float t, ref TState from, ref TState to, ref TState result);
    }
}
