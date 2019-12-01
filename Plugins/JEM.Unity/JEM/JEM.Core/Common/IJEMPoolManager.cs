//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;
using System.Collections.Generic;

namespace JEM.Core.Common
{
    /// <inheritdoc />
    /// <summary>
    ///     Interface that implements simple pooling manager.
    /// </summary>
    public interface IJEMPoolManager : IDisposable
    {
        /// <summary>
        ///     A pool.
        /// </summary>
        List<IJEMPoolItem> Pool { get; set; }

        /// <summary>
        ///     Gets free or creates new <see cref="IJEMPoolItem"/> item.
        /// </summary>
        IJEMPoolItem ResolveItem(params object[] args);

        /// <summary>
        ///     Creates given amount of items and pools them.
        /// </summary>
        void PreparePool(ushort amount);

        /// <summary>
        ///     Creates new item that then will be added to pool or returned to user.
        /// </summary>
        IJEMPoolItem CreateItem(object[] args);

        /// <summary>
        ///     Destroys and removes given item from pool.
        /// </summary>
        void DestroyItem(IJEMPoolItem item);

        /// <summary>
        ///     Pools given item.
        /// </summary>
        void PoolItem(IJEMPoolItem item);
    }
}
