//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;
using JEM.QNet.UnityEngine.Objects;
using JEM.QNet.UnityEngine.Simulation;
using System;

namespace JEM.QNet.UnityEngine.Handlers
{
    internal static class QNetHandlerEntity
    {
        public static void OnClientEntityQuery(QNetMessage message, QNetMessageReader reader, ref bool disallowRecycle)
        {
            if (QNetNetworkScene.SceneState != QNetSceneState.Loaded)
            {
                // Ignore queries while network scene is not loaded.
                // TODO: Queue the queries and execute them after the network scene loads?
                return;
            }

            var objectIdentity = reader.ReadUInt16();
            var componentIndex = reader.ReadByte();
            var methodIndex = reader.ReadByte();

            if (message.IsClientMessage)
            {
                QNetSimulator.ReceivedServerFrame = reader.ReadUInt32();
                QNetSimulator.AdjustServerFrames = QNetSimulator.ReceivedServerFrame > QNetTime.ServerFrame;
            } else reader.ReadUInt32(); // No need to adjust frames on server side (host).

            // Get object.
            var qNetObject = QNetObject.GetObject(objectIdentity);
            if (qNetObject == null)
            {
                QNetManager.PrintLogWarning("Local machine received QNetEntity query message " +
                                            $"but object of identity {objectIdentity} not exists in local world.");
                return;
            }

            // Get component.
            var component = qNetObject.GetObjectByIndex(componentIndex) as QNetBehaviour;
            if (component == null)
                throw new NullReferenceException();

            // Execute message.
            component.ExecuteMessage(methodIndex, reader);
        }
    }
}
