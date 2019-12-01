//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;

namespace JEM.Core.Common
{
    /// <summary>
    ///     An experimental queue implementation.
    /// </summary>
    public class JEMPreallocatedQueue<TType>
    {
        /// <summary>
        ///     True when there is any item poped from collection.
        /// </summary>
        public bool HasItemPoped { get; set; }

        private TType[] Collection { get; }

        private int Index { get; set; }
        private int GetIndex { get; set; }
        private int NewItems { get; set; }

        /// <inheritdoc />
        public JEMPreallocatedQueue(TType[] collection)
        {
            Collection = collection ?? throw new ArgumentNullException(nameof(collection));
            if (Collection.Length == 0)
                throw new NotSupportedException("The collection is empty.");

            // Always start from the end of collection
            GetIndex = Collection.Length - 1;
        }

        /// <summary>
        ///     Pops item from collection and moves the index position.
        /// </summary>
        public TType Pop()
        {
            if (HasItemPoped)
                throw new InvalidOperationException($"You are trying to pop another item when the previous one was not pushed.");
                //throw new InvalidOperationException("You are poping more items than current collection has.");

            Index--;
            HasItemPoped = true;

            if (Index < 0)
                Index = Collection.Length - 1;

            return Collection[Index];
        }

        /// <summary>
        ///     Pushes latest poped item.
        /// </summary>
        public void Push(TType type)
        {
            Collection[Index] = type;
            HasItemPoped = false;

            if (NewItems < Collection.Length)
                NewItems++;
        }

        /// <summary>
        ///     Returns next item from queue.
        /// </summary>
        public bool GetNext(out TType item)
        {
            item = default(TType);
            if (NewItems <= 0)
            {
                return false;
            }

            item = Collection[GetIndex];
            GetIndex--;
            NewItems--;
            if (GetIndex < 0)
                GetIndex = Collection.Length - 1;

            return true;
        }
    }
}
