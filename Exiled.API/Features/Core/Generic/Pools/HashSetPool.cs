// -----------------------------------------------------------------------
// <copyright file="HashSetPool.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generic.Pools
{
    using System.Collections.Generic;
    using System.Linq;

    using BasePools = NorthwoodLib.Pools;

    /// <summary>
    /// Defines a system used to store and retrieve <see cref="HashSet{T}"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the hash set.</typeparam>
    /// <seealso cref="DictionaryPool{TKey, TValue}"/>
    /// <seealso cref="ListPool{T}"/>
    public class HashSetPool<T> : IPool<HashSet<T>>
    {
        private HashSetPool()
        {
        }

        /// <summary>
        /// Gets a <see cref="HashSetPool{T}"/> that stores hash sets.
        /// </summary>
        public static HashSetPool<T> Pool { get; } = new();

        /// <inheritdoc/>
        public HashSet<T> Get() => BasePools.HashSetPool<T>.Shared.Rent();

        /// <summary>
        /// Retrieves a stored object of type <see cref="HashSet{T}"/>, or creates it if it does not exist. The hashset will be filled with all the provided <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to fill the hashset with.</param>
        /// <returns>The stored object, or a new object, of type <see cref="HashSet{T}"/>.</returns>
        public HashSet<T> Get(IEnumerable<T> items) => BasePools.HashSetPool<T>.Shared.Rent(items);

        /// <inheritdoc/>
        public void Return(HashSet<T> obj) => BasePools.HashSetPool<T>.Shared.Return(obj);

        /// <summary>
        /// Returns the <see cref="HashSet{T}"/> to the pool and returns its contents as an array.
        /// </summary>
        /// <param name="obj">The <see cref="HashSet{T}"/> to return.</param>
        /// <returns>The contents of the returned hashset as an array.</returns>
        public T[] ToArrayReturn(HashSet<T> obj)
        {
            T[] array = obj.ToArray();
            Return(obj);
            return array;
        }
    }
}
