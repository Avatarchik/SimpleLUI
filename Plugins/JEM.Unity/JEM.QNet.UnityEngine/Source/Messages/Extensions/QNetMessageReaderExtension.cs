//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JetBrains.Annotations;
using System;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Messages.Extensions
{
    /// <summary>
    ///     Extension method to QNet message reader.
    /// </summary>
    public static class QNetMessageReaderExtension
    {
        /// <summary>
        ///     Reads the Vector2.
        /// </summary>
        public static Vector2 ReadVector2(this QNetMessageReader reader) => new Vector2(reader.ReadSingle(), reader.ReadSingle());

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
        public static Vector2Int ReadVector2Int(this QNetMessageReader reader) => new Vector2Int(reader.ReadInt32(), reader.ReadInt32());

        /// <summary>
        ///     Reads the Vector3.
        /// </summary>
        public static Vector3Int ReadVector3Int(this QNetMessageReader reader) => new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

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

        /// <summary>
        ///     Reads a <see cref="object"/> including unity's types like <see cref="Vector3"/>.
        ///      Reading a object is only possible when value has been written whit valueTypeIndex.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static object ReadUnityObject([NotNull] this QNetMessageReader reader) => ReadUnityObject(reader, new object[] { });

        /// <summary>
        ///     Reads a <see cref="object"/> including unity's types like <see cref="Vector3"/>.
        ///      Reading a object is only possible when value has been written whit valueTypeIndex.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static object ReadUnityObject([NotNull] this QNetMessageReader reader, params object[] additionalReaderParams)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            var enumByte = reader.ReadByte();
            if ((byte) QNetMessageValue.Enum >= enumByte)
            {
                return QNetPeer.ReadObjectFromMessage(reader, (QNetMessageValue) enumByte, additionalReaderParams);
            }

            var unityValue = (QNetMessageUnityValue) enumByte;
            switch (unityValue)
            {
                case QNetMessageUnityValue.Vector2:
                    return reader.ReadVector2();
                case QNetMessageUnityValue.Vector3:
                    return reader.ReadVector3();
                case QNetMessageUnityValue.Vector4:
                    return reader.ReadVector4();
                case QNetMessageUnityValue.Quaternion:
                    return reader.ReadQuaternion();
                case QNetMessageUnityValue.Color4:
                    return reader.ReadColor4();
                default:
                    throw new ArgumentOutOfRangeException(nameof(unityValue), unityValue, null);
            }
        }
    }
}