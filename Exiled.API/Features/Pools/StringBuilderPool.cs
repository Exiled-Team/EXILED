// -----------------------------------------------------------------------
// <copyright file="ListPool.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pools
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Text;

    internal class StringBuilderPool : IPool<StringBuilder>
    {
        private readonly ConcurrentQueue<StringBuilder> pool = new();

        /// <summary>
        /// Gets a <see cref="StringBuilderPool"/> that stores <see cref="StringBuilder"/>.
        /// </summary>
        public static StringBuilder Pool { get; } = new();

        /// <inheritdoc/>
        public StringBuilder Get()
        {
            if (pool.TryDequeue(out StringBuilder sb))
                return sb;

            return new();
        }

        /// <summary>
        /// Retrieves a stored object of type <see cref="StringBuilder"/>, or creates it if it does not exist. The capacity of the StringBuilder will be equal to or greater than <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The capacity of content in the <see cref="StringBuilder"/>.</param>
        /// <returns>The stored object, or a new object, of type <see cref="StringBuilder"/>.</returns>
        public StringBuilder Get(int capacity)
        {
            if (pool.TryDequeue(out StringBuilder sb))
            {
                sb.EnsureCapacity(capacity);
                return sb;
            }

            return new(capacity);
        }

        /// <inheritdoc/>
        public void Return(StringBuilder obj)
        {
            obj.Clear();
            pool.Enqueue(obj);
        }
    }
}
