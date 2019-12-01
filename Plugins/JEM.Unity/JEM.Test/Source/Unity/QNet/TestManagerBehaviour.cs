//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine;
using JEM.QNet.UnityEngine.Objects;
using System.Collections.Generic;
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
        public bool TestSpawn;
        public int SpawnTestObject = 63;
        public bool TestPooling;
        public float TestPoolingLifeTime = 1f;
        public bool DestroyAndReturnToPool = true;

        private List<QNetIdentity> Spawned { get; } = new List<QNetIdentity>();

        private void OnClientPrepare(ref uint token, ref string nickname)
        {
            token = (uint) Random.Range(uint.MinValue, uint.MaxValue);
            nickname = "Bob" + Random.Range(ushort.MinValue, ushort.MaxValue);
        }

        private void OnNetworkWorldInitialized()
        {
            if (TestSpawn && QNetObject.IsServer)
            {
                for (int index = 0; index < SpawnTestObject; index++)
                {
                    var spawnedObject = QNetObject.ServerSpawn(QNetManager.Instance.DatabaseReference.Prefabs[0],
                        RandomVector3(), Quaternion.identity);
                    Spawned.Add(spawnedObject);
                }
            }
        }

        private float _randomAdd;
        private float _randomRemove;
        private float _randomAddSpeed = 1f;
        private float _randomRemoveSpeed = 2f;
        private float _randomMove;

        private void Update()
        {
            if (!QNetObject.IsServer || !TestPooling)
                return;

            if (_randomAdd < 1f)
                _randomAdd += Time.deltaTime * _randomAddSpeed;
            else
            {
                _randomAdd = 0f;
                _randomAddSpeed = Random.Range(1.5f, 6f);

                var spawnedObject = QNetObject.ServerSpawn(QNetManager.Instance.DatabaseReference.Prefabs[0], RandomVector3(), Quaternion.identity);
                Spawned.Add(spawnedObject);

                // Debug.Log("Obj added.", spawnedObject);
            }

            if (_randomRemove < 1f)
                _randomRemove += Time.deltaTime * _randomRemoveSpeed;
            else
            {
                _randomRemove = 0f;
                _randomRemoveSpeed = Random.Range(1.5f, 6f) * TestPoolingLifeTime;

                if (Spawned.Count > 0)
                {
                    var obj = Spawned[0];
                    Spawned.RemoveAt(0);
                    // Debug.Log("Obj destroyed.", obj);

                    QNetObject.ServerDestroy(obj, DestroyAndReturnToPool);
                }
            }

            for (var index = 0; index < Spawned.Count; index++)
            {
                var s = Spawned[index];
                s.transform.position = new Vector3(Spawned.Count / 2 + (index * 1.1f) + _randomMove, 1f, 0f);
            }

            _randomMove += Time.deltaTime * 2f;
            if (_randomMove > 5f)
                _randomMove = -5f;
        }

        private Vector3 RandomVector3() => new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }
}
