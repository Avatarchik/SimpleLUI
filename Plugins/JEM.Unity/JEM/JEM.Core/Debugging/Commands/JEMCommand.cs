//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Reflection;

namespace JEM.Core.Debugging.Commands
{
    /// <summary>
    ///     Used by JEM Commands system to tell when command could be executed.
    /// </summary>
    public enum JEMCommandExecScope
    {
        /// <summary>
        ///     Command can be executed no mather network state.
        ///     Used by default.
        /// </summary>
        Always,

        /// <summary>
        ///     Command can be executed only when any (client or server) peer is active.
        /// </summary>
        AnyPeer,

        /// <summary>
        ///     Command can only be executed when no peer is active.
        /// </summary>
        NoPeer,

        /// <summary>
        ///     Command can be executed only when server peer is active.
        ///     NOTE: Client peers will set data to server to try to execute this type of command on the server side.
        /// </summary>
        ServerPeer,

        /// <summary>
        ///     Command can be executed only when client peer is active.
        /// </summary>
        ClientPeer
    }

    /// <summary>
    ///     Structure of the command data.
    /// </summary>
    public struct JEMCommand
    {
        /// <summary>
        ///     The command name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Index in collection
        /// </summary>
        public byte CollectionIndex { get; set; }

        /// <summary>
        ///     The command description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The execution scope of the command.
        /// </summary>
        /// <remarks>
        ///     Execution scope is not utilized in JEM.Core library.
        ///     To take advantage of this feature you need execute commands via network
        ///       command manager implemented in to your networking framework.
        /// </remarks>
        public JEMCommandExecScope ExecutionScope { get; set; }

        /// <summary>
        ///     The command group, easily unregister batch of commands by one call.
        ///     Use this of eg.: level dependent commands, only 'when playing' command etc.
        /// </summary>
        public JEMCommandGroup Group { get; set; }

        /// <summary>
        ///     Array of parameters of the command.
        /// </summary>
        public ParameterInfo[] Parameters { get; set; }

        /// <summary>
        ///     Target instance of the method of the command.
        /// </summary>
        public object MethodTarget { get; set; }

        /// <summary>
        ///     Method info of the command.
        /// </summary>
        public MethodInfo Method { get; set; }
    }
}
