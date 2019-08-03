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

        [Header("Files")]
        public string[] LuaFiles;

        public SLUIManager Manager { get; set; }

        private void Awake()
        {
            Manager = SLUIManager.CreateNew(Root);
            Manager.AddFiles(LuaFiles);
        }
    }
}
