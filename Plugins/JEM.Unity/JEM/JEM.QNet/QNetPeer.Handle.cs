//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.QNet.Messages;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JEM.QNet
{
    // Methods related to peer messages generation etc.
    public abstract partial class QNetPeer
    {
        /// <summary>
        ///     QNetPeer before message event.
        ///     Called at the beginning of <see cref="GenerateOutgoingMessage(byte, object[])"/>.
        /// </summary>
        public event QNetPeerBeforeMessage OnBeforeMessage;

        /// <summary>
        ///     List of all registered messages.
        /// </summary>
        private Dictionary<byte, QNetMessage> QNetMessages { get; } = new Dictionary<byte, QNetMessage>();

        /// <summary>
        ///     Set the message handler.
        /// </summary>
        public void SetHandler(QNetMessage message)
        {
            if (IsStarted) throw new InvalidOperationException("QNetPeer can't set new handler while is started.");
            if (QNetMessages.ContainsKey(message.Header))
                throw new InvalidOperationException("Unable to register message handler. " +
                                                    "Given Header has been already registered.");

            QNetMessages.Add(message.Header, message);
        }

        /// <summary>
        ///     Try to read and invoke message handler.
        /// </summary>
        public QNetPeerMessageResult InvokeHandler(QNetMessageReader reader)
        {
            if (!IsStarted)
                throw new InvalidOperationException("QNetPeer can't invoke network " +
                                                    "message handler while is not started.");

            var messageHeader = reader.ReadByte();
            if (!QNetMessages.ContainsKey(messageHeader))
            {
                JEMLogger.LogError($"You are trying to invoke not registered network message called '{messageHeader}'.",
                    "QNet");
                return new QNetPeerMessageResult
                {
                    WasSuccessful = false,
                    DisallowToRecycle = false
                };
            }

            var disallowToRecycle = false;
            QNetMessages[messageHeader].MessageDelegate
                .Invoke(QNetMessages[messageHeader], reader, ref disallowToRecycle);
            return new QNetPeerMessageResult
            {
                WasSuccessful = true,
                DisallowToRecycle = disallowToRecycle
            };
        }

        /// <summary>
        ///     Generate new outgoing message.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="NotSupportedException"/>
        public QNetMessageWriter GenerateOutgoingMessage(Enum messageHeader, params object[] args) =>
            GenerateOutgoingMessage(GetByteFromEnum(messageHeader), args);

        /// <summary>
        ///     Generate new outgoing message.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="NotSupportedException"/>
        public QNetMessageWriter GenerateOutgoingMessage(byte messageHeader, params object[] args)
        {
            if (!IsStarted)
                throw new InvalidOperationException("QNetPeer can't generate outgoing message while is not started.");
            if (messageHeader <= 0) throw new ArgumentOutOfRangeException(nameof(messageHeader));

            var message = OriginalPeer.CreateMessage();
            OnBeforeMessage?.Invoke(message);
            message.Write(messageHeader);

            var proxy = new QNetMessageWriter(message);
            for (var index = 0; index < args.Length; index++)
            {
                var o = args[index];
                WriteObjectToMessage(proxy, o);
            }

            return new QNetMessageWriter(message);
        }

        /// <summary>
        ///     Prepares the message reader from received network message.
        /// </summary>
        public QNetMessageReader ReadIncomingMessage(QNetConnection connection, NetIncomingMessage message) =>
            new QNetMessageReader(connection, message);

        /// <summary>
        ///     Try to write a object to the given network message writer.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NotSupportedException"/>
        internal static void WriteObjectToMessage(QNetMessageWriter writer, object obj) =>
            WriteObjectToMessage(writer, obj, false);

        /// <summary>
        ///     Try to write a object to the given network message writer.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NotSupportedException"/>
        internal static void WriteObjectToMessage(QNetMessageWriter writer, object obj, bool includeTypeIndex)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer),
                    "Unable to write object to message. Target MessageWriter is null.");
            if (obj == null)
                throw new ArgumentNullException(nameof(obj),
                    "You are trying to write a null object to the MessageWriter.");

            if (includeTypeIndex)
            {
                var type = GetObjectType(obj);
                writer.WriteEnum(type);
            }

            switch (obj)
            {
                case int i32:
                    writer.WriteInt32(i32);
                    break;
                case uint ui32:
                    writer.WriteUInt32(ui32);
                    break;
                case short i16:
                    writer.WriteInt16(i16);
                    break;
                case ushort ui16:
                    writer.WriteUInt16(ui16);
                    break;
                case long i64:
                    writer.WriteInt64(i64);
                    break;
                case ulong ui64:
                    writer.WriteUInt64(ui64);
                    break;
                case float f:
                    writer.WriteSingle(f);
                    break;
                case double d:
                    writer.WriteDouble(d);
                    break;
                case byte b:
                    writer.WriteByte(b);
                    break;
                case string s:
                    writer.WriteString(s);
                    break;
                case bool b:
                    writer.WriteBool(b);
                    break;
                case byte[] bArray:
                    writer.WriteBytes(bArray);
                    break;
                case IQNetSerializedMessage m:
                    writer.WriteMessage(m, true);
                    // m.Serialize(writer);
                    break;
                case Enum e:
                    writer.WriteEnum(e);
                    break;
                default:
                    throw new NotSupportedException($"Not supported message parameter type {obj.GetType().Name}.");
            }
        }

        /// <summary>
        ///     Try to read the object of given type from given network message reader.
        ///     NOTE: Object reading could only be possible when value has been written via <see cref="WriteObjectToMessage"/> with includeTypeIndex set to true.
        /// </summary>
        internal static object ReadObjectFromMessage(QNetMessageReader reader, params object[] additionalReaderParams) =>
            ReadObjectFromMessage(reader, reader.ReadEnum<QNetMessageValue>(), additionalReaderParams);

        /// <summary>
        ///     Try to read the object of given type from given network message reader.
        ///     NOTE: Object reading could only be possible when value has been written via <see cref="WriteObjectToMessage"/> with includeTypeIndex set to true.
        /// </summary>
        public static object ReadObjectFromMessage(QNetMessageReader reader, QNetMessageValue valueType,
            params object[] additionalReaderParams)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (!Enum.IsDefined(typeof(QNetMessageValue), valueType))
                throw new InvalidEnumArgumentException(nameof(valueType), (int) valueType, typeof(QNetMessageValue));

            switch (valueType)
            {
                case QNetMessageValue.Int32:
                    return reader.ReadInt32();
                case QNetMessageValue.UInt32:
                    return reader.ReadUInt32();
                case QNetMessageValue.Int16:
                    return reader.ReadInt16();
                case QNetMessageValue.UInt16:
                    return reader.ReadUInt16();
                case QNetMessageValue.Int64:
                    return reader.ReadInt64();
                case QNetMessageValue.UInt64:
                    return reader.ReadUInt64();
                case QNetMessageValue.Single:
                    return reader.ReadSingle();
                case QNetMessageValue.Double:
                    return reader.ReadDouble();
                case QNetMessageValue.Byte:
                    return reader.ReadByte();
                case QNetMessageValue.SByte:
                    return reader.ReadSByte();
                case QNetMessageValue.String:
                    return reader.ReadString();
                case QNetMessageValue.Boolean:
                    return reader.ReadBool();
                case QNetMessageValue.ByteArray:
                    return reader.ReadBytes();
                case QNetMessageValue.SerializedMessage:
                    return reader.ReadMessage();
                    // return reader.ReadMessage(additionalReaderParams[0] as Type);
                case QNetMessageValue.Enum:
                    return reader.ReadEnum(additionalReaderParams[0] as Type);
                default:
                    throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null);
            }
        }

        /// <summary>
        ///     Returns a <see cref="QNetMessageValue"/> that match the type of given object.
        /// </summary>
        public static QNetMessageValue GetObjectType(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            switch (obj)
            {
                case int i32:
                    return QNetMessageValue.Int32;
                case uint ui32:
                    return QNetMessageValue.UInt32;
                case short i16:
                    return QNetMessageValue.Int16;
                case ushort ui16:
                    return QNetMessageValue.UInt16;
                case long i64:
                    return QNetMessageValue.Int64;
                case ulong ui64:
                    return QNetMessageValue.UInt64;
                case float f:
                    return QNetMessageValue.Single;
                case double d:
                    return QNetMessageValue.Double;
                case byte b:
                    return QNetMessageValue.Byte;
                case string s:
                    return QNetMessageValue.String;
                case bool b:
                    return QNetMessageValue.Boolean;
                case byte[] bArray:
                    return QNetMessageValue.ByteArray;
                case IQNetSerializedMessage m:
                    return QNetMessageValue.SerializedMessage;
                case Enum e:
                    return QNetMessageValue.Enum;
                default:
                    throw new NotSupportedException($"Not supported object type {obj.GetType().FullName}.");
            }
        }

        /// <summary>
        ///     Collects all Types from all assemblies loaded in to <see cref="AppDomain.CurrentDomain"/> that implements <see cref="IQNetSerializedMessage"/> 
        ///      and registers them in a array with byte index for faster and proper generic type serialization.
        /// </summary>
        internal static void CollectAllSerializedMessageTypes()
        {
            if (SerializedMessageTypes.Count != 0)
            {
                // Collect once.
                return;
            }

            // SerializedMessageTypes.Clear();

            var baseType = typeof(IQNetSerializedMessage);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var index1 = 0; index1 < assemblies.Length; index1++)
            {
                var assembly = assemblies[index1];
                var allTypes = assembly.GetTypes();
                for (var index2 = 0; index2 < allTypes.Length; index2++)
                {
                    var type = allTypes[index2];
                    if (type == baseType)
                        continue; // lol

                    //if (!type.IsClass && !(type.IsValueType && !type.IsPrimitive))
                    //{
                    //    continue;
                    //}

                    if (!baseType.IsAssignableFrom(type)) continue;
                    if (!GetSerializedMessageIndex(type, out _))
                    {
                        SerializedMessageTypes.Add((byte) SerializedMessageTypes.Count, type);
                    }
                }
            }

#if DEBUG
            var sb =
 new StringBuilder($"All IQNetSerializedMessage based types collected ({SerializedMessageTypes.Count}).\n");
            foreach (var t in SerializedMessageTypes)
                sb.AppendLine($"\t{t.Value.FullName}: {t.Key}");

            JEMLogger.Log(sb.ToString(), "QNet");
#endif
        }

        /// <summary>
        ///     Gets the serialized message index by (full)type name.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static bool GetSerializedMessageIndex(Type type, out byte index)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            index = 255;
            foreach (var m in SerializedMessageTypes)
            {
                if (m.Value != type) continue;
                index = m.Key;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Gets the serialized message type by index.
        /// </summary>
        public static bool GetSerializedMessageType(byte index, out Type type)
        {
            type = null;
            if (!SerializedMessageTypes.ContainsKey(index)) return false;
            type = SerializedMessageTypes[index];
            return true;
        }

        /// <summary>
        ///     List of all <see cref="IQNetSerializedMessage"/> based types.
        /// </summary>
        public static Dictionary<byte, Type> SerializedMessageTypes { get; } = new Dictionary<byte, Type>();
    }
}
