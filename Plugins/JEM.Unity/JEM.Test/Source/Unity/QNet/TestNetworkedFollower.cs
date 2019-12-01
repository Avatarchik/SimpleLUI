//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine.Components;
using JEM.QNet.UnityEngine.Objects;
using UnityEngine;

namespace JEM.Test.Unity.QNet
{
    internal class TestNetworkedFollower : QNetBehaviour
    {
        [SerializeField] internal QNetNavMeshAgent Agent;
        [SerializeField] internal QNetBehaviour Target;

        private void UnsafeSimulate() => Agent.Destination = Target.transform.position;
    }
}
