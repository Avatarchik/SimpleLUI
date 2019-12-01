//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Reflection;

namespace JEM.Core.Debugging.CVar
{
    /// <summary>
    ///     CVar creation settings.
    /// </summary>
    public struct JEMCVarSettings<T>
    {
        /// <summary>
        ///     Name of CVar.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Default value of CVar.
        /// </summary>
        public object Default { get; set; }

        /// <summary>
        ///     Description of CVar.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Is CVar networked?
        /// </summary>
        public bool IsNetworkVar { get; set; }

        /// <summary>
        ///     OnValueChanged event.
        /// </summary>
        public Action<T> OnValueChanged { get; set; }

        /// <summary>
        ///     If set to true, all OnValueChanged will be invoked at end of CVar registration.
        /// </summary>
        public bool InvokeAtStart { get; set; }

        /// <summary>
        ///     Property from this CVar has been generated.
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <inheritdoc />
        public JEMCVarSettings(string name, object @default) : this(name, @default, null)
        {
            Description = "No description.";
        }

        /// <inheritdoc />
        public JEMCVarSettings(string name, object @default, string description) : this(name, @default, description, false)
        {
            IsNetworkVar = false;
        }

        /// <inheritdoc />
        public JEMCVarSettings(string name, object @default, string description, bool isNetworkVar) : this(name, @default, description, isNetworkVar, null)
        {
            OnValueChanged = null;
        }

        /// <summary>
        ///     CVar Settings ctr.
        /// </summary>
        public JEMCVarSettings(string name, object @default, string description, bool isNetworkVar,
            Action<T> onValueChanged)
        {
            Name = name;
            Default = @default;
            Description = description;
            IsNetworkVar = isNetworkVar;
            OnValueChanged = onValueChanged;
            InvokeAtStart = true;
            Property = null;
        }
    }
}
