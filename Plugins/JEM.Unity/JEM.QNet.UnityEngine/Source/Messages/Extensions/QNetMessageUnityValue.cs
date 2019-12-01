//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.Messages;

namespace JEM.QNet.UnityEngine.Messages.Extensions
{
    public enum QNetMessageUnityValue : byte
    {
        Unknown = QNetMessageValue.Enum + 1,
        Vector2,
        Vector3,
        Vector4,
        Quaternion,
        Color4
    }
}
