//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Diagnostics.CodeAnalysis;

namespace JEM.Core.Configuration
{
    /// <summary>
    ///     JEM Configuration save method.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum JEMConfigurationSaveMethod : byte
    {
        /// <summary>
        ///     Unknown formatting type.
        /// </summary>
        UNKNOWN,

        /// <summary>
        ///     Save/Load using Newtonsoft.Json.
        /// </summary>
        JSON
    }
}