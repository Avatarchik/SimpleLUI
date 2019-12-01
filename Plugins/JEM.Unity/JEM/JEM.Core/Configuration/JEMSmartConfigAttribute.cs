//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;

namespace JEM.Core.Configuration
{
    /// <summary>
    ///     JEM Smart Configuration Property Mode.
    ///     It defines how Properties of target Type should be gathered.
    /// </summary>
    public enum JEMSmartConfigPropertyMode
    {
        /// <summary>
        ///     A default gather type. Only Properties with JEMSmartConfigVarAttribute will be collected.
        /// </summary>
        Default,

        /// <summary>
        ///     All Properties in target type will be collected. If target property does not have JEMSmartConfigVarAttribute, the name of property will be used as varName instead.
        /// </summary>
        ForcedAll
    }

    /// <inheritdoc />
    /// <summary>
    ///     JEM Smart Configuration Attribute.
    ///     Defines a class that holds configuration vars.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class JEMSmartConfigAttribute : Attribute
    {
        /// <summary>
        ///     Configuration (file)name.
        /// </summary>
        public string ConfigName { get; }

        /// <summary>
        ///     Property mode of this Configuration.
        /// </summary>
        public JEMSmartConfigPropertyMode PropertyMode { get; set; } = JEMSmartConfigPropertyMode.Default;

        /// <inheritdoc />
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="configName">Configuration (file)name.</param>
        public JEMSmartConfigAttribute(string configName)
        {
            ConfigName = configName;
        }
    }
}
