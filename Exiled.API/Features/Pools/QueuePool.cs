// -----------------------------------------------------------------------
// <copyright file="QueuePool.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pools
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a system used to store and retrieve <see cref="Queue{T}"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the list.</typeparam>
    /// <seealso cref="DictionaryPool{TKey, TValue}"/>
    /// <seealso cref="HashSetPool{T}"/>
    public class QueuePool<T> : IPool<Queue<T>>
    {
        private readonly ConcurrentQueue<Queue<T>> pool = new();

        private QueuePool()
        {
        }

        /// <summary>
        /// Gets a <see cref="QueuePool{T}"/> that stores lists.
        /// </summary>
        public static QueuePool<T> Pool { get; } = new();

        /// <inheritdoc/>
        public Queue<T> Get()
        {
            if (pool.TryDequeue(out Queue<T> queue))
                return queue;

            return new();
        }

        /// <summary>
        /// Retrieves a stored object of type <see cref="List{T}"/>, or creates it if it does not exist. The list will be filled with all the provided <paramref name="items"/>.
        /// </summary>
        /// <param name="items">The items to fill the list with.</param>
        /// <returns>The stored object, or a new object, of type <see cref="List{T}"/>.</returns>
        public Queue<T> Get(IEnumerable<T> items)
        {
            if (pool.TryDequeue(out Queue<T> queue))
            {
                foreach (T item in items)
                    queue.Enqueue(item);

                return queue;
            }

            return new(items);
        }

        /// <inheritdoc/>
        public void Return(Queue<T> obj)
        {
            obj.Clear();
            pool.Enqueue(obj);
        }

        /// <summary>
        /// Returns the <see cref="Queue{T}"/> to the pool and returns its contents as an array.
        /// </summary>
        /// <param name="obj">The <see cref="Queue{T}"/> to return.</param>
        /// <returns>The contents of the returned queue as an array.</returns>
        public T[] ToArrayReturn(Queue<T> obj)
        {
            T[] array = obj.ToArray();

            Return(obj);

            return array;
        }
    }
}
