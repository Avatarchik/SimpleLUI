//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// #define INCLUDE_TRANSFORM_SCALE_SYNC

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
#if INCLUDE_TRANSFORM_SCALE_SYNC
        internal Vector3 Scale { get; set; }
#endif

        /// <inheritdoc />
        public bool IsValid { get; set; }
    }

    /// <inheritdoc cref="IState{TSnapshot}" />
    /// <summary>
    ///     Interpolation state of <see cref="TransformInterpolator" />.
    /// </summary>
    public struct TransformState : IState<TransformSnapshot>
    {
        /// <inheritdoc />
        public TransformSnapshot Snapshot { get; set; }

        /// <inheritdoc />
        public float Time { get; set; }

        /// <inheritdoc />
        public uint Frame { get; set; }

        /// <inheritdoc />
        public bool IsValid { get; set; }
    }

    /// <inheritdoc cref="IResult{TStateType}" />
    /// <summary>
    ///     Interpolation result of <see cref="TransformInterpolator" />.
    /// </summary>
    public struct TransformResult : IResult<TransformState>
    {
        /// <inheritdoc />
        public TransformState Interpolated { get; set; }

        /// <inheritdoc />
        public TransformState Prev { get; set; }

        /// <inheritdoc />
        public TransformState Next { get; set; }

        /// <inheritdoc />
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
#if INCLUDE_TRANSFORM_SCALE_SYNC
            s.Scale = from.Snapshot.Scale;
#endif
            result.Snapshot = s;
        }

        /// <inheritdoc />
        protected override void InternalInterpolateState(float t, ref TransformState from, ref TransformState to, ref TransformState result)
        {
            var s = result.Snapshot;
            s.Position = Vector3.LerpUnclamped(from.Snapshot.Position, to.Snapshot.Position, t);
            s.Rotation = Quaternion.LerpUnclamped(from.Snapshot.Rotation, to.Snapshot.Rotation, t);
#if INCLUDE_TRANSFORM_SCALE_SYNC
            s.Scale = Vector3.LerpUnclamped(from.Snapshot.Scale, to.Snapshot.Scale, t);
#endif
            result.Snapshot = s;
        }
    }
}