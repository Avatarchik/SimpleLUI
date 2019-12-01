//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace JEM.QNet.UnityEngine.Components
{
    public sealed partial class QNetNavMeshAgent
    {
        public Vector3 Destination
        {
            get => _destination;
            set
            {
                if (Vector3.Distance(_destination, value) <= 0.01f)
                    return;

                _destination = value;
            }
        }
    }
}
