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
    public abstract class JEMPoolManager : IJEMPoolManager
    {
        /// <inheritdoc />
        public List<IJEMPoolItem> Pool { get; set; }

        private bool _isPreparing;

        /// <summary>
        ///     Class Constructor.
        /// </summary>
        protected JEMPoolManager() => Pool = new List<IJEMPoolItem>();

        /// <inheritdoc />
        public IJEMPoolItem ResolveItem(params object[] args)
        {
            if (Pool == null) Pool = new List<IJEMPoolItem>();

            // Try to get item from pool.
            IJEMPoolItem item = null;
            int indexOf = -1;
            if (!_isPreparing)
            {
                for (var index = 0; index < Pool.Count; index++)
                {
                    var i = Pool[index];
                    if (i.IsPooled)
                    {
                        item = i;
                        indexOf = index;
                        break;
                    }
                }

                if (item != null && !item.IsValid())
                {
                    DestroyItem(item);
                    item = null;
                }
            }

            if (item == null)
            {
                // No item
                // Create new.
                item = CreateItem(args);
                item.IsPooled = false;

                Pool.Add(item);
            }
            else
            {
                item.IsPooled = false;
                item.OnResolved(args);
                Pool[indexOf] = item;
            }

            return item;
        }

        /// <inheritdoc />
        public void PreparePool(ushort amount)
        {
            _isPreparing = true;
            while (amount > 0)
            {
                amount--;
                PoolItem(ResolveItem());
            }
            _isPreparing = false;
        }

        /// <inheritdoc />
        public abstract IJEMPoolItem CreateItem(object[] args);

        /// <inheritdoc />
        public void DestroyItem(IJEMPoolItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (!Pool.Contains(item))
                throw new ArgumentException("Given item is not a part of local pool.");

            item.Dispose();
            Pool.Remove(item);
        }

        /// <inheritdoc />
        public void PoolItem(IJEMPoolItem item)
        {
            if (!_isPreparing)
            {
                if (item == null) throw new ArgumentNullException(nameof(item));
                if (!Pool.Contains(item))
                    throw new ArgumentException("Given item is not a part of local pool.");
            }

            var indexOf = Pool.IndexOf(item);
            item.IsPooled = true;
            item.OnPooled();
            Pool[indexOf] = item;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Pool == null)
                return;

            foreach (var p in Pool)
                p.Dispose();

            Pool.Clear();
            Pool = null;
        }
    }
}
