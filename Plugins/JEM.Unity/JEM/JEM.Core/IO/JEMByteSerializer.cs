//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

//
// NOTE: This class is Obsolete and may be removed in the future.
//

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JEM.Core.IO
{
    /// <summary>
    ///     JEM Byte Serializer.
    /// </summary>
    [Obsolete("This class is Obsolete and may be removed in the future.")]
    public static class JEMByteSerializer
    {
        /// <summary>
        ///     Serialize given object to byte array.
        /// </summary>
        public static byte[] SerializeToBytes(this object source)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                return stream.ToArray();
            }
        }

        /// <summary>
        ///     Deserialize bytes to object of given type.
        /// </summary>
        public static T DeserializeFromBytes<T>(this byte[] source) => (T) DeserializeFromBytes(source);

        /// <summary>
        ///     Deserialize bytes to object of given type.
        /// </summary>
        public static object DeserializeFromBytes(this byte[] source)
        {
            using (var stream = new MemoryStream(source))
            {
                var formatter = new BinaryFormatter();
                stream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(stream);
            }
        }
    }
}