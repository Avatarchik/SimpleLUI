//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Messages.Extensions;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Objects
{
    /// <inheritdoc />
    /// <summary>
    ///     QNet serialized object data.
    /// </summary>
    internal class QNetObjectSerialized : QNetSerializedMessage
    {
        /// <summary>
        ///     Object identity.
        /// </summary>
        internal ushort ObjectIdentity;

        /// <summary>
        ///     Owner identity.
        /// </summary>
        internal ushort OwnerIdentity;

        /// <summary>
        ///     Prefab identity.
        /// </summary>
        internal ushort PrefabIdentity;

        /// <summary>
        ///     Position of the serialized object.
        /// </summary>
        internal Vector3 Position;

        /// <summary>
        ///     Rotation of the serialized object.
        /// </summary>
        internal Quaternion Rotation;

        /// <summary>
        ///     Scale of the serialized object.
        /// </summary>
        internal Vector3 Scale;

        /// <summary>
        ///     List of custom components added runtime.
        /// </summary>
        internal byte[] CustomComponents;

        /// <inheritdoc />
        public override void Serialize(QNetMessageWriter writer)
        {
            writer.WriteUInt16(ObjectIdentity);
            writer.WriteUInt16(PrefabIdentity);
            writer.WriteUInt16(OwnerIdentity);

            writer.WriteVector3(Position);
            writer.WriteQuaternion(Rotation);
            writer.WriteVector3(Scale);

            if (CustomComponents == null)
                writer.WriteByte(0);
            else
            {
                writer.WriteByte((byte) CustomComponents.Length);
                for (var index = 0; index < CustomComponents.Length; index++)
                {
                    var c = CustomComponents[index];
                    writer.WriteByte(c);
                }
            }
        }

        /// <inheritdoc />
        public override void DeSerialize(QNetMessageReader reader)
        {
            ObjectIdentity = reader.ReadUInt16();
            PrefabIdentity = reader.ReadUInt16();
            OwnerIdentity = reader.ReadUInt16();

            Position = reader.ReadVector3();
            Rotation = reader.ReadQuaternion();
            Scale = reader.ReadVector3();

            var customComponents = reader.ReadByte();
            var index = 0;
            CustomComponents = new byte[customComponents];
            while (index < customComponents)
            {
                CustomComponents[index] = reader.ReadByte();
                index++;
            }
        }
    }
}