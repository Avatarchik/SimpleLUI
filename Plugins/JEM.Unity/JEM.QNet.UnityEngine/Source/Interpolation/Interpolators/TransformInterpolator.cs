//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.QNet.UnityEngine.Interpolation.Interpolators
{
    /// <inheritdoc cref="ISnapshot" />
    /// <summary>
    ///     Interpolation snapshot of <see cref="TransformInterpolator" />.
    /// </summary>
    public struct TransformSnapshot : ISnapshot
    {
        internal Vector3 Position { get; set; }
        internal Quaternion Rotation { get; set; }
        internal Vector3 Scale { get; set; }
    }

    /// <inheritdoc cref="IState{TSnapshot}" />
    /// <summary>
    ///     Interpolation state of <see cref="TransformInterpolator" />.
    /// </summary>
    public struct TransformState : IState<TransformSnapshot>
    {
        public TransformSnapshot Snapshot { get; set; }
        public float Time { get; set; }
        public uint Frame { get; set; }
        public bool IsValid { get; set; }
    }

    /// <inheritdoc cref="IResult{TStateType}" />
    /// <summary>
    ///     Interpolation result of <see cref="TransformInterpolator" />.
    /// </summary>
    public struct TransformResult : IResult<TransformState>
    {
        public TransformState Interpolated { get; set; }
        public TransformState Prev { get; set; }
        public TransformState Next { get; set; }
        public float Amount { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Simple transform interpolator.
    ///     Interpolates transform position, rotation and localScale.
    /// </summary>
    public class TransformInterpolator : BaseInterpolator<TransformState, TransformResult, TransformSnapshot>
    {
        /// <inheritdoc />
        protected override void ResetState(ref TransformState result, ref TransformState from)
        {
            var s = result.Snapshot;
            s.Position = from.Snapshot.Position;
            s.Rotation = from.Snapshot.Rotation;
            s.Scale = from.Snapshot.Scale;
            result.Snapshot = s;
        }

        /// <inheritdoc />
        protected override void InternalInterpolateState(float t, ref TransformState from, ref TransformState to, ref TransformState result)
        {
            var s = result.Snapshot;
            s.Position = Vector3.LerpUnclamped(from.Snapshot.Position, to.Snapshot.Position, t);
            s.Rotation = Quaternion.LerpUnclamped(from.Snapshot.Rotation, to.Snapshot.Rotation, t);
            s.Scale = Vector3.LerpUnclamped(from.Snapshot.Scale, to.Snapshot.Scale, t);
            result.Snapshot = s;
        }
    }
}