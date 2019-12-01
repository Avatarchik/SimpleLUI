//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.ComponentModel;

namespace JEM.QNet
{
    // Some additional methods for peer related to Statistics.
    public abstract partial class QNetPeer
    {
        /// <summary>
        ///     Gets the statistic value of given name from this Peer.
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public float GetStatisticValue(QNetStatisticName statisticName)
        {
            if (!Enum.IsDefined(typeof(QNetStatisticName), statisticName))
                throw new InvalidEnumArgumentException(nameof(statisticName), (int) statisticName,
                    typeof(QNetStatisticName));

            float value;
            switch (statisticName)
            {
                case QNetStatisticName.ReceivedBytes:
                    value = OriginalPeer.Statistics.ReceivedBytes;
                    break;
                case QNetStatisticName.ReceivedPackets:
                    value = OriginalPeer.Statistics.ReceivedPackets;
                    break;
                case QNetStatisticName.ReceivedMessages:
                    value = OriginalPeer.Statistics.ReceivedMessages;
                    break;
                case QNetStatisticName.SentBytes:
                    value = OriginalPeer.Connections.Count == 0 ? 0 : 
                        OriginalPeer.Connections[0].Statistics.SentBytes;
                    break;
                case QNetStatisticName.SentPackets:
                    value = OriginalPeer.Connections.Count == 0 ? 0 : 
                        OriginalPeer.Connections[0].Statistics.SentPackets;
                    break;
                case QNetStatisticName.SentMessages:
                    value = OriginalPeer.Connections.Count == 0 ? 0 : 
                        OriginalPeer.Connections[0].Statistics.SentMessages;
                    break;
                case QNetStatisticName.MessagesInRecycle:
                    value = !Configuration.UseMessageRecycling ? 0 : 
                        OriginalPeer.Statistics.BytesInRecyclePool;
                    break;
                case QNetStatisticName.AllocatedBytes:
                    value = OriginalPeer.Statistics.StorageBytesAllocated;
                    break;
                case QNetStatisticName.ConnectionLatency:
                    value = ConnectionLatency;
                    break;
                case QNetStatisticName.Dropped:
                    value = OriginalPeer.Connections.Count == 0 ? 0 :
                        OriginalPeer.Connections[0].Statistics.DroppedMessages;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statisticName), statisticName, null);
            }

            return value;
        }
    }
}
