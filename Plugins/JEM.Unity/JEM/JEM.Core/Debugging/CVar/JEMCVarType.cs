//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.Core.Debugging.CVar
{
    /// <summary>
    ///     Defines CVar data type.
    /// </summary>
    public enum JEMCVarType : byte
    {
        /// <summary>
        ///     The type of data is unknown, and target will most likely throw an error.
        /// </summary>
        Unknown,

        /// <summary>
        ///     String data type.
        /// </summary>
        String,

        /// <summary>
        ///     Int32 data type.
        /// </summary>
        Int32,

        /// <summary>
        ///     uInt32 data type.
        /// </summary>
        UInt32,

        /// <summary>
        ///     Int64 data type.
        /// </summary>
        Int64,

        /// <summary>
        ///     Single data type.
        /// </summary>
        Single,

        /// <summary>
        ///     Double data type.
        /// </summary>
        Double,

        /// <summary>
        ///     Boolean data type.
        /// </summary>
        Boolean
    }
}
