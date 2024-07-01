// -----------------------------------------------------------------------
// <copyright file="QueueExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features.Pools;

    /// <summary>
    /// <see cref="Queue{T}"/> extensions.
    /// </summary>
    public static class QueueExtensions
    {
        /// <summary>
        /// Removes a specific value from a queue.
        /// </summary>
        /// <param name="queue">The <see cref="Queue{T}"/> to remove from.</param>
        /// <param name="data">The item to remove.</param>
        /// <typeparam name="T">The <see cref="Type"/> of data used.</typeparam>
        public static void RemoveFromQueue<T>(this Queue<T> queue, T data)
        {
            List<T> toKeep = ListPool<T>.Pool.Get();
            for (int i = 0; i < queue.Count; i++)
            {
                T item = queue.Dequeue();
                if (!item.Equals(data))
                    toKeep.Add(item);
            }

            foreach (T item2 in toKeep)
                queue.Enqueue(item2);

            ListPool<T>.Pool.Return(toKeep);
        }
    }
}