//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using Lidgren.Network;
using System;
using System.Collections.Generic;
using JEM.Core.Debugging;

namespace JEM.QNet.Messages
{
    /// <summary>
    ///     QNet message writer.
    /// </summary>
    public sealed class QNetMessageWriter
    {
        private NetOutgoingMessage Message { get; }

        /// <summary/>
        internal QNetMessageWriter(NetOutgoingMessage message) => Message = message;

        /// <summary>
        ///     Writes a <see cref="int"/>.
        /// </summary>
        public void WriteInt32(int value) => Message.Write(value);
        
        /// <summary>
        ///     Writes a unsigned <see cref="int"/>.
        /// </summary>
        public void WriteUInt32(uint value) => Message.Write(value);    

        /// <summary>
        ///     Writes a <see cref="long"/>.
        /// </summary>
        public void WriteInt64(long value) => Message.Write(value);
        
        /// <summary>
        ///     Writes a unsigned <see cref="long"/>.
        /// </summary>
        public void WriteUInt64(ulong value) => Message.Write(value);     

        /// <summary>
        ///     Writes a <see cref="short"/>.
        /// </summary>
        public void WriteInt16(short value) => Message.Write(value);
       
        /// <summary>
        ///     Writes a unsigned <see cref="short"/>.
        /// </summary>
        public void WriteUInt16(ushort value) => Message.Write(value);
        
        /// <summary>
        ///     Writes a <see cref="bool"/>.
        /// </summary>
        public void WriteBool(bool value) => Message.Write(value);
        
        /// <summary>
        ///     Writes a <see cref="float"/>.
        /// </summary>
        public void WriteSingle(float value) => Message.Write(value);
        
        /// <summary>
        ///     Writes a <see cref="double"/>.
        /// </summary>
        public void WriteDouble(double value) => Message.Write(value);  

        /// <summary>
        ///     Writes a <see cref="string"/>.
        /// </summary>
        public void WriteString(string str) => Message.Write(str);

        /// <summary>
        ///     Writes a message of <typeparamref name="TSerializedMessage"/> type.
        /// </summary>
        /// <param name="message">QNetSerializedMessage based class instance.</param>
        public void WriteMessage<TSerializedMessage>(TSerializedMessage message) where TSerializedMessage : IQNetSerializedMessage => WriteMessage(message, false);
        
        /// <summary>
        ///     Writes a message of <typeparamref name="TSerializedMessage"/> type.
        /// </summary>
        /// <param name="message">QNetSerializedMessage based class instance.</param>
        /// <param name="includeTypeIndex">
        ///     When set to true, typeIndex will be included while serializing the message.
        ///     This allows to deserialize message on remote peer without knowing the type.
        ///     Should never be used when type is known when reading!
        /// </param>
        public void WriteMessage<TSerializedMessage>(TSerializedMessage message, bool includeTypeIndex) where TSerializedMessage : IQNetSerializedMessage
        {
            var isDefault = message?.Equals(default(TSerializedMessage)) ?? true;
            WriteBool(isDefault);

            if (includeTypeIndex)
            {
                // When message is default, try to serialize type index using TSerializedMessage.
                var messageType = isDefault ? typeof(TSerializedMessage) : message.GetType();
                if (QNetPeer.GetSerializedMessageIndex(messageType, out var typeIndex))
                {
                    WriteByte(typeIndex);
                }
                else throw new NullReferenceException($"Failed to find typeIndex of serialized message of type {messageType.FullName}.");
            }

            if (!isDefault)
            {
                message.Serialize(this);
            }
        }

        /// <summary>
        ///     Writes a <see cref="byte"/>.
        /// </summary>
        public void WriteByte(byte _byte) => Message.Write(_byte);

        /// <summary>
        ///     Writes a singed <see cref="byte"/>.
        /// </summary>
        public void WriteSByte(sbyte _byte) => Message.Write(_byte);

        /// <summary>
        ///     Writes a <see cref="byte"/> array.
        /// </summary>
        public void WriteBytes(byte[] bytes)
        {
            WriteUInt16((ushort) bytes.Length);
            Message.Write(bytes);
            Message.WritePadBits();
        }

        /// <summary>
        ///     Writes a <see cref="Enum"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NotSupportedException"/>
        public void WriteEnum(Enum e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            var c = Convert.ChangeType(e, e.GetTypeCode());
            switch (e.GetTypeCode())
            {
                case TypeCode.Byte:
                    WriteByte((byte) c);
                    break;
                case TypeCode.Int16:
                    WriteInt16((short) c);
                    break;
                case TypeCode.UInt16:
                    WriteUInt16((ushort) c);
                    break;
                case TypeCode.Int32:
                    WriteInt32((int) c);
                    break;
                case TypeCode.UInt32:
                    WriteUInt32((uint) c);
                    break;
                case TypeCode.Int64:
                    WriteInt64((long) c);
                    break;
                case TypeCode.UInt64:
                    WriteUInt64((ulong) c);
                    break;
                default:
                    throw new NotSupportedException($"TypeCode {e.GetTypeCode()} is not supported.");
            }
        }

        /// <summary>
        ///     Writes a <see cref="byte"/> array directly (without a size of array).
        /// </summary>
        public void WriteBytesRaw(byte[] bytes) => Message.Write(bytes);

        /// <summary>
        ///     Writes a array of <typeparamref name="TType"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void WriteArray<TType>(TType[] array, Action<TType> onSerializeItem)
        {
            if (onSerializeItem == null) throw new ArgumentNullException(nameof(onSerializeItem));
            if (array == null)
                WriteByte(0);
            else
            {
                WriteByte(1);
                WriteUInt16((ushort) array.Length);
                for (int index = 0; index < array.Length; index++)
                {
                    onSerializeItem.Invoke(array[index]);
                }
            }
        }

        /// <summary>
        ///     Writes a list of <typeparamref name="TType"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void WriteList<TType>(IList<TType> list, Action<TType> onSerializeItem)
        {
            if (onSerializeItem == null) throw new ArgumentNullException(nameof(onSerializeItem));
             if (list == null)
                 WriteByte(0);
             else
             {
                 WriteByte(1);
                 WriteUInt16((ushort) list.Count);
                 for (int index = 0; index < list.Count; index++)
                 {
                     onSerializeItem.Invoke(list[index]);
                 }
             }
        }

        /// <summary>
        ///     Writes a dictionary.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void WriteDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
            Action<TKey, TValue> onSerializeItem)
        {
            if (onSerializeItem == null) throw new ArgumentNullException(nameof(onSerializeItem));
            if (dictionary == null)
                WriteByte(0);
            else
            {
                WriteByte(1);
                WriteUInt16((ushort) dictionary.Count);
                foreach (var d in dictionary)
                {
                    onSerializeItem.Invoke(d.Key, d.Value);
                }
            }
        }

        /// <summary>
        ///     Writes a <see cref="object"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public void WriteObject(object obj) => WriteObject(obj, false);
        
        /// <summary>
        ///     Writes a <see cref="object"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        public void WriteObject(object obj, bool includeTypeIndex) => QNetPeer.WriteObjectToMessage(this, obj, includeTypeIndex);

        /// <summary/>
        public void WritePadBites() => Message.WritePadBits();

        /// <summary>
        ///     Gets outgoing message of this writer.
        /// </summary>
        public NetOutgoingMessage GetMessage() => Message;     
    }
}