//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Interpolation.Interpolators;
using JEM.QNet.UnityEngine.Objects;
using JEM.QNet.UnityEngine.Simulation;
using System;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Interpolation
{
    public class EnumFlagsAttribute : PropertyAttribute
    {
        public EnumFlagsAttribute() { }
    }

#if UNITY_EDITOR
    // Source: https://answers.unity.com/questions/486694/default-editor-enum-as-flags-.html 
    [UnityEditor.CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect _position, UnityEditor.SerializedProperty _property, GUIContent _label)
        {
            _property.intValue = UnityEditor.EditorGUI.MaskField(_position, _label, _property.intValue, _property.enumNames);
        }
    }
#endif

    /// <summary>
    ///     Defines what axis transform interpolator should serialize over network.
    /// </summary>
    [Flags]
    public enum TransformSnapshotAxis : byte
    {
        X    = 1 << 0,
        Y    = 1 << 1,
        Z    = 1 << 2,
    }

    /// <inheritdoc />
    /// <summary>
    ///     Serialize snapshot data of transform interpolator.-
    /// </summary>
    public class TransformSerializedSnapshot : QNetSerializedMessage
    {
        public TransformSnapshotAxis PositionAxis;
        public TransformSnapshotAxis RotationAxis;
        public TransformSnapshotAxis ScaleAxis;

        public TransformSnapshot Snapshot;

        /// <inheritdoc />
        public override void Serialize(QNetMessageWriter writer)
        {
            WriteVector3WithAxis(writer, PositionAxis, Snapshot.Position);
            WriteVector3WithAxis(writer, RotationAxis, Snapshot.Rotation.eulerAngles);
            WriteVector3WithAxis(writer, ScaleAxis, Snapshot.Scale);
        }

        /// <inheritdoc />
        public override void DeSerialize(QNetMessageReader reader)
        {
            Snapshot = new TransformSnapshot
            {
                Position = ReadVector3WithAxis(reader),
                Rotation = Quaternion.Euler(ReadVector3WithAxis(reader)),
                Scale = ReadVector3WithAxis(reader)
            };
        }

        /// <summary>
        ///     Reads the Vector3 that has been serialized using <see cref="TransformSnapshotAxis"/>.
        /// </summary>
        public static Vector3 ReadVector3WithAxis(QNetMessageReader reader)
        {
            var axis = (TransformSnapshotAxis) reader.ReadByte();
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
            writer.WriteByte((byte) axis);
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
        [Header("Transform Settings")]
        [Range(0f, 30f)]
        public float TransformSmoothing = 18f;

        /// <summary>
        ///     Defines what axis of position should be synchronized over network.
        /// </summary>
        [EnumFlags]
        public TransformSnapshotAxis TransformPositionSync = TransformSnapshotAxis.X | TransformSnapshotAxis.Y | TransformSnapshotAxis.Z;

        /// <summary>
        ///     Defines what axis of rotation should be synchronized over network.
        /// </summary>
        [EnumFlags]
        public TransformSnapshotAxis TransformRotationSync = TransformSnapshotAxis.X | TransformSnapshotAxis.Y | TransformSnapshotAxis.Z;

        /// <summary>
        ///     Defines what axis of scale should be synchronized over network.
        /// </summary>
        [EnumFlags]
        public TransformSnapshotAxis TransformScaleSync = TransformSnapshotAxis.X | TransformSnapshotAxis.Y | TransformSnapshotAxis.Z;

        private ObjectInterpolationHelper<TransformState, TransformResult, TransformSnapshot,
            TransformInterpolator> _transformInterpolator;

        // Load the transform interpolator.
        private void LoadTransform()
        {
            // Create the interpolator.
            _transformInterpolator = new ObjectInterpolationHelper<TransformState, TransformResult,
                TransformSnapshot, TransformInterpolator>(this);

            // Register final interpolation
            _transformInterpolator.RegisterFinalInterpolation((snapshot, smooth) =>
            {
                var t = Time.deltaTime * smooth;
                transform.position = Vector3.LerpUnclamped(transform.position, snapshot.Position, t);
                transform.rotation = Quaternion.LerpUnclamped(transform.rotation, snapshot.Rotation, t);
                transform.localScale = Vector3.LerpUnclamped(transform.localScale, snapshot.Scale, t);
            });
        }

        // Apply the interpolated transform state.
        private void ApplyTransform() =>_transformInterpolator?.Apply(TransformSmoothing);

        // Interpolate the transform.
        private void InterpolateTransform() => _transformInterpolator?.Interpolate();

        // Send the transform state to interpolate on clients.
        private void SendTransform()
        {
            // Create snapshot.
            var snapshot = new TransformSerializedSnapshot
            {
                PositionAxis = TransformPositionSync,
                RotationAxis = TransformRotationSync,
                ScaleAxis = TransformScaleSync, 
                Snapshot =
                {
                    Position = transform.position,
                    Rotation = transform.rotation,
                    Scale = transform.localScale
                }                  
            };

            // Send to all.
            SendMessageToAll(nameof(ReceiveTransform), snapshot, QNetTime.ServerFrame);
        }

        // Receive transform state from server.
        [QNetMessage(Method =  QNetMessageMethod.Unreliable)]
        private void ReceiveTransform(TransformSerializedSnapshot snapshot, uint serverFrame)
        {
            // Write the snapshot to interpolate.
            _transformInterpolator?.AddSnapshot(snapshot.Snapshot, serverFrame);
        }
    }
}
