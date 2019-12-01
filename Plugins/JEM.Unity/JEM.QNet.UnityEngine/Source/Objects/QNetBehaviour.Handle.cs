//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Messages;
using JEM.QNet.UnityEngine.Simulation;
using JetBrains.Annotations;
using System;

namespace JEM.QNet.UnityEngine.Objects
{
    // Methods related to QNetBehaviour network message handling (sending).
    public abstract partial class QNetBehaviour
    {
        ///// <summary>
        /////     Creates a new <see cref="QNetMessageWriter"/>.
        ///// </summary>
        //[NotNull]
        //public QNetMessageWriter CreateNetworkMessage([NotNull] string messagePointerName, out QNetBehaviourOutgoingMessage outgoingMessage) =>
        //    CreateNetworkMessage(IsServer, GetMessagePointer(messagePointerName), out outgoingMessage);

        /// <summary>
        ///     Creates a new <see cref="QNetMessageWriter"/>.
        /// </summary>
        [NotNull]
        public QNetMessageWriter CreateNetworkMessage(bool isServerMessage, [NotNull] string messagePointerName, out QNetBehaviourOutgoingMessage outgoingMessage) => 
            CreateNetworkMessage(isServerMessage, GetMessagePointer(messagePointerName), out outgoingMessage);

        ///// <summary>
        /////     Creates a new <see cref="QNetMessageWriter"/>.
        ///// </summary>
        //[NotNull]
        //public QNetMessageWriter CreateNetworkMessage([NotNull] QNetMessagePointer messagePointer, out QNetBehaviourOutgoingMessage outgoingMessage) => 
        //    CreateNetworkMessage(IsServer, messagePointer, out outgoingMessage);
        
        /// <summary>
        ///     Creates a new <see cref="QNetMessageWriter"/>.
        /// </summary>
        [NotNull]
        public QNetMessageWriter CreateNetworkMessage(bool isServerMessage, [NotNull] QNetMessagePointer messagePointer, out QNetBehaviourOutgoingMessage outgoingMessage)
        {
            if (messagePointer == null) throw new ArgumentNullException(nameof(messagePointer));

            // Generate writer from target peer.
            var writer = isServerMessage ? QNetManager.GenerateServerMessage(QNetUnityHeader.ENTITY_QUERY) :
                QNetManager.GenerateClientMessage(QNetUnityHeader.ENTITY_QUERY);

            // Write header data.
            writer.WriteUInt16(Identity);
            writer.WriteByte(ComponentIndex);
            writer.WriteByte(messagePointer.Index);

            // Write the frame this message was send on.
            writer.WriteUInt32(QNetTime.ServerFrame);

            // Create new outgoingMessage instance
            outgoingMessage = new QNetBehaviourOutgoingMessage(this, isServerMessage, messagePointer, writer);

            return writer;
        }
    }
}
