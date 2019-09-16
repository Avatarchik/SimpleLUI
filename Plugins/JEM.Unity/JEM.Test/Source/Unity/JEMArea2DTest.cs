//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.UnityEngine.Components;
using System.Collections;
using UnityEngine;

namespace JEM.Test.Unity
{
    internal class JEMArea2DTest : JEMArea2D
    {
        [Header("Test Stuff")]
        public GameObject Obj;

        public bool Unreliable = true;
        public int ObjToGenerate = 10;

        private IEnumerator Start()
        {
            int i = 0;
            while (i < ObjToGenerate)
            {
                i++;

                if (Unreliable)
                {
                    if (GenerateUnreliablePoint(out var point, out var angle))
                    {
                        Instantiate(Obj, point, Quaternion.Euler(0f, 0f, angle)).SetActive(true);
                    }
                    else Debug.LogError("Failed to generate new point.");
                }
                else
                {
                    if (GenerateReliablePoint(out var point, out var angle))
                    {
                        Instantiate(Obj, point, Quaternion.Euler(0f, 0f, angle)).SetActive(true);
                    }
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
