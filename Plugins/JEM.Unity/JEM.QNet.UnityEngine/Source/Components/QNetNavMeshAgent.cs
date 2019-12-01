//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Interpolation;
using JEM.QNet.UnityEngine.Interpolation.Interpolators;
using JEM.QNet.UnityEngine.Messages.Extensions;
using JEM.QNet.UnityEngine.Objects;
using JEM.QNet.UnityEngine.Simulation;
using JEM.UnityEngine.Extension;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace JEM.QNet.UnityEngine.Components
{
    public struct NavMeshAgentInputSample : IObjectInputSample
    {
        public uint ClientFrame { get; set; }

        public Vector3 Destination { get; set; }

        /// <inheritdoc />
        public void Serialize(QNetMessageWriter writer)
        {
            writer.WriteUInt32(ClientFrame);

            writer.WriteVector3(Destination);
        }

        /// <inheritdoc />
        public void DeSerialize(QNetMessageReader reader)
        {
            ClientFrame = reader.ReadUInt32();

            Destination = reader.ReadVector3();
        }
    }

    public struct NavMeshAgentSimulationResult : IObjectSimulationResult
    {
        public Vector3 Position { get; set; }
        public Vector3 Destination { get; set; }

        /// <inheritdoc />
        public void Serialize(QNetMessageWriter writer)
        {
            writer.WriteVector3(Position);
            writer.WriteVector3(Destination);
        }

        /// <inheritdoc />
        public void DeSerialize(QNetMessageReader reader)
        {
            Position = reader.ReadVector3();
            Destination = reader.ReadVector3();
        }

        /// <inheritdoc />
        public bool Compare(IObjectSimulationResult result, out float distance)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (!(result is NavMeshAgentSimulationResult myResult))
                throw new InvalidOperationException($"Failed to convert {nameof(IObjectSimulationResult)} " +
                                                    $"in to {nameof(NavMeshAgentSimulationResult)} type.");

            distance = Vector3.Distance(Position, myResult.Position);
            return distance < QNetSettings.PositionMaxMismatch;
        }
    }

    public struct NavMeshAgentSerializedSnapshot : IQNetSerializedMessage
    {
        public NavMeshAgentSnapshot Snapshot { get; set; }

        public void Serialize(QNetMessageWriter writer)
        {
            writer.WriteVector3(Snapshot.Position);
            writer.WriteSingle(Snapshot.Rotation);
        }

        public void DeSerialize(QNetMessageReader reader)
        {
            Snapshot = new NavMeshAgentSnapshot
            {
                Position = reader.ReadVector3(),
                Rotation = reader.ReadSingle()
            };
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Script that synchronizes <see cref="NavMeshAgent"/>
    ///     When used, all properties of <see cref="NavMeshAgent"/> should be accessed from this component instead.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent)), DisallowMultipleComponent]
    public sealed partial class QNetNavMeshAgent : QNetBehaviour
    {
        /// <summary>
        ///     Amount of smoothing applied to transform at final interpolation.
        /// </summary>
        [Header("Interpolation Settings")]
        [Range(0f, 30f)]
        public float TransformSmoothing = 18f;

        public float MaxDistance = 1f;

        /// <summary>
        ///     Reference to the <see cref="NavMeshAgent"/> component.
        /// </summary>
        public NavMeshAgent Agent { get; private set; }

        /// <summary>
        ///     Currently target destination.
        /// </summary>
        private Vector3 _destination;

        /// <inheritdoc />
        /// <remarks>
        ///     As <see cref="LoadMethods"/> is called right after the object spawn, we can collect the references to the components here.
        /// </remarks>
        protected override void LoadMethods()
        {
            // Invoke base method.
            base.LoadMethods();

            // Collect component.
            Agent = GetComponent<NavMeshAgent>();
        }

        private ObjectInterpolationHelper<NavMeshAgentState, NavMeshAgentResult, NavMeshAgentSnapshot, NavMeshAgentInterpolator> _navMeshAgentInterpolator;
        private QNetMessagePointer _receiveNavMeshAgentPointer;

        private void OnNetworkSpawned()
        {
            // Simulate server object!
            SimulateServerObject = true;
            AllowClientSidePrediction = true;

            // Create the interpolator.
            _navMeshAgentInterpolator = new ObjectInterpolationHelper<NavMeshAgentState, NavMeshAgentResult,  NavMeshAgentSnapshot, NavMeshAgentInterpolator>(this);
            _receiveNavMeshAgentPointer = GetMessagePointer(nameof(ReceiveNavMeshAgent));

            // Register final interpolation
            _navMeshAgentInterpolator.RegisterFinalInterpolation((snapshot) =>
            {
                // Get time.
                // var time = Time.deltaTime * TransformSmoothing;

                if (IsServer)
                    return;

                var distance = Vector3.Distance(transform.position, snapshot.Position);
                if (distance > MaxDistance)
                {
                    transform.position = snapshot.Position;
                    Debug.Log(distance, this);
                }
            });
        }

        private void OnSerializeServerState(QNetMessageWriter writer)
        {
            writer.WriteInt32(Agent.agentTypeID);
            writer.WriteSingle(Agent.baseOffset);

            writer.WriteSingle(Agent.speed);
            writer.WriteSingle(Agent.angularSpeed);
            writer.WriteSingle(Agent.acceleration);
            writer.WriteSingle(Agent.stoppingDistance);
            writer.WriteBool(Agent.autoBraking);

            writer.WriteSingle(Agent.radius);
            writer.WriteSingle(Agent.height);
            writer.WriteEnum(Agent.obstacleAvoidanceType);
            writer.WriteInt32(Agent.avoidancePriority);

            writer.WriteBool(Agent.autoTraverseOffMeshLink);
            writer.WriteBool(Agent.autoRepath);
            writer.WriteInt32(Agent.areaMask);
        }

        private void OnDeserializeServerState(QNetMessageReader reader)
        {
            Agent.agentTypeID = reader.ReadInt32();
            Agent.baseOffset = reader.ReadSingle();

            Agent.speed = reader.ReadSingle();
            Agent.angularSpeed = reader.ReadSingle();
            Agent.acceleration = reader.ReadSingle();
            Agent.stoppingDistance = reader.ReadSingle();
            Agent.autoBraking = reader.ReadBool();

            Agent.radius = reader.ReadSingle();
            Agent.height = reader.ReadSingle();
            Agent.obstacleAvoidanceType = reader.ReadEnum<ObstacleAvoidanceType>();
            Agent.avoidancePriority = reader.ReadInt32();

            Agent.autoTraverseOffMeshLink = reader.ReadBool();
            Agent.autoRepath = reader.ReadBool();
            Agent.areaMask = reader.ReadInt32();
        }

        private void UnsafeSimulate() => _navMeshAgentInterpolator?.Apply();

        // Interpolate the transform.
        private void InterpolateTransform() => _navMeshAgentInterpolator?.Interpolate();

        private void OnSendSnapshot()
        {
            // Create snapshot.
            var snapshot = new NavMeshAgentSerializedSnapshot
            {
                Snapshot = new NavMeshAgentSnapshot
                {
                    Position = transform.position,
                    Rotation = transform.rotation.eulerAngles.y
                }
            };

            // Send to all.
            // SendMessageToAll(nameof(ReceiveTransform), snapshot, QNetTime.ServerFrame);
            var writer = CreateNetworkMessage(true, _receiveNavMeshAgentPointer, out var outgoingMessage);
            writer.WriteMessage(snapshot);
            writer.WriteUInt32(QNetTime.ServerFrame);
            outgoingMessage.SendMessageToAll();
        }

        // Receive transform state from server.
        [QNetMessage(Method = QNetMessageMethod.Unreliable)]
        private void ReceiveNavMeshAgent(QNetMessageReader reader)
        {
            var snapshot = reader.ReadMessage<NavMeshAgentSerializedSnapshot>();
            var serverFrame = reader.ReadUInt32();

            // Write the snapshot to interpolate.
            _navMeshAgentInterpolator?.AddSnapshot(snapshot.Snapshot, serverFrame);
        }

        private void OnSampleInput(out IObjectInputSample inputSample)
        {
            // Collect your input.
            inputSample = new NavMeshAgentInputSample
            {
                Destination = _destination
            };
        }

        private void OnResetState(IObjectSimulationResult results)
        {
            if (results is NavMeshAgentSimulationResult myResults)
            {
                // Reset object state!
                transform.position = myResults.Position;
                _destination = myResults.Destination;
            }
        }

        private IObjectSimulationResult OnSimulateWithResult(IObjectInputSample inputSample)
        {
            if (inputSample is NavMeshAgentInputSample myInput)
            {
                // Make sure that updatePosition is disabled.
                Agent.updatePosition = false;

                // Set destination
                Agent.SetDestination(myInput.Destination);

                // Apply next position
                transform.position = Agent.nextPosition;

                // Snap position for minimal error
                transform.position = transform.position.Snap(QNetSettings.PositionGridSnap);
            }

            // Simulate your object!
            return new NavMeshAgentSimulationResult {Position = transform.position, Destination = Agent.destination};
        }
    }
}
