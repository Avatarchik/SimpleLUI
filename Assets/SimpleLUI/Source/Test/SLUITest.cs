//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Collections;
using UnityEngine;

namespace SimpleLUI.Test
{
    public class SLUITest : MonoBehaviour
    {
        [Header("References")]
        public Canvas Root;

        [Header("Settings")]
        public string SLUIName = "Tests";
        public float RefreshWait = 2f;

        [Header("Files")]
        public string[] LuaFiles;

        public SLUIManager Manager { get; set; }

        private void Awake()
        {
            // lul
            Screen.fullScreen = false;

            SLUIWorker.LookForCustomTypes();

            Manager = SLUIManager.CreateNew(SLUIName, Root);
            Manager.AddFiles(LuaFiles);
            Manager.Reload();
        }

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(RefreshWait);
                if (Manager.LookForChanges())
                {
                    Manager.Reload();
                }
            }
        }
    }
}
