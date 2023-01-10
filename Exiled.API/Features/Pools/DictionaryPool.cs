// -----------------------------------------------------------------------
// <copyright file="DictionaryPool.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pools
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a system used to store and retrieve <see cref="Dictionary{TKey, TValue}"/> objects.
    /// </summary>
    /// <typeparam name="TKey">The type of the key in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the value in the dictionary.</typeparam>
    /// <seealso cref="ListPool{T}"/>
    /// <seealso cref="HashSetPool{T}"/>
    public class DictionaryPool<TKey, TValue> : IPool<Dictionary<TKey, TValue>>
    {
        private readonly ConcurrentQueue<Dictionary<TKey, TValue>> pool = new();

        /// <summary>
        /// Gets a <see cref="DictionaryPool{TKey, TValue}"/> that stores dictionaries.
        /// </summary>
        public static DictionaryPool<TKey, TValue> Pool { get; } = new();

        /// <summary>
        /// Rent a <see cref="Dictionary{TKey, TValue}"/> temporarily.
        /// </summary>
        /// <returns>The <see cref="Dictionary{TKey, TValue}"/>.</returns>
        public Dictionary<TKey, TValue> Get()
        {
            if (pool.TryDequeue(out Dictionary<TKey, TValue> result))
                return result;

            return new();
        }

        /// <summary>
        /// Rent a <see cref="Dictionary{TKey, TValue}"/> temporarily. Fills it with the provided <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        public Dictionary<TKey, TValue> Get(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            if (!pool.TryDequeue(out Dictionary<TKey, TValue> dict))
                dict = new();

            foreach (KeyValuePair<TKey, TValue> pair in pairs)
                dict.Add(pair.Key, pair.Value);

            return dict;
        }

        /// <summary>
        /// Returns a finished <see cref="Dictionary{TKey, TValue}"/> to the pool, clearing all of its contents.
        /// </summary>
        /// <param name="obj">The <see cref="Dictionary{TKey, TValue}"/> to return.</param>
        public void Return(Dictionary<TKey, TValue> obj)
        {
            obj.Clear();
            pool.Enqueue(obj);
        }
    }
}
