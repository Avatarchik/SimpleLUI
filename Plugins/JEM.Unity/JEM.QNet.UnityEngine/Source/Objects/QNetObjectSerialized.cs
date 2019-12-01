//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Collections.Generic;
using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Messages.Extensions;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Objects
{
    /// <inheritdoc cref="IQNetSerializedMessage" />
    /// <summary>
    ///     QNet serialized object data.
    /// </summary>
    internal struct QNetObjectSerialized : IQNetSerializedMessage
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
        ///     Active state of this object.
        /// </summary>
        internal bool IsNetworkActive;

        /// <summary>
        ///     List of custom components added runtime.
        /// </summary>
        internal IDictionary<byte, byte> CustomObjects;

        /// <inheritdoc />
        public void Serialize(QNetMessageWriter writer)
        {
            writer.WriteUInt16(ObjectIdentity);
            writer.WriteUInt16(PrefabIdentity);
            writer.WriteUInt16(OwnerIdentity);

            writer.WriteVector3(Position);
            writer.WriteQuaternion(Rotation);
            writer.WriteVector3(Scale);

            writer.WriteBool(IsNetworkActive);

            writer.WriteDictionary(CustomObjects, (key, value) => { writer.WriteByte(key); writer.WriteByte(value); });
            // writer.WriteArray(CustomComponents, writer.WriteByte);
        }

        /// <inheritdoc />
        public void DeSerialize(QNetMessageReader reader)
        {
            ObjectIdentity = reader.ReadUInt16();
            PrefabIdentity = reader.ReadUInt16();
            OwnerIdentity = reader.ReadUInt16();

            Position = reader.ReadVector3();
            Rotation = reader.ReadQuaternion();
            Scale = reader.ReadVector3();

            IsNetworkActive = reader.ReadBool();

            CustomObjects = reader.ReadDictionary((out byte key, out byte value) => { key = reader.ReadByte();
                value = reader.ReadByte();
            });
            // CustomComponents = reader.ReadArray(reader.ReadByte);
        }
    }
}