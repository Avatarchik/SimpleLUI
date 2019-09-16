//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Messages.Extensions
{
    /// <summary>
    ///     Extension method to QNet message reader.
    /// </summary>
    public static class QNetMessageReaderExtension
    {
        /// <summary>
        ///     Reads the Vector3.
        /// </summary>
        public static Vector3 ReadVector3(this QNetMessageReader reader) => new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        
        /// <summary>
        ///     Reads the Vector4.
        /// </summary>
        public static Vector4 ReadVector4(this QNetMessageReader reader) => new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        
        /// <summary>
        ///     Reads the Vector2.
        /// </summary>
        public static Vector2 ReadVector2(this QNetMessageReader reader) => new Vector3(reader.ReadSingle(), reader.ReadSingle());

        /// <summary>
        ///     Reads the Quaternion.
        /// </summary>
        public static Quaternion ReadQuaternion(this QNetMessageReader reader) => Quaternion.Euler(ReadVector3(reader));

        /// <summary>
        ///     Reads the Color with alpha always set to 1f.
        /// </summary>
        public static Color ReadColor3(this QNetMessageReader reader) => new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 1f);

        /// <summary>
        ///     Reads the Color with alpha.
        /// </summary>
        public static Color ReadColor4(this QNetMessageReader reader) => new Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }
}