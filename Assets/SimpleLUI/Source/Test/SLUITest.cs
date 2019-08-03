//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEngine;

namespace SimpleLUI.Test
{
    public class SLUITest : MonoBehaviour
    {
        [Header("References")]
        public Canvas Root;

        [Header("Settings")]
        public string SLUIName = "Tests";

        [Header("Files")]
        public string[] LuaFiles;

        public SLUIManager Manager { get; set; }

        private void Awake()
        {
            Manager = SLUIManager.CreateNew(SLUIName, Root);
            Manager.AddFiles(LuaFiles);
            Manager.Reload();
        }
    }
}
