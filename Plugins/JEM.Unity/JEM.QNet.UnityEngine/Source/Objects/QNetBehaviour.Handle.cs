//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Messages;
using JEM.QNet.UnityEngine.Messages.Extensions;
using JEM.QNet.UnityEngine.Simulation;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Objects
{
    // Methods related to QNetBehaviour network message handling (sending).
    public abstract partial class QNetBehaviour
    {
        /// <summary>
        ///     Sends a client message to the server the client is connected with.
        /// </summary>
        /// <remarks>
        ///     Only as owner of the object you can send behaviour's network methods.
        /// </remarks>
        /// <exception cref="ArgumentNullException"/>
        public void SendMessage(string methodName, params object[] args)
        {
            var pointer = GetMessagePointer(methodName);
            if (pointer == null)
                throw new NullReferenceException("Failed to send network message. " +
                                                 $"Message of name {methodName} not exist.");

            SendMessage(pointer.Index, pointer.Attribute.Channel, pointer.Attribute.Method, args);
        }

        /// <summary>
        ///     Sends a client message to the server the client is connected with.
        /// </summary>
        /// <remarks>
        ///     Only as owner of the object you can send behaviour's network methods.
        /// </remarks>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void SendMessage(string methodName, [NotNull] Enum channel, QNetMessageMethod method, params object[] args)
        {
            var pointer = GetMessagePointer(methodName);
            if (pointer == null)
                throw new NullReferenceException("Failed to send network message. " +
                                                 $"Message of name {methodName} not exist.");

            SendMessage(pointer.Index, channel, method, args);
        }

        /// <summary>
        ///     Sends a client message to the server the client is connected with.
        /// </summary>
        /// <remarks>
        ///     Only as owner of the object you can send behaviour's network methods!
        /// </remarks>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        internal void SendMessage(byte messageIndex, [NotNull] Enum channel, QNetMessageMethod method, params object[] args)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (!Enum.IsDefined(typeof(QNetMessageMethod), method))
                throw new InvalidEnumArgumentException(nameof(method), (int) method, typeof(QNetMessageMethod));

            // Prepare method message.
            var outgoingMessage = PrepareMethodMessage(false, messageIndex, args);

            // Send to all.
            // NOTE. as QNetManager.SentToAll ignores hostConnection we need to call this the default way.
            QNetManager.Send(channel, method, outgoingMessage);
        }

        /// <summary>
        ///     Sends a server message to the given connection.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void SendMessage(QNetConnection connection, string methodName, params object[] args)
        {
            var pointer = GetMessagePointer(methodName);
            if (pointer == null)
                throw new NullReferenceException("Failed to send network message. " +
                                                 $"Message of name {methodName} not exist.");

            SendMessage(connection, pointer.Index, pointer.Attribute.Channel, pointer.Attribute.Method, args);
        }

        /// <summary>
        ///     Sends a server message to the given connection.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void SendMessage(QNetConnection connection, string methodName, [NotNull] Enum channel, QNetMessageMethod method, params object[] args)
        {
            var pointer = GetMessagePointer(methodName);
            if (pointer == null)
                throw new NullReferenceException("Failed to send network message. " +
                                                 $"Message of name {methodName} not exist.");

            SendMessage(connection, pointer.Index, channel, method, args);
        }

        /// <summary>
        ///     Sends a server message to the given connection.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        internal void SendMessage(QNetConnection connection, byte messageIndex, [NotNull] Enum channel, QNetMessageMethod method, params object[] args)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (!Enum.IsDefined(typeof(QNetMessageMethod), method))
                throw new InvalidEnumArgumentException(nameof(method), (int)method, typeof(QNetMessageMethod));
            if (connection.Equals(default(QNetConnection)))
                throw new NullReferenceException("Failed to send network message. " +
                                                 "Target connection is equal default.");

            // Prepare method message.
            var outgoingMessage = PrepareMethodMessage(true, messageIndex, args);

            // Send to all.
            // NOTE. as QNetManager.SentToAll ignores hostConnection we need to call this the default way.
            QNetManager.Send(connection, channel, method, outgoingMessage);
        }

        /// <summary>
        ///     Sends a server message to all connected players.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void SendMessageToAll(string methodName, params object[] args)
        {
            var pointer = GetMessagePointer(methodName);
            if (pointer == null)
                throw new NullReferenceException("Failed to send network message. " +
                                                 $"Message of name {methodName} not exist.");

            SendMessageToAll(pointer.Index, pointer.Attribute.Channel, pointer.Attribute.Method, args);
        }

        /// <summary>
        ///     Sends a server message to all connected players.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        public void SendMessageToAll(string methodName, Enum channel, QNetMessageMethod method, params object[] args)
        {
            var pointer = GetMessagePointer(methodName);
            if (pointer == null)
                throw new NullReferenceException("Failed to send network message. " +
                                                 $"Message of name {methodName} not exist.");

            SendMessageToAll(pointer.Index, channel, method, args);
        }

        /// <summary>
        ///     Sends a server message to all connected players.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidEnumArgumentException"/>
        internal void SendMessageToAll(byte messageIndex, [NotNull] Enum channel, QNetMessageMethod method, params object[] args)
        {
            if (channel == null) throw new ArgumentNullException(nameof(channel));
            if (!Enum.IsDefined(typeof(QNetMessageMethod), method))
                throw new InvalidEnumArgumentException(nameof(method), (int) method, typeof(QNetMessageMethod));

            // Prepare method message.
            var outgoingMessage = PrepareMethodMessage(true, messageIndex, args);

            // Send to all.
            // NOTE. as QNetManager.SentToAll ignores hostConnection we need to call this the default way.
            QNetManager.Instance.Server.SendToAll(channel, method, outgoingMessage);
        }

        /// <summary>
        ///     Sends a server message to all connected players.
        ///     Prepares a message of network message.
        /// </summary>
        private QNetMessageWriter PrepareMethodMessage(bool serverMethod, byte messageIndex, params object[] args)
        {
            // Get the pointer.
            var pointer = GetMessagePointer(messageIndex);

            // Generate server outgoing message.
            var outgoingMessage = serverMethod ? QNetManager.GenerateServerMessage(QNetUnityHeader.ENTITY_QUERY) : 
                                                 QNetManager.GenerateClientMessage(QNetUnityHeader.ENTITY_QUERY);

            // Write header.
            outgoingMessage.WriteUInt16(Identity);
            outgoingMessage.WriteByte(ComponentIndex);
            outgoingMessage.WriteByte(messageIndex);

            // Write the frame this message was send on.
            outgoingMessage.WriteUInt32(QNetTime.ServerFrame);

            // TODO: We may want to check if given args array math the method parameters.

            // Check if the method is generic.
            if (pointer.Method.IsGenericMethod)
            {
                // Write type data for generic method.
                var pars = pointer.Method.GetParameters();
                var genericPass = 0;
                var genericArguments = pointer.Method.GetGenericArguments();
                for (var index1 = 0; index1 < genericArguments.Length; index1++)
                {
                    var genericType = genericArguments[index1];

                    for (var index2 = 0; index2 < pars.Length; index2++)
                    {
                        var parameter = pars[index2];
                        if (!parameter.ParameterType.IsGenericParameter) continue;
                        if (parameter.ParameterType.Name != genericType.Name) continue;
                        if (index2 >= args.Length)
                            throw new IndexOutOfRangeException("Failed to prepare network message. " +
                                                               $"Given arguments array does not math the size of target method. ({pars.Length} vs {args.Length})");

                        var arg = args[index2];
                        var argType = arg.GetType();
                        if (GetSerializedMessageIndex(argType, out var typeIndex))
                        {
                            outgoingMessage.WriteByte(typeIndex);
                            genericPass++;
                        }
                        else throw new NullReferenceException("Failed to prepare network message. " +
                                                              $"Unable to find a index for type {argType.FullName}");
                    }
                }

                if (genericPass != genericArguments.Length)
                    throw new ArgumentException("Failed prepare network message. " +
                                                "Unable to serialize generic type. " +
                                                $"Types serialized: {genericPass} from {genericArguments.Length}");
            }

            // Write arguments.
            for (var index = 0; index < args.Length; index++)
            {
                var obj = args[index];
                if (obj == null)
                    throw new ArgumentNullException($"Failed to prepate method {pointer.Method.Name} message. " +
                                                    $"Argument of index #{index} is null.");

                // Check if arg is a unity type.
                switch (obj)
                {
                    case Vector3 v3:
                        outgoingMessage.WriteVector3(v3);
                        break;
                    case Vector2 v2:
                        outgoingMessage.WriteVector2(v2);
                        break;
                    case Vector4 v4:
                        outgoingMessage.WriteVector3(v4);
                        break;
                    case Color c:
                        outgoingMessage.WriteColor4(c);
                        break;
                    // We need to overwrite the QNetSerializedMessage for the generic types support.
                    case QNetSerializedMessage m:
                        m.Serialize(outgoingMessage);
                        break;
                    default:
                        // This is not a unity type.
                        // Continue with WriteObject.
                        outgoingMessage.WriteObject(obj);
                        break;
                }
            }

            return outgoingMessage;
        }

        /// <summary>
        ///     Collects all Types that are subclass of <see cref="QNetSerializedMessage"/>
        ///     and registers them in a array with byte index for faster and proper generic type serialization.
        /// </summary>
        public static void CollectAllSerializedMessageTypes()
        {
            SerializedMessageTypes.Clear();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var index1 = 0; index1 < assemblies.Length; index1++)
            {
                var assembly = assemblies[index1];
                var allTypes = assembly.GetTypes();
                for (var index2 = 0; index2 < allTypes.Length; index2++)
                {
                    var type = allTypes[index2];
                    if (!type.IsClass)
                    {
                        continue;
                    }

                    if (!type.IsSubclassOf(typeof(QNetSerializedMessage))) continue;
                    if (!GetSerializedMessageIndex(type, out _))
                    {
                        SerializedMessageTypes.Add((byte) SerializedMessageTypes.Count, type);
                    }
                }
            }

#if DEBUG
            QNetManager.PrintLogMsc($"All QNetSerializedMessage based types collected ({SerializedMessageTypes.Count}).");
#endif
        }

        /// <summary>
        ///     Prints all the serialized messages types with it's indexes.
        /// </summary>
        public static void PrintSerializedMessagesTypes()
        {
#if DEBUG
            foreach (var t in SerializedMessageTypes)
            {
                QNetManager.PrintLogMsc($"{t.Value.FullName}: {t.Key}");
            }
#endif
        }

        /// <summary>
        ///     Gets the serialized message index by (full)type name.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        internal static bool GetSerializedMessageIndex([NotNull] Type type, out byte index)
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
        internal static bool GetSerializedMessageType(byte index, out Type type)
        {
            type = null;
            if (!SerializedMessageTypes.ContainsKey(index)) return false;
            type = SerializedMessageTypes[index];
            return true;

        }

        internal static Dictionary<byte, Type> SerializedMessageTypes { get; } = new Dictionary<byte, Type>();
    }
}
