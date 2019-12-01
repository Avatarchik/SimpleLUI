//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine;
using JEM.UnityEngine.Effects;
using JEM.UnityEngine.Extension;
using System.Collections;
using UnityEngine;

namespace JEM.Test.Unity
{
    internal class JEMEffects2DTest : JEMEffects2D<JEMEffects2DTest>
    {
        [Header("Test")]
        public GameObject EffectPrefab;
        public int QuantityToSpawn = 5;

        /// <inheritdoc />
        protected override void OnAwake()
        {
            // ignore
        }

        private IEnumerator Start()
        {
            while (true)
            {
                var pos = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
                QueueItem(EffectPrefab, pos);
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnGUI()
        {
            var c = 0;
            foreach (var i in transform)
            {
                c++;
            }
            GUILayout.Box("Spawned: " + c);
        }

        /// <inheritdoc />
        protected override void OnItemSpawn(JEMEffect2DSettings settings, int quantityRemoved)
        {
            JEMObject.LiteInstantiate(settings.Prefab, settings.Point, Quaternion.Euler(0f, 0f, settings.Angle), obj =>
            {
                obj.transform.SetParent(transform);
                obj.LiteSetActive(true);
                Destroy(obj, 1f);
            });
        }
    }
}
