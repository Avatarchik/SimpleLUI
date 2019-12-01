//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace JEM.Core.Debugging.CVar
{
    [Serializable]
    internal class JEMCVarSave
    {
        [Serializable]
        public class Item
        {
            public string Key;
            public object Object;
        }

        public List<Item> Items = new List<Item>();

        internal object Get(string key)
        {
            return (from i in Items where i.Key == key select i.Object).FirstOrDefault();
        }
    }
}
