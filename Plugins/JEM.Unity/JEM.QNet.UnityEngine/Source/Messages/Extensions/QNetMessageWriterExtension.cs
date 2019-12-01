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
    ///     Extension method to QNet message writer.
    /// </summary>
    public static class QNetMessageWriterExtension
    {
        /// <summary>
        ///     Writes Vector2.
        /// </summary>
        public static void WriteVector2(this QNetMessageWriter writer, Vector2 vector)
        {
            writer.WriteSingle(vector.x);
            writer.WriteSingle(vector.y);
        }

        /// <summary>
        ///     Writes Vector3.
        /// </summary>
        public static void WriteVector3(this QNetMessageWriter writer, Vector3 vector)
        {
            writer.WriteSingle(vector.x);
            writer.WriteSingle(vector.y);
            writer.WriteSingle(vector.z);
        }

        /// <summary>
        ///     Writes Vector4.
        /// </summary>
        public static void WriteVector4(this QNetMessageWriter writer, Vector4 vector)
        {
            writer.WriteSingle(vector.x);
            writer.WriteSingle(vector.y);
            writer.WriteSingle(vector.z);
            writer.WriteSingle(vector.w);
        }

        /// <summary>
        ///     Writes Vector2Int.
        /// </summary>
        public static void WriteVector2Int(this QNetMessageWriter writer, Vector2Int vector)
        {
            writer.WriteInt32(vector.x);
            writer.WriteInt32(vector.y);
        }

        /// <summary>
        ///     Writes Vector3Int.
        /// </summary>
        public static void WriteVector3Int(this QNetMessageWriter writer, Vector3Int vector)
        {
            writer.WriteInt32(vector.x);
            writer.WriteInt32(vector.y);
            writer.WriteInt32(vector.z);
        }

        /// <summary>
        ///     Writes the Quaternion.
        /// </summary>
        public static void WriteQuaternion(this QNetMessageWriter writer, Quaternion quaternion) => WriteVector3(writer, quaternion.eulerAngles);

        /// <summary>
        ///     Writes the Color with alpha set always to 1f.
        /// </summary>
        public static void WriteColor3(this QNetMessageWriter writer, Color color)
        {
            writer.WriteSingle(color.r);
            writer.WriteSingle(color.g);
            writer.WriteSingle(color.b);
        }

        /// <summary>
        ///     Writes the Color with alpha set always to 1f.
        /// </summary>
        public static void WriteColor4(this QNetMessageWriter writer, Color color)
        {
            writer.WriteSingle(color.r);
            writer.WriteSingle(color.g);
            writer.WriteSingle(color.b);
            writer.WriteSingle(color.r);
        }

        /// <summary>
        ///     Writes the array of <see cref="object"/>s. NOTE: This is just a for loop using <see cref="WriteUnityObject"/>.
        /// </summary>
        public static void WriteUnityObjects(this QNetMessageWriter writer, [NotNull] object[] objs) => WriteUnityObjects(writer, objs, false);
        
        /// <summary>
        ///     Writes the array of <see cref="object"/>s. NOTE: This is just a for loop using <see cref="WriteUnityObject"/>.
        /// </summary>
        public static void WriteUnityObjects(this QNetMessageWriter writer, [NotNull] object[] objs, bool includeTypeIndex)
        {
            if (objs == null) throw new ArgumentNullException(nameof(objs));
            for (var index = 0; index < objs.Length; index++)
            {
                var obj = objs[index];
                WriteUnityObject(writer, obj, includeTypeIndex);
            }
        }

        /// <summary>
        ///     Writes the <see cref="object"/> to the <see cref="QNetMessageWriter"/> including the unity's types like <see cref="Vector3"/>.
        /// </summary>
        public static void WriteUnityObject(this QNetMessageWriter writer, [NotNull] object obj) => WriteUnityObject(writer, obj, false);
        
        /// <summary>
        ///     Writes the <see cref="object"/> to the <see cref="QNetMessageWriter"/> including the unity's types like <see cref="Vector3"/>.
        /// </summary>
        public static void WriteUnityObject(this QNetMessageWriter writer, [NotNull] object obj, bool includeTypeIndex)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            if (includeTypeIndex)
            {
                var unityValue = GetUnityObjectType(obj);
                var isUnityValue = unityValue != QNetMessageUnityValue.Unknown;
                if (isUnityValue)
                    writer.WriteEnum(unityValue);
            }

            // Check if arg is a unity type.
            switch (obj)
            {
                case Vector3 v3:
                    writer.WriteVector3(v3);
                    break;
                case Vector2 v2:
                    writer.WriteVector2(v2);
                    break;
                case Vector4 v4:
                    writer.WriteVector3(v4);
                    break;
                case Color c:
                    writer.WriteColor4(c);
                    break;
                default:
                    // This is not a unity type.
                    // Continue with WriteObject.
                    writer.WriteObject(obj, includeTypeIndex);
                    break;
            }
        }

        public static QNetMessageUnityValue GetUnityObjectType([NotNull] object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            switch (obj)
            {
                case Vector2 _:
                    return QNetMessageUnityValue.Vector2;
                case Vector3 _:
                    return QNetMessageUnityValue.Vector3;
                case Vector4 _:
                    return QNetMessageUnityValue.Vector4;
                case Quaternion _:
                    return QNetMessageUnityValue.Quaternion;
                case Color _:
                    return QNetMessageUnityValue.Color4;
                default:
                    return QNetMessageUnityValue.Unknown;
            }
        }
    }
}