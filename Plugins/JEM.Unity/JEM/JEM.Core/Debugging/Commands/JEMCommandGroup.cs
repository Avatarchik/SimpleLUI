//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.Reflection;

namespace JEM.Core.Debugging.Commands
{
    /// <summary>
    ///     JEM Command Group.
    ///     The command groups stores lists of commands.
    /// </summary>
    public class JEMCommandGroup
    {
        /// <summary>
        ///     Name of command group.
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        ///     Index in collection.
        /// </summary>
        public byte CollectionIndex { get; }

        /// <summary>
        ///     Commands registered in this group.
        /// </summary>
        public List<JEMCommand> Commands { get; } = new List<JEMCommand>();

        /// <summary>
        ///     Creates new commands group.
        /// </summary>
        public JEMCommandGroup(string groupName, byte index)
        {
            GroupName = groupName;
            CollectionIndex = index;
        }

        /// <summary>
        ///     Registers command with specified name and execution method.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <param name="onExecute">Called when command is being executed.</param>
        /// <param name="description">(optional)The command description.</param>
        /// <param name="scope">Defines when this command could be executed.</param>
        public void Register(string commandName, Action onExecute, string description = "No description.", JEMCommandExecScope scope = JEMCommandExecScope.Always) =>
            RegisterCommand(commandName, onExecute.Target, onExecute.Method, description, scope);

        /// <summary>
        ///     Registers command with specified name and execution method.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <param name="onExecute">Called when command is being executed.</param>
        /// <param name="description">(optional)The command description.</param>
        /// <param name="scope">Defines when this command could be executed.</param>
        public void Register<T1>(string commandName, Action<T1> onExecute, string description = "No description.", JEMCommandExecScope scope = JEMCommandExecScope.Always) =>
            RegisterCommand(commandName, onExecute.Target, onExecute.Method, description, scope);

        /// <summary>
        ///     Registers command with specified name and execution method.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <param name="onExecute">Called when command is being executed.</param>
        /// <param name="description">(optional)The command description.</param>
        /// <param name="scope">Defines when this command could be executed.</param>
        public void Register<T1, T2>(string commandName, Action<T1, T2> onExecute, string description = "No description.", JEMCommandExecScope scope = JEMCommandExecScope.Always) =>
            RegisterCommand(commandName, onExecute.Target, onExecute.Method, description, scope);

        /// <summary>
        ///     Registers command with specified name and execution method.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <param name="onExecute">Called when command is being executed.</param>
        /// <param name="description">(optional)The command description.</param>
        /// <param name="scope">Defines when this command could be executed.</param>
        public void Register<T1, T2, T3>(string commandName, Action<T1, T2, T3> onExecute, string description = "No description.", JEMCommandExecScope scope = JEMCommandExecScope.Always) =>
            RegisterCommand(commandName, onExecute.Target, onExecute.Method, description, scope);

        /// <summary>
        ///     Registers command with specified name and execution method.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <param name="onExecute">Called when command is being executed.</param>
        /// <param name="description">(optional)The command description.</param>
        /// <param name="scope">Defines when this command could be executed.</param>
        public void Register<T1, T2, T3, T4>(string commandName, Action<T1, T2, T3, T4> onExecute, string description = "No description.", JEMCommandExecScope scope = JEMCommandExecScope.Always) =>
            RegisterCommand(commandName, onExecute.Target, onExecute.Method, description, scope);

        /// <summary>
        ///     Registers command with specified name and execution method.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <param name="onExecute">Called when command is being executed.</param>
        /// <param name="description">(optional)The command description.</param>
        /// <param name="scope">Defines when this command could be executed.</param>
        public void Register<T1, T2, T3, T4, T5>(string commandName, Action<T1, T2, T3, T4, T5> onExecute, string description = "No description.", JEMCommandExecScope scope = JEMCommandExecScope.Always) =>
            RegisterCommand(commandName, onExecute.Target, onExecute.Method, description, scope);

        /// <summary>
        ///     Registers command with specified name and execution method.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <param name="onExecute">Called when command is being executed.</param>
        /// <param name="description">(optional)The command description.</param>
        /// <param name="scope">Defines when this command could be executed.</param>
        public void Register<T1, T2, T3, T4, T5, T6>(string commandName, Action<T1, T2, T3, T4, T5, T6> onExecute, string description = "No description.", JEMCommandExecScope scope = JEMCommandExecScope.Always) =>
            RegisterCommand(commandName, onExecute.Target, onExecute.Method, description, scope);

        /// <summary>
        ///     Register command with specified name and execution method.
        /// </summary>
        /// <param name="commandName">The command name.</param>
        /// <param name="instance"/>
        /// <param name="method"/>
        /// <param name="description">(optional)The command description.</param>
        /// <param name="scope">Defines when this command could be executed.</param>
        public void RegisterCommand(string commandName, object instance, MethodInfo method, string description = "No description.", JEMCommandExecScope scope = JEMCommandExecScope.Always)
        {
            var parameters = method.GetParameters();

            // check commands, do not allow duplicates!
            foreach (var command in Commands)
                if (command.Name == commandName && command.Parameters.Length == parameters.Length)
                {
                    JEMLogger.LogError(
                        $"Command with one of name `{commandName}` with the same parameters count already exists.",
                        "COMMAND");
                    break;
                }

            // register command
            Commands.Add(new JEMCommand
            {
                Name = commandName,
                CollectionIndex = (byte) Commands.Count,
                Description = description,
                ExecutionScope = scope,
                Group = this,
                Parameters = parameters,
                MethodTarget = instance,
                Method = method
            });
        }
    }
}
