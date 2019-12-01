//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;

namespace JEM.Core.Configuration
{
    /// <inheritdoc />
    /// <summary>
    ///     JEM Smart Configuration Var Attribute.
    ///     It defines a configuration vars.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JEMSmartConfigVarAttribute : Attribute
    {
        /// <summary>
        ///     Name of configuration var.
        /// </summary>
        public string VarName { get; }

        /// <summary>
        ///     Name of change method to call.
        ///     The method need to be in the same Class of property.
        /// </summary>
        public string OnValueChanged { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="varName">Name of configuration var.</param>
        /// <param name="onValueChanged">Name of change method to call. The method need to be in the same Class of property.</param>
        public JEMSmartConfigVarAttribute(string varName, string onValueChanged = null)
        {
            VarName = varName;
            OnValueChanged = onValueChanged;
        }
    }
}
