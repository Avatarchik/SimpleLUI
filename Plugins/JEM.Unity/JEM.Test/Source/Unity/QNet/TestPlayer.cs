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
    /// <inheritdoc />
    /// <summary>
    ///     A example player input.
    /// </summary>
    internal class TestInput : ObjectInputSample
    {
        internal bool Forward;
        internal bool Backward;
        internal bool Left;
        internal bool Right;

        /// <inheritdoc />
        public override void Serialize(QNetMessageWriter writer)
        {
            // Invoke base method to serialize frame.
            base.Serialize(writer);

            writer.WriteBool(Forward);
            writer.WriteBool(Backward);
            writer.WriteBool(Left);
            writer.WriteBool(Right);
        }

        /// <inheritdoc />
        public override void DeSerialize(QNetMessageReader reader)
        {
            // Invoke base method to de-serialize frame.
            base.DeSerialize(reader);

            Forward = reader.ReadBool();
            Backward = reader.ReadBool();
            Left = reader.ReadBool();
            Right = reader.ReadBool();
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     A example results of player simulation.
    /// </summary>
    internal class TestResult : ObjectSimulationResult
    {
        internal Vector3 Position { get; set; } = Vector3.zero;

        /// <inheritdoc />
        public override void Serialize(QNetMessageWriter writer)
        {
            writer.WriteVector3(Position);
        }

        /// <inheritdoc />
        public override void DeSerialize(QNetMessageReader reader)
        {
            Position = reader.ReadVector3();
        }

        /// <inheritdoc />
        public override bool Compare(ObjectSimulationResult result, out float distance)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (!(result is TestResult testResult))
                throw new InvalidOperationException($"Failed to convert {nameof(ObjectSimulationResult)} " +
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
            if (IsServer)
            {
                Test = gameObject.AddComponent<TestAnotherBehaviour>();
            }
            else
            {
                Test = gameObject.GetComponent<TestAnotherBehaviour>();
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

        private void OnSampleInput(out TestInput inputSample)
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

        private void OnResetState(TestResult results)
        {
            // Reset object state!
            transform.position = results.Position;
        }

        private TestResult OnSimulateWithResult(TestInput inputSample)
        {
            var horizontal = inputSample.Left ? 1f : inputSample.Right ? -1f : 0f;
            var vertical = inputSample.Forward ? 1f : inputSample.Backward ? -1f : 0f;

            transform.Translate(new Vector3(horizontal, 0f, vertical) * Time.fixedDeltaTime * SpeedMod);

            // Simulate your object!
            return new TestResult {Position = transform.position};
        }
    }
}
