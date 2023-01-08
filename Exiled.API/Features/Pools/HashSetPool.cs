// -----------------------------------------------------------------------
// <copyright file="HashSetPool.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pools
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a system used to store and retrieve <see cref="HashSet{T}"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the hash set.</typeparam>
    /// <seealso cref="DictPool{TKey, TValue}"/>
    /// <seealso cref="ListPool{T}"/>
    public class HashSetPool<T> : IPool<HashSet<T>>
    {
        private readonly ConcurrentQueue<HashSet<T>> pool = new();

        /// <summary>
        /// Gets a <see cref="HashSetPool{T}"/> that stores hash sets.
        /// </summary>
        public static HashSetPool<T> Pool { get; } = new();

        /// <inheritdoc/>
        public HashSet<T> Get()
        {
            if (pool.TryDequeue(out HashSet<T> set))
                return set;

            return new();
        }

        /// <summary>
        /// Retrieves a stored object of type <see cref="HashSet{T}"/>, or creates it if it does not exist. The hashset will be filled with all the provided <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to fill the hashset with.</param>
        /// <returns>The stored object, or a new object, of type <see cref="HashSet{T}"/>.</returns>
        public HashSet<T> Get(IEnumerable<T> items)
        {
            if (pool.TryDequeue(out HashSet<T> set))
            {
                foreach (T item in items)
                    set.Add(item);
                return set;
            }

            return new(items);
        }

        /// <inheritdoc/>
        public void Return(HashSet<T> obj)
        {
            obj.Clear();
            pool.Enqueue(obj);
        }
    }
}
