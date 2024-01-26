// -----------------------------------------------------------------------
// <copyright file="DictionaryPool.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generic.Pools
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

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

        private DictionaryPool()
        {
        }

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
            {
                pool.Enqueue(new Dictionary<TKey, TValue>());
                result.Clear();
            }
            else
            {
                result = new Dictionary<TKey, TValue>();
            }

            return result;
        }

        /// <summary>
        /// Rent a <see cref="Dictionary{TKey, TValue}"/> temporarily. Fills it with the provided <see cref="IEnumerable{T}"/> of <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </summary>
        /// <param name="pairs">The items to fill the dictionary with.</param>
        /// <returns>The <see cref="Dictionary{TKey, TValue}"/>.</returns>
        public Dictionary<TKey, TValue> Get(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            Dictionary<TKey, TValue> dict = Get();

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

        /// <summary>
        /// Returns the <see cref="Dictionary{TKey, TValue}"/> to the pool and returns its contents as an array.
        /// </summary>
        /// <param name="obj">The <see cref="Dictionary{TKey, TValue}"/> to return.</param>
        /// <returns>The contents of the returned dictionary as an array.</returns>
        public KeyValuePair<TKey, TValue>[] ToArrayReturn(Dictionary<TKey, TValue> obj)
        {
            KeyValuePair<TKey, TValue>[] array = obj.ToArray();
            Return(obj);
            return array;
        }
    }
}
