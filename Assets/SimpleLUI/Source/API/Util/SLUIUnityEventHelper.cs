//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleLUI.API.Util
{
    [ExecuteInEditMode]
    public class SLUIUnityEventHelper : MonoBehaviour
    {
        [Serializable]
        public class Item
        {
            public List<string> obj = new List<string>();
        }

        public List<Item> Items = new List<Item>();
    }
}
