//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;

namespace JEM.Core.Debugging.CVar
{
    /// <inheritdoc />
    /// <summary>
    ///     Thanks to CVarAttribute you can define what static properties should be loaded as console var.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JEMCVarAttribute : Attribute
    {
        /// <summary>
        ///     Name of CVar.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Description of the CVar.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Defines whether the CVar is synchronized over network. If so, the CVar can only be changed by the server.
        /// </summary>
        public bool IsNetworkVar { get; set; } = false;

        /// <inheritdoc />
        public JEMCVarAttribute(string name) : this(name, "No Description") { }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor.
        /// </summary>
        public JEMCVarAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
