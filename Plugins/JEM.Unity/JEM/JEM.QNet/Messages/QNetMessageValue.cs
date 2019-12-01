//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet.Messages
{
#pragma warning disable 1591
    /// <summary>
    ///     Defines a type of the value <see cref="QNetMessageReader"/> and <see cref="QNetMessageWriter"/> can use.
    /// </summary>
    public enum QNetMessageValue : byte
    {
        Int32,
        UInt32,
        Int16,
        UInt16,
        Int64,
        UInt64,
        Single,
        Double,
        Byte,
        SByte,
        String,
        Boolean,
        ByteArray,
        SerializedMessage,
        Enum
    }
}
