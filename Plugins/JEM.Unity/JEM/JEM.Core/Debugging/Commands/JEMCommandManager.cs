//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

// source: https://github.com/AlwaysTooLate/AlwaysTooLate.Commands

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JEM.Core.Debugging.Commands
{
    /// <summary>
    ///     Allows to register commands.
    /// </summary>
    public static class JEMCommandManager
    {
        /// <summary>
        ///     Creates and registers commands group of given name.
        /// </summary>
        public static JEMCommandGroup RegisterGroup(string groupName)
        {
            if (GetGroup(groupName) != null)
            {
                throw new NotSupportedException($"Command group {groupName} already exists.");
            }

            var group = new JEMCommandGroup(groupName, (byte)Groups.Count);
            Groups.Add(group);
            return group;
        }

        /// <summary>
        ///     Resolves the <see cref="JEMCommand"/> instance to given info.
        /// </summary>
        /// <returns>Returns a info that represents a result of operation.</returns>
        public static string ResolveCommandReference(string commandGroup, string commandName, int parametersCount, bool hasSender, out JEMCommand command)
        {
            if (commandName == null) throw new ArgumentNullException(nameof(commandName));
            command = default(JEMCommand);

            // Get all commands for target group.
            JEMCommand[] allCommands;
            if (string.IsNullOrEmpty(commandGroup))
                allCommands = GetAllCommands();
            else allCommands = (GetGroup(commandGroup)?.Commands ?? new List<JEMCommand>()).ToArray();

            // Resolve command of name.
            var commands = allCommands.Where(x => x.Name == commandName).OrderBy(x => x.Parameters.Length).ToArray();

            var cmdName = string.IsNullOrEmpty(commandGroup) ? commandName : $"{commandGroup}.{commandName}";
            if (commands.Length == 0)
            {
                return $"\'{cmdName}\' command not found.";
            }

            var found = false;
            command = new JEMCommand();
            for (var index = commands.Length - 1; index >= 0; index--)
            {
                var cmd = commands[index];
                if (cmd.Parameters.Length != parametersCount)
                {
                    // To allow not always defining a sender object in target command
                    if (cmd.Parameters.Length != parametersCount + 1)
                        continue;
                }

                found = true;
                command = cmd;
                break;
            }

            return !found ? $"\'{cmdName}\' command exists, but invalid parameters were given. {parametersCount}" : string.Empty;
        }

        /// <summary>
        ///     Executes command with given parameters.
        /// </summary>
        /// <param name="commandGroup">The command group.</param>
        /// <param name="commandName">The command name.</param>
        /// <param name="parameters">The command parameters, typeless, checking and parsing will be done.</param>
        public static string Execute(string commandGroup, string commandName, params string[] parameters) =>
            Execute(commandGroup, commandName, null, parameters);

        /// <summary>
        ///     Executes command with given parameters.
        /// </summary>
        /// <param name="commandGroup">The command group.</param>
        /// <param name="commandName">The command name.</param>
        /// <param name="sender">A optional sender.</param>
        /// <param name="parameters">The command parameters, typeless, checking and parsing will be done.</param>
        public static string Execute(string commandGroup, string commandName, object sender, params string[] parameters)
        {
            // Resolve command.
            var hasSender = sender != null;
            var resolveInfo = ResolveCommandReference(commandGroup, commandName, parameters.Length, hasSender, out var command);
            if (command.Equals(default(JEMCommand)))
                return resolveInfo;

            // Parse parameters.
            var parseInfo = PrepareParameters(command.Parameters, parameters, out var parseParams);
            if (!string.IsNullOrEmpty(parseInfo))
            {
                return parseInfo;
            }

            // Execute command.
            return ExecuteDirectly(command, sender, parseParams);
        }

        /// <summary>
        ///     Executes target command with given sender and parameters.
        /// </summary>
        public static string ExecuteDirectly(JEMCommand command, object sender, object[] parameters)
        {
            try
            {
                if (sender != null && command.Parameters.Length == parameters.Length + 1)
                {
                    var fixedParameters = new List<object> { sender };
                    fixedParameters.AddRange(parameters);
                    command.Method.Invoke(command.MethodTarget, fixedParameters.ToArray());
                }
                else
                {
#if DEBUG
                    // A parameters test.
                    if (command.Parameters.Length > parameters.Length)
                    {
                        if (parameters.Length == 0)
                            return "given parameters array is empty";

                        for (int index = 1; index < parameters.Length; index++)
                        {
                            if (command.Parameters[index].ParameterType != parameters[index].GetType())
                                return $"parameter of index {index} has invalid type";
                        }

                        return "first parameter has invalid type for received data " + $"({command.Parameters[0].ParameterType.Name} vs {parameters[0].GetType().Name})!" +
                               " is sender parameter defined in target command?";
                    }

                    if (command.Parameters.Length < parameters.Length) return $"no enough parameters were given ({command.Parameters.Length} vs {parameters.Length})";
#endif
                    command.Method.Invoke(command.MethodTarget, parameters);
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException t)
                    JEMLogger.LogException(t.InnerException);
                else JEMLogger.LogException(e);
                return "an unexcepted error while executing command";
            }
        }

        /// <summary>
        ///     Parse string in to object parameters with it's actual type.
        /// </summary>
        public static string PrepareParameters(ParameterInfo[] info, string[] parameters, out object[] parseParams)
        {
            // parse
            parseParams = new object[info.Length];
            if (parseParams.Length == 0)
            {
                // Nothing to parse.
                return string.Empty;
            }

            var parseIndex = 0;
            var parse = new List<object>();
            for (var index = 0; index < info.Length; index++)
            {
                if (index >= info.Length)
                    throw new InvalidOperationException($"index is higher than parameters info array ({index} vs {info.Length})");
                if (parseIndex >= parameters.Length)
                    break; //throw new InvalidOperationException($"parse index is higher than parameters string array ({parseIndex} vs {parameters.Length} vs {info.Length})");

                var parameterInfo = info[index];
                var parameterStr = parameters[parseIndex];

                switch (parameterInfo.ParameterType.Name.ToLower())
                {
                    case "string":
                        // string does not need any type check
                        parse.Add(parameterStr);
                        break;
                    case "int32":
                        if (!int.TryParse(parameterStr, out var resultInt))
                            return $"invalid parameter type were given for \'{parameterStr}\'";

                        parse.Add(resultInt);
                        break;
                    case "single":
                        if (!float.TryParse(parameterStr, out var resultSingle))
                            return $"invalid parameter type were given for \'{parameterStr}\'";

                        parse.Add(resultSingle);
                        break;
                    case "double":
                        if (!double.TryParse(parameterStr, out var resultDouble))
                            return $"invalid parameter type were given for \'{parameterStr}\'";

                        parse.Add(resultDouble);
                        break;
                    case "boolean":
                        if (!bool.TryParse(parameterStr, out var resultBoolean))
                            return $"invalid parameter type were given for \'{parameterStr}\'";

                        parse.Add(resultBoolean);
                        break;
                    default:
                        parseIndex--;
                        break;
                        //default:
                        //    return "command target method has invalid type in parameters!";
                }

                parseIndex++;
            }

            parseParams = parse.ToArray();
            return string.Empty;
        }

        /// <summary>
        ///     Parse command to get name and parameters.
        ///     Strings are supported! eg.: 'test "Hello, World!"'
        /// </summary>
        public static List<string> ParseCommand(string command, out string group, out string name)
        {
            // trim
            command = command.Trim();

            group = string.Empty;
            name = string.Empty;

            var parameters = new List<string>();
            var parameter = "";
            var stringRead = false;

            foreach (var ch in command)
                if (ch == ' ' && !stringRead)
                {
                    // next param

                    if (!string.IsNullOrEmpty(parameter))
                        parameters.Add(parameter);

                    parameter = string.Empty;
                }
                else if (ch == '"')
                {
                    // start or stop string param   
                    if (stringRead)
                    {
                        stringRead = false;
                        continue;
                    }

                    stringRead = true;
                }
                else
                {
                    // add to current.
                    parameter += ch;
                }

            // add last parameter
            if (!string.IsNullOrEmpty(parameter)) parameters.Add(parameter);

            // set name
            if (parameters.Count > 0)
            {
                var s = parameters[0].Split('.');
                if (s.Length <= 1)
                    name = parameters[0];
                else
                {
                    group = s[0];
                    name = s[1];
                }

                parameters.RemoveAt(0);
            }

            return parameters;
        }

        /// <summary>
        ///     Executes raw command, parse arguments etc.
        /// </summary>
        /// <param name="command">The command string, eg.: 'volume master 0.2' or 'volume "master" 0.2'</param>
        public static string ExecuteRaw(string command)
        {
            // parse
            var parameters = ParseCommand(command, out var group, out var name);
            if (string.IsNullOrEmpty(group))
            {
                group = DefaultGroup;
            }

            // execute
            return Execute(group, name, parameters.ToArray());
        }

        /// <summary>
        ///     Removes all registered commands.
        /// </summary>
        public static void ClearAll() =>
            Groups.Clear();

        /// <summary>
        ///     Lists all available commands.
        /// </summary>
        /// <returns>The commands array containing all commands.</returns>
        public static JEMCommand[] GetAllCommands()
        {
            var commands = new List<JEMCommand>();
            foreach (var g in Groups)
            {
                commands.AddRange(g.Commands);
            }

            return commands.ToArray();
        }

        /// <summary>
        ///     Gets group of given name.
        /// </summary>
        public static JEMCommandGroup GetGroup(string groupName)
        {
            foreach (var g in Groups)
            {
                if (g.GroupName == groupName)
                    return g;
            }

            return null;
        }

        /// <summary>
        ///     Default group name. If set, will be used if no group been given while executing the command.
        /// </summary>
        public static string DefaultGroup { get; set; } = string.Empty;

        /// <summary>
        ///     All registered groups.
        /// </summary>
        public static List<JEMCommandGroup> Groups { get; } = new List<JEMCommandGroup>();
    }
}