//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.Debugging;
using JEM.QNet.Messages;
using System;
using System.ComponentModel;

namespace JEM.QNet.Extras
{
    /// <summary>
    ///     Interface that implements a types that could execute commands or change cVars over network.
    /// </summary>
    public class QNetExecutor
    {
        /// <summary>
        ///     Reference to the <see cref="QNetConnection"/> of peer that executes target.
        /// </summary>
        /// <remarks>
        ///     NOTE: When <see cref="Connection"/> is default, local/active peer is sending the execution request.
        /// </remarks>
        public QNetConnection Connection { get; }

        /// <summary>
        ///     Serialized data of this executor.
        /// </summary>
        public IQNetSerializedMessage Serialized { get; }

        /// <summary/>
        public QNetExecutor(QNetConnection connection, IQNetSerializedMessage serialized)
        {
            Connection = connection;
            Serialized = serialized;
        }

        /// <summary>
        ///     Sends a log to this executor.
        /// </summary>
        public void SendLog(string str) => SendLog(JEMLogType.Log, str);

        /// <summary>
        ///     Sends a warning to this executor.
        /// </summary>
        public void SendWarning(string str) => SendLog(JEMLogType.Warning, str);

        /// <summary>
        ///     Sends a error to this executor.
        /// </summary>
        public void SendError(string str) => SendLog(JEMLogType.Error, str);

        /// <summary>
        ///     Sends a log to this executor.
        /// </summary>
        public void SendLog(JEMLogType logType, string str)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (!Enum.IsDefined(typeof(JEMLogType), logType))
                throw new InvalidEnumArgumentException(nameof(logType), (int) logType, typeof(JEMLogType));
            if (logType == JEMLogType.Exception)
                throw new ArgumentException("You can send Exception to client peers.");
            if (QNetGlobal.ServerReference == null)
                throw new NullReferenceException("You are trying to send executor log data when ServerReference is not set.");

            if (Connection.Equals(default(QNetConnection)))
            {
                // Connection is missing, apply to local peer.
                ApplyReceivedLogData(logType, str);
            }
            else
            {
                // Send log data.
                var outgoingMessage = QNetGlobal.ServerReference.GenerateOutgoingMessage(QNetBaseHeader.EXECUTOR_LOGS_DATA);
                outgoingMessage.WriteEnum(logType);
                outgoingMessage.WriteString(str);
                QNetGlobal.ServerReference.Send(Connection, QNetBaseChannel.DEFAULT, QNetMessageMethod.ReliableOrdered,
                    outgoingMessage);
            }
        }

        internal static void OnExecutorLogsData(QNetMessage message, QNetMessageReader reader, ref bool disallowRecycle)
        {
            var logType = reader.ReadEnum<JEMLogType>();
            var str = reader.ReadString();
            ApplyReceivedLogData(logType, str);
        }

        private static void ApplyReceivedLogData(JEMLogType logType, string str)
        {
            switch (logType)
            {
                case JEMLogType.Log:
                    JEMLogger.Log(str, "QNetExecutor");
                    break;
                case JEMLogType.Warning:
                    JEMLogger.LogWarning(str, "QNetExecutor");
                    break;
                case JEMLogType.Error:
                    JEMLogger.LogError(str, "QNetExecutor");
                    break;
                case JEMLogType.Exception:
                case JEMLogType.Assert:
                    throw new InvalidOperationException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
            }
        }
    }
}
