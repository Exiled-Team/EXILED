// -----------------------------------------------------------------------
// <copyright file="StringBuilderPool.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pools
{
    using System.Text;

    using BasePools = NorthwoodLib.Pools;

    /// <summary>
    /// Defines a system used to store and retrieve <see cref="StringBuilder"/> objects.
    /// </summary>
    public class StringBuilderPool : IPool<StringBuilder>
    {
        private StringBuilderPool()
        {
        }

        /// <summary>
        /// Gets a <see cref="StringBuilderPool"/> that stores <see cref="StringBuilder"/>.
        /// </summary>
        public static StringBuilderPool Pool { get; } = new();

        /// <inheritdoc/>
        public StringBuilder Get() => BasePools.StringBuilderPool.Shared.Rent();

        /// <summary>
        /// Retrieves a stored object of type <see cref="StringBuilder"/>, or creates it if it does not exist. The capacity of the StringBuilder will be equal to or greater than <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The capacity of content in the <see cref="StringBuilder"/>.</param>
        /// <returns>The stored object, or a new object, of type <see cref="StringBuilder"/>.</returns>
        public StringBuilder Get(int capacity) => BasePools.StringBuilderPool.Shared.Rent(capacity);

        /// <inheritdoc/>
        public void Return(StringBuilder obj) => BasePools.StringBuilderPool.Shared.Return(obj);

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
