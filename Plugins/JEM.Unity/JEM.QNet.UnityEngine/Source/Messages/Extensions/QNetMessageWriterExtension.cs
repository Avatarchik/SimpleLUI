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
    ///     Extension method to QNet message writer.
    /// </summary>
    public static class QNetMessageWriterExtension
    {
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
        ///     Writes Vector2.
        /// </summary>
        public static void WriteVector2(this QNetMessageWriter writer, Vector2 vector)
        {
            writer.WriteSingle(vector.x);
            writer.WriteSingle(vector.y);
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
    }
}