//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine.Objects;
using UnityEngine;

namespace JEM.Test.Unity.QNet
{
    internal class TestAnotherBehaviour : QNetBehaviour
    {
        private void OnNetworkSpawned()
        {
            Debug.Log("Hello on another component!");
            if (IsOwner)
            {
                Debug.Log("Im the owner of this object!", this);
            }
        }
    }
}
