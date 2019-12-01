//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging.Commands;
using JEM.QNet.Messages;
using System;

namespace JEM.QNet.Extras
{
    /// <summary>
    ///     A extension to <see cref="JEMCommandManager"/> that supports <see cref="JEMCommandExecScope"/>.
    /// </summary>
    public static class QNetCommandManager
    {
        /// <summary>
        ///     Executes given command with executor.
        ///     Implements <see cref="JEMCommandExecScope"/> support.
        /// </summary>
        /// <param name="command">The command string, eg.: 'volume master 0.2' or 'volume "master" 0.2'</param>
        /// <returns>Returns a info that represents a result of the operation.</returns>
        public static string ExecuteRaw(string command) => ExecuteRaw((QNetExecutor) null, command);
        
        /// <summary>
        ///     Executes given command with executor.
        ///     Implements <see cref="JEMCommandExecScope"/> support.
        /// </summary>
        /// <param name="serialized">Serialized data of executor of this command.</param>
        /// <param name="command">The command string, eg.: 'volume master 0.2' or 'volume "master" 0.2'</param>
        /// <returns>Returns a info that represents a result of the operation.</returns>
        public static string ExecuteRaw(IQNetSerializedMessage serialized, string command) => ExecuteRaw(new QNetExecutor(default(QNetConnection), serialized), command);

        /// <summary>
        ///     Executes given command with executor.
        ///     Implements <see cref="JEMCommandExecScope"/> support.
        /// </summary>
        /// <param name="executor">A executor of this command.</param>
        /// <param name="command">The command string, eg.: 'volume master 0.2' or 'volume "master" 0.2'</param>
        /// <returns>Returns a info that represents a result of the operation.</returns>
        public static string ExecuteRaw(QNetExecutor executor, string command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            // Parse command.
            var hasExecutor = executor != null;
            var parameters = JEMCommandManager.ParseCommand(command, out var group, out var name);
            if (string.IsNullOrEmpty(group))
            {
                group = JEMCommandManager.DefaultGroup;
            }

            // Resolve command.
            var resolveInfo = JEMCommandManager.ResolveCommandReference(group, name, parameters.Count, hasExecutor, out var commandInstance);
            if (commandInstance.Equals(default(JEMCommand)))
                return resolveInfo;

            // Parse parameters.
            var parseInfo = JEMCommandManager.PrepareParameters(commandInstance.Parameters, parameters.ToArray(), out var parseParams);
            if (!string.IsNullOrEmpty(parseInfo))
            {
                return parseInfo;
            }

            // Process command using exec scope.
            switch (commandInstance.ExecutionScope)
            {
                case JEMCommandExecScope.Always:
                    // Command could be executed always.
                    return JEMCommandManager.ExecuteDirectly(commandInstance, executor, parseParams);
                case JEMCommandExecScope.AnyPeer:
                    if (!QNetGlobal.IsAnyPeerActive)
                        return "This command could only be executed when network is active.";

                    // Command could be executed.
                    return JEMCommandManager.ExecuteDirectly(commandInstance, executor, parseParams);
                case JEMCommandExecScope.NoPeer:
                    if (QNetGlobal.IsAnyPeerActive)
                        return "This command could only be executed when network is NOT active.";

                    // Command could be executed.
                    return JEMCommandManager.ExecuteDirectly(commandInstance, executor, parseParams);
                case JEMCommandExecScope.ServerPeer:
                    if (QNetGlobal.IsServerActive) return JEMCommandManager.ExecuteDirectly(commandInstance, executor, parseParams);
                    if (!QNetGlobal.IsClientActive)
                        return "This command could only be executed on server peer or by client via broadcasting.";
                    if (QNetGlobal.ClientReference == null)
                        return "You are trying to broadcast command exec request while ClientReference is not set.";

                    // Send command request to server.
                    var outgoingMessage = QNetGlobal.ClientReference.GenerateOutgoingMessage(QNetBaseHeader.COMMAND_DATA);
                    outgoingMessage.WriteByte(commandInstance.Group.CollectionIndex);
                    outgoingMessage.WriteByte(commandInstance.CollectionIndex);
                    outgoingMessage.WriteArray(parseParams, obj => { outgoingMessage.WriteObject(obj, true); });

                    var hasSerialized = hasExecutor && (executor.Serialized?.Equals(default(IQNetSerializedMessage)) ?? false);
                    outgoingMessage.WriteBool(hasSerialized);
                    if (hasSerialized)
                    {
                        outgoingMessage.WriteMessage(executor.Serialized, true);
                    }

                    QNetGlobal.ClientReference.Send(QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered, outgoingMessage);

                    // As message is send to the server, all we can is wait for server to respond.
                    // Return empty string as successful cmd exec.
                    return string.Empty;
                case JEMCommandExecScope.ClientPeer:
                    if (!QNetGlobal.IsClientActive)
                        return "This command could only be executed when client peer is active.";

                    // Command could be executed.
                    return JEMCommandManager.ExecuteDirectly(commandInstance, executor, parseParams);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     A <see cref="QNetBaseHeader.COMMAND_DATA"/> handler received from client peer to execute target command.
        /// </summary>
        internal static void OnCommandData(QNetMessage message, QNetMessageReader reader, ref bool disallowRecycle)
        {
            // Read request.
            var groupIndex = reader.ReadByte();
            var cmdIndex = reader.ReadByte();
            var parameters = reader.ReadArray(reader.ReadObject);
            var hasSerialized = reader.ReadBool();
            var serialized = default(IQNetSerializedMessage);
            if (hasSerialized)
                serialized = reader.ReadMessage();

            // Get target.
            var group = JEMCommandManager.Groups[groupIndex];
            var command = group.Commands[cmdIndex];
            var executor = new QNetExecutor(reader.Connection, serialized);

            // Execute command.
            JEMCommandManager.ExecuteDirectly(command, executor, parameters);
        }
    }
}
