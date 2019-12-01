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

            var serverFrame = reader.ReadUInt32();
            if (message.IsClientMessage)
            {
                QNetSimulator.ReceivedServerFrame = serverFrame;
                QNetSimulator.AdjustServerFrames = QNetSimulator.ReceivedServerFrame > QNetTime.ServerFrame;
            } // No need to adjust frames on server side (host).

            // Get object.
            var qNetObject = QNetObject.GetObject(objectIdentity);
            if (qNetObject == null)
            {
                QNetManager.PrintLogWarning("Local peer received QNetEntity query message " +
                                            $"but object of identity {objectIdentity} not exists in local world.");
                return;
            }

            if (!qNetObject.HasStateDeserialized)
            {
            /*
#if DEBUG
                QNetManager.PrintLogMsc("Local peer received QNetEntity query message " +
                                        $"but state of object of identity {objectIdentity} has been not deserialized yet.");
#endif
            */
                return;
            }

            // Get object.
            var componentObject = qNetObject.GetObjectByIndex(componentIndex);
            if (componentObject == null)
            {
#if DEBUG
                QNetManager.PrintLogMsc($"List of Components(Objects) at {objectIdentity} ->\n" +
                                        $"{qNetObject.GetStringOfCurrentObjects()}");
#endif

                throw new NullReferenceException("Local peer received QNetEntity query message " +
                                                 $"buy we failed to find component of index {componentIndex} on object of identity {objectIdentity}.");
            }

            // Get behaviour.
            if (!(componentObject is QNetBehaviour behaviour))
            {
#if DEBUG
                QNetManager.PrintLogMsc($"List of Components(Objects) at {objectIdentity} ->\n" +
                                        $"{qNetObject.GetStringOfCurrentObjects()}");
#endif

                throw new InvalidCastException("Local peer received QNetEntity query message " +
                                               $"but target componentObject of index {componentIndex} on object of identity {objectIdentity} " +
                                               $"is not a QNetBehaviour based type ({componentObject.GetType().FullName}).");
            }

            // Execute message.
            behaviour.ExecuteMessage(methodIndex, reader);
        }
    }
}
