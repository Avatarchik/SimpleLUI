//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine;
using UnityEngine;
// ReSharper disable RedundantAssignment

namespace JEM.Test.Unity.QNet
{
    /// <inheritdoc />
    /// <summary>
    ///     A test QNetManager behaviour where we can handle all the methods of Manager.
    /// </summary>
    internal class TestManagerBehaviour : QNetManagerBehaviour
    {
        private void OnClientPrepare(ref uint token, ref string nickname)
        {
            token = (uint) Random.Range(uint.MinValue, uint.MaxValue);
            nickname = "Bob" + Random.Range(ushort.MinValue, ushort.MaxValue);
        }
    }
}
