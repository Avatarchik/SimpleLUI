//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

//
// Original Network simulation design and implementation
//  by Damian 'Erdroy' Korczowski (https://github.com/Erdroy)
//

#define DEEP_DEBUG

using JEM.Core.Common;
using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Simulation;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace JEM.QNet.UnityEngine.Objects
{
    /// <inheritdoc />
    /// <summary>
    ///     Base class for object input sample used for simulation.
    /// </summary>
    public interface IObjectInputSample : IQNetSerializedMessage
    {
        /// <summary>
        ///     Client frame this input sample was generated from.
        /// </summary>
        uint ClientFrame { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Base class for object simulation result.
    /// </summary>
    public interface IObjectSimulationResult : IQNetSerializedMessage
    {
        /// <summary>
        ///     Compare two results.
        ///     Return true if everything is ok.
        /// </summary>
        bool Compare([NotNull] IObjectSimulationResult result, out float distance);
    }

    /// <inheritdoc cref="IQNetSerializedMessage" />
    /// <summary>
    ///     Object simulation correction data.
    /// </summary>
    internal struct ObjectSimulationCorrection : IQNetSerializedMessage
    {
        /// <summary>
        ///     A simulation result this object correction wants to apply.
        /// </summary>
        public IObjectSimulationResult Result { get; set; }

        /// <summary>
        ///     The server frame this correction was generated on.
        /// </summary>
        public uint ServerFrame { get; internal set; }

        /// <summary>
        ///     A client of input this correction was generated from.
        /// </summary>
        public uint ClientFrame { get; internal set; }

        /// <inheritdoc />
        public void Serialize(QNetMessageWriter writer)
        {
            writer.WriteMessage(Result, true);
            writer.WriteUInt32(ServerFrame);
            writer.WriteUInt32(ClientFrame);
        }

        /// <inheritdoc />
        public void DeSerialize(QNetMessageReader reader)
        {
            Result = reader.ReadMessage() as IObjectSimulationResult;
            ServerFrame = reader.ReadUInt32();
            ClientFrame = reader.ReadUInt32();
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
        public IObjectInputSample Input { get; set; }

        /// <summary>
        ///     Command simulation result.
        /// </summary>
        public IObjectSimulationResult Result { get; set; }

        /// <summary>
        ///     First execution state.
        ///     If true, this is most likely a simulation correction.
        /// </summary>
        public bool IsFirstExecution { get; set; }

        /// <summary>
        ///     Should we reset the state?
        /// </summary>
        public bool ResetState { get; set; }
    }

    // QNetBehaviour simulation stuff.
    public abstract partial class QNetBehaviour
    {
        /// <summary>
        ///     Controls if entity should be simulated if it's a server object.
        ///     When true, by default the simulation will be done on both server and all clients.
        ///     To simulate object only on the server set <see cref="AllowClientSidePrediction"/> to false.
        /// </summary>
        public bool SimulateServerObject { get; set; } = false;

        /// <summary>
        ///     Local flag that tells if this <see cref="QNetBehaviour"/> based component can use client side prediction.
        /// </summary>
        public bool AllowClientSidePrediction { get; set; } = true;

        /// <summary>
        ///     Defines if entity can be simulated.
        /// </summary>
        public bool CanSimulate => _hasMethodsToSimulate && QNetTime.ServerFrame > _simulationSkip && (IsServer || HasStateDeserialized);

        /// <summary>
        ///     State of client side prediction.
        ///     False if this <see cref="QNetBehaviour"/> based object can't use this feature.
        ///     While prediction is disabled, client will be interpolated by server instead.
        /// </summary>
        public bool ClientSidePrediction => QNetSettings.ClientSidePrediction && AllowClientSidePrediction;

        /// <summary>
        ///     True if this <see cref="QNetBehaviour"/> based object is a server object and can be simulated in local peer.
        /// </summary>
        public bool CanSimulateServerObject => Identity.IsServerObject && SimulateServerObject && (IsServer || IsClient && AllowClientSidePrediction);

        // TODO: Create method that allow to set this field so local peer cloud skip the simulation to given frame
        // TODO:  (by skipping we mean ignoring all the corrections received by client or requests to simulate received by client).
        private uint _simulationSkip;

        private bool _canQueueCommands = true;
        private uint _lastClientUpdateFrame;

        private readonly Queue<ObjectSimulationCommand> _commandQueue = new Queue<ObjectSimulationCommand>();
        private List<IObjectInputSample> _inputHistory = new List<IObjectInputSample>();

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
        private JEMSmartMethodS<IObjectSimulationResult> _onResetState;

        /// <summary>
        ///     OnAdjustCameraState called by <see cref="ExecuteCommand"/> when the client-side prediction is disabled
        ///      and server need to adjust the camera state.
        ///     Invoked right after OnResetState.
        /// </summary>
        private JEMSmartMethodS<IObjectInputSample> _onAdjustCameraState;

        /// <summary>
        ///     OnSimulateWithResult called on both server and client(if prediction enabled) to simulate current frame.
        /// </summary>
        /// <remarks>
        ///     You should only simulate core movement mechanics that can be predicted in this method.
        /// </remarks>
        private JEMSmartMethodR<IObjectSimulationResult, IObjectInputSample> _onSimulateWithResult;

        /// <summary>
        ///     OnSimulateFirstExecution called on both server and client to simulate the rest of object logic. ex.: animations, weapons...
        /// </summary>
        private JEMSmartMethodS<ObjectSimulationCommand> _onSimulateFirstExecution;

        /// <summary>
        ///     OnSimulateOwner is called to simulate the object on server side only.
        /// </summary>
        private JEMSmartMethodS _onSimulateServer;

        /// <summary>
        ///     OnSimulateClient is called to simulate the object on the client that is not a owner and the server is not active.
        /// </summary>
        private JEMSmartMethodS _onSimulateClient;

        private QNetMessagePointer _clientRequestSimulationPointer;
        private QNetMessagePointer _serverCorrectsSimulationPointer;

        private void LoadSimulationMethods()
        {
            Profiler.BeginSample("QNetBehaviour.LoadSimulationMethods");

            // Setup input history of length at most one second.
            _inputHistory = new List<IObjectInputSample>(QNetTime.TickRate);

            // Collect methods.
            _onSampleInput = new JEMSmartMethod(this, "OnSampleInput");

            _onResetState = new JEMSmartMethodS<IObjectSimulationResult>(this, "OnResetState");
            _onAdjustCameraState = new JEMSmartMethodS<IObjectInputSample>(this, "OnAdjustCameraState");

            _onSimulateWithResult = new JEMSmartMethodR<IObjectSimulationResult, IObjectInputSample>(this, "OnSimulateWithResult");
            _onSimulateFirstExecution = new JEMSmartMethodS<ObjectSimulationCommand>(this, "OnSimulateFirstExec");

            _onSimulateServer = new JEMSmartMethodS(this, "OnSimulateServer");
            _onSimulateClient = new JEMSmartMethodS(this, "OnSimulateClient");

            // Check if we can simulate.
            // We can only simulate if all core methods for simulation are implemented in to the object.
            _hasMethodsToSimulate = _onSampleInput.IsValid() && _onResetState.IsValid() && _onSimulateWithResult.IsValid();

            // Get message pointers.
            _clientRequestSimulationPointer = GetMessagePointer(nameof(ClientRequestsSimulation));
            _serverCorrectsSimulationPointer = GetMessagePointer(nameof(ServerCorrectsSimulation));

            Profiler.EndSample();
        }

        /* No need to implement object interpolation internaly. User can write his own code or use QNetInterpolator
        /// <inheritdoc />
        internal override void InterpolateFrame()
        {
            if (!CanSimulate)
            {
                return;
            }
         
            // TODO: Interpolate
        }
        */

        // UPDATE: Override InterpolateFrame so the object extending QNetBehaviour dont need to.
        /// <inheritdoc />
        public override void InterpolateFrame() { }

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

            if (IsOwner || CanSimulateServerObject)
            {
                // Sample input.
                object[] p = { null };
                _onSampleInput.Invoke(p);
                var inputSample = (IObjectInputSample)p[0];
                // Set frame.
                inputSample.ClientFrame = QNetTime.Frame;

                // Create command.
                var command = new ObjectSimulationCommand
                {
                    IsFirstExecution = true,
                    ResetState = false,
                    Input = inputSample
                };

                // Queue command for client-side prediction.
                QueueCommand(command);
            }

            // Call onSimulateClient
            if (!IsServer && !IsOwner || CanSimulateServerObject)
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

                    // SendMessage(nameof(ClientRequestsSimulation), command.Input, command.Result);
                    var writer = CreateNetworkMessage(false, _clientRequestSimulationPointer, out var outgoingMessage);
                    writer.WriteMessage(command.Input, true);
                    writer.WriteMessage(command.Result, true);
                    outgoingMessage.SendMessage();
                }

                // Correct only when this is not a host owned object but still a player owned.
                if (IsServer && !IsOwner && !CanSimulateServerObject)
                {
                    var serverResults = command.Result;
                    var isGood = serverResults.Compare(clientResults, out var resultDistance);
                    if (!isGood)
                    {
#if DEBUG
#if DEEP_DEBUG
                        QNetManager.PrintLogWarning($"The server will send correction because of client({Identity.Owner}) error. ({resultDistance:0.000})", this);
#endif
                        TotalSimulationCorrections++;
#endif

                        // Send back server results.
                        var correction = new ObjectSimulationCorrection
                        {
                            Result = serverResults,
                            ServerFrame = QNetTime.ServerFrame,
                            ClientFrame = command.Input.ClientFrame
                        };

                        // Send the message to owner of object.
                        // SendMessage(Identity, nameof(ServerCorrectsSimulation), correction);
                        var writer = CreateNetworkMessage(true, _serverCorrectsSimulationPointer, out var outgoingMessage);
                        writer.WriteMessage(correction);
                        outgoingMessage.SendMessage(Identity);
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
            // Get if client prediction is disabled.
            // As server this will be always true.
            var disableClientPrediction = IsServer && !ClientSidePrediction;

            if (resetState)
            {
                // Reset state.
                _onResetState.Invoke(command.Result);

                // Adjust camera state.
                if (disableClientPrediction)
                    _onAdjustCameraState.Invoke(command.Input);

                return;
            }

            // Simulate authoritative movement and setup result.
            // This can be done only on server or on client when client prediction is enabled.
            if (IsServer || ClientSidePrediction)
            {
                command.Result = _onSimulateWithResult.Invoke(command.Input);
                if (command.Result == null)
                    throw new NullReferenceException("Method OnSimulateWithResult has returned a null value or " +
                                                     "value that can't be converted in to ObjectSimulationResult type.");

                if (command.IsFirstExecution)
                {
                    // Simulate the rest of the object logic.
                    _onSimulateFirstExecution.Invoke(command);
                }
            }
        }

        /// <summary>
        ///     Queues the command.
        /// </summary>
        protected void QueueCommand(ObjectSimulationCommand command)
        {
            QNetManager.PrintLogAssert(_canQueueCommands, "Cannot queue commands from the current context!", this);

            // Update the history only if local peer is a owner of this object or this is a server object that could be simulated.
            if ((IsOwner || CanSimulateServerObject) && command.IsFirstExecution)
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
        private void ClientRequestsSimulation(QNetMessageReader reader)
        {
            if (!IsServer)
            {
                return;
            }

            if (!CanSimulate)
            {
#if DEBUG && DEEP_DEBUG
                QNetManager.PrintLogMsc("Refusing to simulate request.", this);
#endif
                return;
            }

            if (CanSimulateServerObject)
            {
                // Don't know how somone get here, but no... you can't request simulation of a server object :)
                return;
            }

            // Resolve sample.
            if (!(reader.ReadMessage() is IObjectInputSample sample))
                throw new InvalidOperationException();

            // Resolve simulation result.
            if (!(reader.ReadMessage() is IObjectSimulationResult clientResult))
                throw new InvalidOperationException();

            // Discard old frames (out-of order)
            // This is naive check, can be hacked, but this should only check ordering.
            if (sample.ClientFrame <= _lastClientUpdateFrame)
            {
#if DEBUG && DEEP_DEBUG
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
                IsFirstExecution = true
            };

            // Queue command.
            QueueCommand(command);
        }

        [QNetMessage(Method = QNetMessageMethod.Unreliable)]
        private void ServerCorrectsSimulation(QNetMessageReader reader)
        {
            if (IsServer)
            {
                // Ignore correction on server.
                return;
            }

            // Resolve correction
            var correction = reader.ReadMessage<ObjectSimulationCorrection>();

            // Reconciliate
            // Find first input that is essential for correction (input that failed on the server side).
            var baseInputIndex = _inputHistory.FindIndex(x => x.ClientFrame == correction.ClientFrame);

            // Clear current commands (should be empty, tho).
            _commandQueue.Clear();

            if (baseInputIndex == -1)
            {
                // Construct and queue command.
                QueueCommand(new ObjectSimulationCommand
                {
                    IsFirstExecution = false,
                    ResetState = true,
                    Input = _inputHistory.LastOrDefault(),
                    Result = correction.Result
                });
            }
            else
            {
                //Construct and queue base command
                QueueCommand(new ObjectSimulationCommand
                {
                    IsFirstExecution = false,
                    ResetState = true,
                    Input = _inputHistory[baseInputIndex],
                    Result = correction.Result
                });

                // Write all newer input commands to match the new move
                for (var i = baseInputIndex + 1; i < _inputHistory.Count; i++)
                {
                    var input = _inputHistory[i];

                    // Construct and queue base command
                    QueueCommand(new ObjectSimulationCommand
                    {
                        IsFirstExecution = false,
                        ResetState = false,
                        Input = input
                    });
                }
            }

#if DEBUG
            TotalSimulationCorrections++;
#endif
        }

#if DEBUG
        /// <summary>
        ///     Amount of all simulation corrections generated.
        /// </summary>
        /// <remarks>
        ///     For server this is total generated, but for client total received (includes all objects).
        /// </remarks>
        public static int TotalSimulationCorrections { get; set; } = 0;
#endif
    }
}
