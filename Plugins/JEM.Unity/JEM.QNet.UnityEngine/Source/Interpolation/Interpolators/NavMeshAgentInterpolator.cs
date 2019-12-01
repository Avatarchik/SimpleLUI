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
    ///     Interpolation snapshot of <see cref="NavMeshAgentInterpolator" />.
    /// </summary>
    public struct NavMeshAgentSnapshot : ISnapshot
    {
        internal Vector3 Position { get; set; }
        internal float Rotation { get; set; }

        /// <inheritdoc />
        public bool IsValid { get; set; }
    }

    /// <inheritdoc cref="IState{TSnapshot}" />
    /// <summary>
    ///     Interpolation state of <see cref="NavMeshAgentInterpolator" />.
    /// </summary>
    public struct NavMeshAgentState : IState<NavMeshAgentSnapshot>
    {
        /// <inheritdoc />
        public NavMeshAgentSnapshot Snapshot { get; set; }

        /// <inheritdoc />
        public float Time { get; set; }

        /// <inheritdoc />
        public uint Frame { get; set; }

        /// <inheritdoc />
        public bool IsValid { get; set; }
    }

    /// <inheritdoc cref="IResult{TStateType}" />
    /// <summary>
    ///     Interpolation result of <see cref="NavMeshAgentInterpolator" />.
    /// </summary>
    public struct NavMeshAgentResult : IResult<NavMeshAgentState>
    {
        /// <inheritdoc />
        public NavMeshAgentState Interpolated { get; set; }

        /// <inheritdoc />
        public NavMeshAgentState Prev { get; set; }

        /// <inheritdoc />
        public NavMeshAgentState Next { get; set; }
        public float Amount { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Simple navMeshAgent interpolator.
    ///     Interpolates transform position, rotation (only y axis).
    /// </summary>
    public class NavMeshAgentInterpolator : BaseInterpolator<NavMeshAgentState, NavMeshAgentResult, NavMeshAgentSnapshot>
    {
        /// <inheritdoc />
        protected override void ResetState(ref NavMeshAgentState result, ref NavMeshAgentState from)
        {
            var s = result.Snapshot;
            s.Position = from.Snapshot.Position;
            s.Rotation = from.Snapshot.Rotation;
            result.Snapshot = s;
        }

        /// <inheritdoc />
        protected override void InternalInterpolateState(float t, ref NavMeshAgentState from, ref NavMeshAgentState to, ref NavMeshAgentState result)
        {
            var s = result.Snapshot;
            s.Position = Vector3.LerpUnclamped(from.Snapshot.Position, to.Snapshot.Position, t);
            var rotationHelper = Quaternion.Lerp(Quaternion.Euler(0f, from.Snapshot.Rotation, 0f),
                Quaternion.Euler(0f, to.Snapshot.Rotation, 0f), t);
            s.Rotation = rotationHelper.eulerAngles.y;
            result.Snapshot = s;
        }
    }
}