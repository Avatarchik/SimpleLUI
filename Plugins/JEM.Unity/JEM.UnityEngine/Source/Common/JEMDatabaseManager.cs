//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System.Linq;
using UnityEngine;

namespace JEM.UnityEngine.Common
{
    /// <inheritdoc />
    /// <summary>
    ///     A base class of 'Database' manager system that holds references to all items that are <see cref="T:JEM.UnityEngine.Common.JEMDatabaseItem" /> based.
    /// </summary>
    public abstract class JEMDatabaseManager<T> : ScriptableObject where T : JEMDatabaseItem
    {
        /// <summary>
        ///     List of all items in database.
        /// </summary>
        [Header("Settings")]
        public T[] Items = new T[0];

        /// <summary>
        ///     Gets a item by <see cref="JEMDatabaseItem.Identity"/>.
        /// </summary>
        public T GetItem(ushort identity) => Items.FirstOrDefault(t => t.Identity == identity);
    }
}
