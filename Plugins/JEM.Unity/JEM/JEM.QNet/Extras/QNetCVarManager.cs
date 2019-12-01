//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging.CVar;
using JEM.QNet.Messages;
using System;

namespace JEM.QNet.Extras
{
    /// <summary>
    ///     A extension to <see cref="JEMCVarManager"/> that will sync server cVars with all connected client peers.
    /// </summary>
    public static class QNetCVarManager
    {
        private static bool _hasNetworkCVarsHooked;

        /// <summary>
        ///     Hooks all network CVars so sever will broadcast all changes to connected client peers.
        /// </summary>
        internal static void HookNetworkCVar()
        {
            if (!QNetGlobal.IsServerActive) return;
            if (_hasNetworkCVarsHooked) return;
            _hasNetworkCVarsHooked = true;

            for (var index = 0; index < JEMCVarManager.RegisteredVars.Count; index++)
            {
                var cVar = JEMCVarManager.RegisteredVars[index];
                if (!cVar.IsNetworkVar)
                    continue;

                // Register change event.
                JEMCVarManager.RegisterCVarChange(cVar.Name, change =>
                {
                    // Send change to all.
                    SendCVarDataToAll(cVar);
                });
            }
        }

        /// <summary>
        ///     Sends all networked cVars to given connection.
        /// </summary>
        public static void SendAllNetworkedCVars(QNetConnection connection)
        {
            for (var index = 0; index < JEMCVarManager.RegisteredVars.Count; index++)
            {
                var cVar = JEMCVarManager.RegisteredVars[index];
                if (!cVar.IsNetworkVar)
                    continue;

                // Send change to all.
                SendCVarData(connection, cVar);
            }
        }

        /// <summary>
        ///     Sends state of given cVar to all connected players.
        /// </summary>
        public static void SendCVarData(QNetConnection connection, JEMCVar cVar)
        {
            if (cVar == null) throw new ArgumentNullException(nameof(cVar));
            var outgoingMessage = GetCVarBroadcastMessage(cVar);
            QNetGlobal.ServerReference.Send(connection, QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered, outgoingMessage);
        }

        /// <summary>
        ///     Sends state of given cVar to all connected players.
        /// </summary>
        public static void SendCVarDataToAll(JEMCVar cVar)
        {
            if (cVar == null) throw new ArgumentNullException(nameof(cVar));
            if (!QNetGlobal.IsServerActive)
                return;

            var outgoingMessage = GetCVarBroadcastMessage(cVar);
            QNetGlobal.ServerReference.SendToAll(QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered, outgoingMessage);
        }

        private static QNetMessageWriter GetCVarBroadcastMessage(JEMCVar cVar)
        {
            if (cVar == null) throw new ArgumentNullException(nameof(cVar));
            var outgoingMessage = QNetGlobal.ServerReference.GenerateOutgoingMessage(QNetBaseHeader.CVAR_DATA);
            outgoingMessage.WriteString(cVar.Name);
            outgoingMessage.WriteObject(cVar.Data, true);
            return outgoingMessage;
        }

        internal static void OnCVarData(QNetMessage message, QNetMessageReader reader, ref bool disallowRecycle)
        {
            if (QNetGlobal.IsServerActive)
                return;

            var cVarName = reader.ReadString();
            var cVarData = reader.ReadObject();

            JEMCVarManager.EditCVar(cVarName, cVarData);
        }
    }
}
