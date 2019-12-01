//
// JEM Library Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using System;

namespace JEM.Core.Extension
{
    /// <summary>
    ///     Set of utility methods: Array
    /// </summary>
    public static class ExtensionArray
    {
        /// <summary>
        ///     Gets random item from array.
        /// </summary>
        /// <typeparam name="T">Type of object in array.</typeparam>
        /// <param name="array">Array.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static T GetRandom<T>(this T[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (array.Length == 0) throw new ArgumentException("Value cannot be an empty array.", nameof(array));

            var random = new Random();
            return array[random.Next(0, array.Length)];
        }

        // source: https://codereview.stackexchange.com/questions/132630/removing-n-elements-from-array-starting-from-index
        /// <summary>
        ///     Removes given amount of items starting at given index.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static T[] RemoveAt<T>(this T[] array, int startIndex, int length)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (length < 0)
            {
                startIndex += 1 + length;
                length = -length;
            }

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (startIndex + length > array.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            var newArray = new T[array.Length - length];

            Array.Copy(array, 0, newArray, 0, startIndex);
            Array.Copy(array, startIndex + length, newArray, startIndex, array.Length - startIndex - length);

            return newArray;
        }
    }
}