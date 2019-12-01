//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Messages.Extensions;
using JEM.QNet.UnityEngine.Objects;
using JEM.QNet.UnityEngine.Simulation;
using System;
using UnityEngine;

namespace JEM.Test.Unity.QNet
{
    /// <inheritdoc cref="IObjectInputSample" />
    /// <summary>
    ///     A example player input.
    /// </summary>
    internal struct TestInput : IObjectInputSample
    {
        public uint ClientFrame { get; set; }

        internal bool Forward { get; set; }
        internal bool Backward { get; set; }
        internal bool Left { get; set; }
        internal bool Right { get; set; }

        /// <inheritdoc />
        public void Serialize(QNetMessageWriter writer)
        {
            writer.WriteUInt32(ClientFrame);

            writer.WriteBool(Forward);
            writer.WriteBool(Backward);
            writer.WriteBool(Left);
            writer.WriteBool(Right);
        }

        /// <inheritdoc />
        public void DeSerialize(QNetMessageReader reader)
        {
            ClientFrame = reader.ReadUInt32();

            Forward = reader.ReadBool();
            Backward = reader.ReadBool();
            Left = reader.ReadBool();
            Right = reader.ReadBool();
        }
    }

    /// <inheritdoc cref="IObjectSimulationResult" />
    /// <summary>
    ///     A example results of player simulation.
    /// </summary>
    internal struct TestResult : IObjectSimulationResult
    {
        internal Vector3 Position { get; set; }

        /// <inheritdoc />
        public void Serialize(QNetMessageWriter writer)
        {
            writer.WriteVector3(Position);
        }

        /// <inheritdoc />
        public void DeSerialize(QNetMessageReader reader)
        {
            Position = reader.ReadVector3();
        }

        /// <inheritdoc />
        public bool Compare(IObjectSimulationResult result, out float distance)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (!(result is TestResult testResult))
                throw new InvalidOperationException($"Failed to convert {nameof(IObjectSimulationResult)} " +
                                                    $"in to {nameof(TestResult)} type.");

            distance = Vector3.Distance(Position, testResult.Position);
            return distance < QNetSettings.PositionMaxMismatch;
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Test player network script.
    ///     Implements simple authoritative network movement.
    /// </summary>
    internal class TestPlayer : QNetBehaviour
    {
        public float SpeedMod = 5f;

        internal bool Forward;
        internal bool Backward;
        internal bool Left;
        internal bool Right;

        public TestAnotherBehaviour Test;

        private void OnNetworkSpawned()
        {
            // Test runtime component add.
            if (IsServer && !Identity.WasPooled)
            {
                Test = gameObject.AddComponent<TestAnotherBehaviour>();
            }
            else
            {
                Test = gameObject.GetComponent<TestAnotherBehaviour>();
            }

            if (IsOwner)
            {
                ActivePlayer = this;
            }
        }

        private void UnsafeSimulate()
        {
            if (IsServer)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Destroy(Test);
                }
            }

            // Collect the actual input in unsafe simulate.
            Forward = Input.GetKey(KeyCode.W);
            Backward = Input.GetKey(KeyCode.S);
            Left = Input.GetKey(KeyCode.A);
            Right = Input.GetKey(KeyCode.D);
        }

        private void OnSampleInput(out IObjectInputSample inputSample)
        {
            // Collect your input.
            inputSample = new TestInput
            {
                Forward = Forward,
                Backward = Backward,
                Left = Left,
                Right = Right
            };
        }

        private void OnResetState(IObjectSimulationResult results)
        {
            if (results is TestResult myResults)
            {
                // Reset object state!
                transform.position = myResults.Position;
            }
        }

        private IObjectSimulationResult OnSimulateWithResult(IObjectInputSample inputSample)
        {
            if (inputSample is TestInput myInput)
            {
                var horizontal = myInput.Left ? 1f : myInput.Right ? -1f : 0f;
                var vertical = myInput.Forward ? 1f : myInput.Backward ? -1f : 0f;

                transform.Translate(new Vector3(horizontal, 0f, vertical) * Time.fixedDeltaTime * SpeedMod);
            }

            // Simulate your object!
            return new TestResult {Position = transform.position};
        }

        public static TestPlayer ActivePlayer { get; private set; }
    }
}
