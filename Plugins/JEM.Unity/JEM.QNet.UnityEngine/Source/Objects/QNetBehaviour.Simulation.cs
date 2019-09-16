//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

//
// Original Network simulation design and execution
//  by Damian 'Erdroy' Korczowski (https://github.com/Erdroy)
//

using JEM.Core.Common;
using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Simulation;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JEM.QNet.UnityEngine.Objects
{
    /// <inheritdoc />
    /// <summary>
    ///     Base class for object input sample used for simulation.
    /// </summary>
    public abstract class ObjectInputSample : QNetSerializedMessage
    {
        /// <summary>
        ///     Client frame this input sample was generated from.
        /// </summary>
        public uint ClientFrame { get; internal set; }

        protected ObjectInputSample() { }

        /// <inheritdoc />
        public override void Serialize(QNetMessageWriter writer)
        {
            writer.WriteUInt32(ClientFrame);   
        }

        /// <inheritdoc />
        public override void DeSerialize(QNetMessageReader reader)
        {
            ClientFrame = reader.ReadUInt32();
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Base class for object simulation result.
    /// </summary>
    public abstract class ObjectSimulationResult : QNetSerializedMessage
    {
        protected ObjectSimulationResult() { }

        /// <summary>
        ///     Compare two results.
        ///     Return true if everything is ok.
        /// </summary>
        public abstract bool Compare([NotNull] ObjectSimulationResult result, out float distance);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Object simulation correction data.
    /// </summary>
    internal sealed class ObjectSimulationCorrection : QNetSerializedMessage
    {
        /// <summary>
        ///     A simulation result this object correction wants to apply.
        /// </summary>
        public ObjectSimulationResult Result { get; set; }

        /// <summary>
        ///     The frame this correction was generated on.
        /// </summary>
        public uint Frame { get; internal set; }

        /// <inheritdoc />
        public override void Serialize(QNetMessageWriter writer)
        {
            if (QNetBehaviour.GetSerializedMessageIndex(Result.GetType(), out byte typeIndex))
            {
                writer.WriteByte(typeIndex);
            }
            else throw new NullReferenceException("Failed to serialize ObjectSimulationCorrection class. " +
                                                  $"Unable to find typeIndex of type {Result.GetType().FullName}.");

            writer.WriteMessage(Result);
            writer.WriteUInt32(Frame);
        }

        /// <inheritdoc />
        public override void DeSerialize(QNetMessageReader reader)
        {
            var typeIndex = reader.ReadByte();
            if (QNetBehaviour.GetSerializedMessageType(typeIndex, out var resultType))
            {
                Result = reader.ReadMessage(resultType) as ObjectSimulationResult;
            }
            else throw new NullReferenceException("Failed to deserialize ObjectSimulationCorrection class. " +
                                                  $"Unable to find type of index {typeIndex}.");

            Frame = reader.ReadUInt32();
        }
    }

    /// <summary>
    ///     Object simulation command.
    /// </summary>
    public class ObjectSimulationCommand
    {
        /// <summary>
        ///     Command input sample.
        /// </summary>
        public ObjectInputSample Input { get; set; }

        /// <summary>
        ///     Command simulation result.
        /// </summary>
        public ObjectSimulationResult Result { get; set; }

        /// <summary>
        ///     First execution state.
        ///     If true, this is most likely a simulation correction.
        /// </summary>
        public bool IsFirstExecution { get; set; }

        /// <summary>
        ///     Should we reset the state?
        /// </summary>
        public bool ResetState { get; set; }

        /// <summary>
        ///     Command execution frame.
        /// </summary>
        public uint ServerFrame { get; set; }
    }

    // QNetBehaviour simulation stuff.
    public abstract partial class QNetBehaviour
    {
        /// <summary>
        ///     Defines if entity can be simulated.
        /// </summary>
        internal bool CanSimulate => _hasMethodsToSimulate && QNetTime.ServerFrame > _simulationSkip && (IsServer || HasStateDeserialized);
        private uint _simulationSkip;

        private bool _canQueueCommands = true;
        private uint _lastClientUpdateFrame;

        private readonly Queue<ObjectSimulationCommand> _commandQueue = new Queue<ObjectSimulationCommand>();
        private List<ObjectInputSample> _inputHistory = new List<ObjectInputSample>();

        /// <summary>
        ///     False, if this QNetBehaviour based component does not have defined all the methods needed to simulate the object.
        /// </summary>
        private bool _hasMethodsToSimulate;

        /// <summary>
        ///     OnSampleInput called by <see cref="SimulateFrame"/> to sample the input for current frame simulation.
        ///     Called only for the owner of object.
        /// </summary>
        private JEMSmartMethod _onSampleInput;

        /// <summary>
        ///     OnResetState called by <see cref="ExecuteCommand"/> to restart the object state to received result.
        ///     Usually result of a invalid client simulation result.
        /// </summary>
        private JEMSmartMethod _onResetState;

        /// <summary>
        ///     OnAdjustCameraState called by <see cref="ExecuteCommand"/> when the client-side prediction is disabled
        ///      and server need to adjust the camera state.
        ///     Invoked right after OnResetState.
        /// </summary>
        private JEMSmartMethod _onAdjustCameraState;

        /// <summary>
        ///     OnSimulateWithResult called on both server and client(if prediction enabled) to simulate current frame.
        /// </summary>
        /// <remarks>
        ///     You should only simulate core movement mechanics that can be predicted in this method.
        /// </remarks>
        private JEMSmartMethod _onSimulateWithResult;

        /// <summary>
        ///     OnSimulateFirstExecution called on both server and client to simulate the rest of object logic. ex.: animations, weapons...
        /// </summary>
        private JEMSmartMethod _onSimulateFirstExecution;

        /// <summary>
        ///     OnSimulateOwner is called to simulate the object on server side only.
        /// </summary>
        private JEMSmartMethod _onSimulateServer;

        /// <summary>
        ///     OnSimulateClient is called to simulate the object on the client that is not a owner and the server is not active.
        /// </summary>
        private JEMSmartMethod _onSimulateClient;

        private void LoadSimulationMethods()
        {
            // Setup input history of length at most one second.
            _inputHistory = new List<ObjectInputSample>(QNetTime.TickRate);

            // Collect methods.
            _onSampleInput = new JEMSmartMethod(this, "OnSampleInput");

            _onResetState = new JEMSmartMethod(this, "OnResetState");
            _onAdjustCameraState = new JEMSmartMethod(this, "OnAdjustCameraState");

            _onSimulateWithResult = new JEMSmartMethod(this, "OnSimulateWithResult");
            _onSimulateFirstExecution = new JEMSmartMethod(this, "OnSimulateFirstExec");

            _onSimulateServer = new JEMSmartMethod(this, "OnSimulateServer");
            _onSimulateClient = new JEMSmartMethod(this, "OnSimulateClient");

            // Check if we can simulate.
            _hasMethodsToSimulate = _onSampleInput.IsValid() && _onResetState.IsValid() && _onSimulateWithResult.IsValid();
        }

        /// <inheritdoc />
        internal override void InterpolateFrame()
        {
            if (!CanSimulate)
            {
                return;
            }
         
            // TODO: Interpolate
        }

        /// <inheritdoc />
        internal override void SimulateFrame()
        {
            if (!CanSimulate)
            {
                return;
            }

            // Call onSimulateServer
            if (IsServer)
            {
                _onSimulateServer.Invoke();
            }

            if (IsOwner)
            {
                // Sample input.
                object[] p = {null};
                _onSampleInput.Invoke(p);
                var inputSample = (ObjectInputSample) p[0];
                // Set frame.
                inputSample.ClientFrame = QNetTime.Frame;

                // Create command.
                var command = new ObjectSimulationCommand
                {
                    IsFirstExecution = true,
                    ResetState = false,
                    ServerFrame = QNetTime.ServerFrame,
                    Input = inputSample
                };

                // Queue command for client-side prediction.
                QueueCommand(command);
            }

            // Call onSimulateClient
            if (!IsServer && !IsOwner)
            {
                _onSimulateClient.Invoke();
            }

            // Execute commands.
            _canQueueCommands = false;
            try
            {
                ExecuteCommands();
            }
            catch (Exception e)
            {
                QNetManager.PrintLogException(e, this);
                throw;
            }
            finally
            {
                _commandQueue.Clear();
                _canQueueCommands = true;
            }
        }

        private void ExecuteCommands()
        {
            // Execute commands
            foreach (var command in _commandQueue)
            {
                var clientResults = command.Result;

                // Execute current command.
                ExecuteCommand(command, command.ResetState);

                if (IsOwner && command.IsFirstExecution && !IsServer)
                {
                    // Send input and results to server
                    // Note: Should be called only when this command was executed
                    // The first time (i.e.: not a move correction command).
                    SendMessage(nameof(ClientRequestsSimulation), command.Input, command.Result);
                }

                if (IsServer && !IsOwner)
                {
                    var serverResults = command.Result;
                    if (!serverResults.Compare(clientResults, out var resultDistance))
                    {
#if DEBUG
                        QNetManager.PrintLogWarning($"The server will send correction because of client({Identity.Owner}) error. ({resultDistance})", this);
#endif

                        // Send back server results.
                        var correction = new ObjectSimulationCorrection
                        {
                            Result =  serverResults,
                            Frame = QNetTime.ServerFrame
                        };

                        // Send the message to owner of object.
                        SendMessage(Identity, nameof(ServerCorrectsSimulation), correction);
                    }
                }
            }
            // _commandQueue.Clear();
        }

        /// <summary>
        ///     Executes the target command.
        /// </summary>
        protected virtual void ExecuteCommand(ObjectSimulationCommand command, bool resetState)
        {
            var disableClientPrediction = IsServer && !QNetSettings.ClientSidePrediction;

            if (resetState || disableClientPrediction)
            {
                // Note: When client-side prediction is disabled,
                // We use the reset state on server to apply client-side position.
                // Additionally we must set yaw angle.

                // Reset state.
                _onResetState.Invoke(command.Result);

                // Adjust camera state.
                if (disableClientPrediction)
                    _onAdjustCameraState.Invoke(command.Input);
            }

            if (IsClient || !disableClientPrediction)
            {
                // Simulate authoritative movement and setup result.
                command.Result = _onSimulateWithResult.Invoke(command.Input) as ObjectSimulationResult;
                if (command.Result == null)
                    throw new NullReferenceException("Method OnSimulateWithResult has returned a null value or " +
                                                     "value that can't be converted in to ObjectSimulationResult type.");
            }

            if (command.IsFirstExecution)
            {
                // Simulate the rest of the object logic.
                _onSimulateFirstExecution.Invoke(command);
            }
        }

        /// <summary>
        ///     Queues the command.
        /// </summary>
        protected void QueueCommand(ObjectSimulationCommand command)
        {
            QNetManager.PrintLogAssert(_canQueueCommands, "Cannot queue commands from the current context!", this);

            if (IsOwner && command.IsFirstExecution)
            {
                // Add input to the history.
                // Note: This is needed only for the owner, as owner
                // must replay all the input commands when receives movement correction.
                while (_inputHistory.Count + 1 >= _inputHistory.Capacity)
                    _inputHistory.RemoveAt(0);

                _inputHistory.Add(command.Input);
            }

            // Queue input.
            _commandQueue.Enqueue(command);
        }

        [QNetMessage(Method = QNetMessageMethod.Unreliable)]
        private void ClientRequestsSimulation<TInputSample, TSimulationResult>(TInputSample sample, TSimulationResult clientResult) 
            where TInputSample : ObjectInputSample where TSimulationResult : ObjectSimulationResult
        {
            if (!CanSimulate)
            {
#if DEBUG
                QNetManager.PrintLogMsc("Refusing to simulate request.", this);
#endif
                return;
            }

            // Discard old frames (out-of order)
            // This is naive check, can be hacked, but this should only check ordering.
            if (sample.ClientFrame <= _lastClientUpdateFrame)
            {
#if DEBUG
                QNetManager.PrintLogMsc("Dropping client movement update due to message being out-of-order", this);
#endif
                return;
            }
            _lastClientUpdateFrame = sample.ClientFrame;

            // Setup command
            var command = new ObjectSimulationCommand
            {
                Input = sample,
                Result = clientResult,
                ServerFrame = QNetTime.ServerFrame,
                IsFirstExecution = true
            };

            // Queue command.
            QueueCommand(command);
        }

        [QNetMessage(Method = QNetMessageMethod.Unreliable)]
        private void ServerCorrectsSimulation(ObjectSimulationCorrection correct)
        {
            if (IsServer)
            {
                // Ignore correction on server.
                return; 
            }

            // Clear current commands (should be empty, tho).
            _commandQueue.Clear();

            // Queue command.
            QueueCommand(new ObjectSimulationCommand
            {
                IsFirstExecution = false,
                ResetState = true,
                ServerFrame = QNetTime.ServerFrame,
                Input = _inputHistory.LastOrDefault(),
                Result = correct.Result
            });
        }
    }
}
