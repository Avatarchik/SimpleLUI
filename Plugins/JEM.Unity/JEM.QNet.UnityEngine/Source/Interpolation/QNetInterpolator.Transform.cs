//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// #define INCLUDE_TRANSFORM_SCALE_SYNC

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Interpolation.Interpolators;
using JEM.QNet.UnityEngine.Objects;
using JEM.QNet.UnityEngine.Simulation;
using JEM.UnityEngine.Attribute;
using JEM.UnityEngine.Extension;
using System;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Interpolation
{
    /// <summary>
    ///     Defines what axis transform interpolator should serialize over network.
    /// </summary>
    [Flags]
    public enum TransformSnapshotAxis : sbyte
    {
        X    = 1 << 0,
        Y    = 1 << 1,
        Z    = 1 << 2,
    }

    /// <inheritdoc cref="IQNetSerializedMessage" />
    /// <summary>
    ///     Serialize snapshot data of transform interpolator.-
    /// </summary>
    public struct TransformSerializedSnapshot : IQNetSerializedMessage
    {
        public TransformSnapshotAxis PositionAxis { get; set; }
        public TransformSnapshotAxis RotationAxis { get; set; }
#if INCLUDE_TRANSFORM_SCALE_SYNC
        public TransformSnapshotAxis ScaleAxis { get; set; }
#endif

        public TransformSnapshot Snapshot { get; set; }

        /// <inheritdoc />
        public void Serialize(QNetMessageWriter writer)
        {
            WriteVector3WithAxis(writer, PositionAxis, Snapshot.Position);
            WriteVector3WithAxis(writer, RotationAxis, Snapshot.Rotation.eulerAngles);
#if INCLUDE_TRANSFORM_SCALE_SYNC
            WriteVector3WithAxis(writer, ScaleAxis, Snapshot.Scale);
#endif
        }

        /// <inheritdoc />
        public void DeSerialize(QNetMessageReader reader)
        {
            Snapshot = new TransformSnapshot
            {
                Position = ReadVector3WithAxis(reader),
                Rotation = Quaternion.Euler(ReadVector3WithAxis(reader)),
#if INCLUDE_TRANSFORM_SCALE_SYNC
                Scale = ReadVector3WithAxis(reader),
#endif
                IsValid = true
            };
        }

        /// <summary>
        ///     Reads the Vector3 that has been serialized using <see cref="TransformSnapshotAxis"/>.
        /// </summary>
        public static Vector3 ReadVector3WithAxis(QNetMessageReader reader)
        {
            var axis = (TransformSnapshotAxis) reader.ReadSByte();
            float x = 0f;
            float y = 0f;
            float z = 0f;
            if (axis.HasFlag(TransformSnapshotAxis.X))
                x = reader.ReadSingle();
            if (axis.HasFlag(TransformSnapshotAxis.Y))
                y = reader.ReadSingle();
            if (axis.HasFlag(TransformSnapshotAxis.Z))
                z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        /// <summary>
        ///     Writes the Vector3 with using <see cref="TransformSnapshotAxis"/>.
        /// </summary>
        public static void WriteVector3WithAxis(QNetMessageWriter writer, TransformSnapshotAxis axis, Vector3 vector)
        {
            writer.WriteSByte((sbyte) axis);
            if (axis.HasFlag(TransformSnapshotAxis.X))
                writer.WriteSingle(vector.x);
            if (axis.HasFlag(TransformSnapshotAxis.Y))
                writer.WriteSingle(vector.y);
            if (axis.HasFlag(TransformSnapshotAxis.Z))
                writer.WriteSingle(vector.z);
        }
    }

    // Transform interpolation of QNetInterpolation component.
    public sealed partial class QNetInterpolator
    {
        /// <summary>
        ///     Amount of smoothing applied to transform at final interpolation.
        /// </summary>
        /// <remarks>
        ///     When set to zero, no smoothing will be applied.
        /// </remarks>
        [Header("Transform Settings")]
        [Range(0f, 30f)]
        public float TransformSmoothing = 18f;

        /// <summary>
        ///     Defines what axis of position should be synchronized over network.
        /// </summary>
        [JEMEnumFlags]
        public TransformSnapshotAxis TransformPositionSync = TransformSnapshotAxis.X | TransformSnapshotAxis.Y | TransformSnapshotAxis.Z;

        /// <summary>
        ///     Defines what axis of rotation should be synchronized over network.
        /// </summary>
        [JEMEnumFlags]
        public TransformSnapshotAxis TransformRotationSync = TransformSnapshotAxis.X | TransformSnapshotAxis.Y | TransformSnapshotAxis.Z;

#if INCLUDE_TRANSFORM_SCALE_SYNC
        /// <summary>
        ///     Defines what axis of scale should be synchronized over network.
        /// </summary>
        [JEMEnumFlags]
        public TransformSnapshotAxis TransformScaleSync = TransformSnapshotAxis.X | TransformSnapshotAxis.Y | TransformSnapshotAxis.Z;
#endif

        private ObjectInterpolationHelper<TransformState, TransformResult, TransformSnapshot,  TransformInterpolator> _transformInterpolator;
        private QNetMessagePointer _receiveTransformPointer;
        private bool _firstAfterPool;

        // Load the transform interpolator.
        private void LoadTransform()
        {
            // Create the interpolator.
            if (Identity.WasPooled) return;
            _transformInterpolator = new ObjectInterpolationHelper<TransformState, TransformResult, TransformSnapshot, TransformInterpolator>(this);
            _receiveTransformPointer = GetMessagePointer(nameof(ReceiveTransform));

            // Register final interpolation
            _transformInterpolator.RegisterFinalInterpolation(snapshot =>
            {
                // Get time.
                var time = Time.deltaTime * TransformSmoothing;

                // Lerp unclamped the transform.
                transform.LerpUnclampedPosition(snapshot.Position, time);
                transform.LerpUnclampedRotation(snapshot.Rotation, time);
#if INCLUDE_TRANSFORM_SCALE_SYNC
                transform.LerpUnclampedLocalScale(snapshot.Scale, time);
#endif
            });

            // Register state reset.
            _transformInterpolator.RegisterStateReset((state, first) =>
            {
                // Apply transform state.
                transform.position = state.Position;
                transform.rotation = state.Rotation;
#if INCLUDE_TRANSFORM_SCALE_SYNC
                transform.localScale = state.Scale;
#endif
            });
        }

        private void PoolTransform() => _firstAfterPool = true; 

        // Apply the interpolated transform state.
        private void ApplyTransform() =>_transformInterpolator?.Apply();

        // Interpolate the transform.
        private void InterpolateTransform() => _transformInterpolator?.Interpolate();

        private bool _wasZero;

        // Send the transform state to interpolate on clients.
        private void SendTransform()
        {
            // To prevent jumping in to default position, we need to check if the transform is at default position in more than two frames.
            if (Vector3.Distance(transform.position, Vector3.zero) < 0.05f)
            {
                if (!_wasZero)
                {
                    _wasZero = true;
                }
                else
                {
                    return;
                }
            }
            else _wasZero = false;
 
            // Create snapshot.
            var snapshot = new TransformSerializedSnapshot
            {
                PositionAxis = TransformPositionSync,
                RotationAxis = TransformRotationSync,
#if INCLUDE_TRANSFORM_SCALE_SYNC
                ScaleAxis = TransformScaleSync,
#endif
                Snapshot = new TransformSnapshot
                {
                    Position = transform.position,
                    Rotation = transform.rotation,
#if INCLUDE_TRANSFORM_SCALE_SYNC
                    Scale = transform.localScale,
#endif
                    IsValid = true
                }                  
            };

            // Send to all.
            var writer = CreateNetworkMessage(true, _receiveTransformPointer, out var outgoingMessage);
            writer.WriteMessage(snapshot);
            writer.WriteUInt32(QNetTime.ServerFrame);
            outgoingMessage.SendMessageToAll();
        }

        // Receive transform state from server.
        [QNetMessage(Method =  QNetMessageMethod.Unreliable)]
        private void ReceiveTransform(QNetMessageReader reader)
        {
            var snapshot = reader.ReadMessage<TransformSerializedSnapshot>();
            var serverFrame = reader.ReadUInt32();
;
            _transformInterpolator?.AddSnapshot(snapshot.Snapshot, serverFrame, _firstAfterPool);
            _firstAfterPool = false;
        }
    }
}
