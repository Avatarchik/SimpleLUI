//
// QNet Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet.Messages
{
    /// <summary>
    ///     QNet serialized message interface.
    /// </summary>
    public interface IQNetSerializedMessage
    {
        /// <summary>
        ///     Serialize the network message.
        /// </summary>
        void Serialize(QNetMessageWriter writer);

        /// <summary>
        ///     De-serialize the network message.
        /// </summary>
        void DeSerialize(QNetMessageReader reader);
    }
}