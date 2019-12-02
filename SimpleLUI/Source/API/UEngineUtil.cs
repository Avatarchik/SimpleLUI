﻿//
// SimpleLUI Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;
using UnityEngine;

namespace SimpleLUI.API
{
    /// <summary/>
    internal static class UEngineUtil
    {
        /// <summary/>
        internal static T CollectComponent<T>([NotNull] this GameObject g) where T : Component
        {
            if (g == null) throw new ArgumentNullException(nameof(g));
            var i = g.GetComponent<T>();
            if (i == null)
                i = g.AddComponent<T>();

            return i;
        }

        /// <summary/>
        internal static Component CollectComponent([NotNull] this GameObject g, [NotNull] Type t)
        {
            if (g == null) throw new ArgumentNullException(nameof(g));
            if (t == null) throw new ArgumentNullException(nameof(t));
            var i = g.GetComponent(t);
            if (i == null)
                i = g.AddComponent(t);

            return i;
        }
    }
}
