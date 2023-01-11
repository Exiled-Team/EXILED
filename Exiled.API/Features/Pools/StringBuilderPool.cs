// -----------------------------------------------------------------------
// <copyright file="StringBuilderPool.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pools
{
    using System.Collections.Concurrent;
    using System.Text;

    /// <summary>
    /// Defines a system used to store and retrieve <see cref="StringBuilder"/> objects.
    /// </summary>
    public class StringBuilderPool : IPool<StringBuilder>
    {
        private readonly ConcurrentQueue<StringBuilder> pool = new();

        /// <summary>
        /// Gets a <see cref="StringBuilderPool"/> that stores <see cref="StringBuilder"/>.
        /// </summary>
        public static StringBuilderPool Pool { get; } = new();

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

        /// <summary>
        /// Returns the contents of the <see cref="StringBuilder"/> and returns it to the pool.
        /// </summary>
        /// <param name="obj">The <see cref="StringBuilder"/> to return.</param>
        /// <returns>The value of the <see cref="StringBuilder"/>.</returns>
        public string ToStringReturn(StringBuilder obj)
        {
            string s = obj.ToString();
            
            Return(obj);
            
            return s;
        }
    }
}
