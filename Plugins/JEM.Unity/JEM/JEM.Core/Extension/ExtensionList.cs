//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;

namespace JEM.Core.Extension
{
    /// <summary>
    ///     Set of utility methods: List
    /// </summary>
    public static class ExtensionList
    {
        /// <summary>
        ///     Gets random item from list.
        /// </summary>
        /// <typeparam name="T">Type of object in list.</typeparam>
        /// <param name="list">List.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static T GetRandom<T>(this List<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (list.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(list));

            var random = new Random();
            return list[random.Next(0, list.Count)];
        }
    }
}