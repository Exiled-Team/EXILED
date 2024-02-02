// -----------------------------------------------------------------------
// <copyright file="IPool.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generic.Pools
{
    /// <summary>
    /// Defines the contract for a class that stores and retrieves commonly used objects.
    /// </summary>
    /// <typeparam name="T">The type that is stored in the pool.</typeparam>
    public interface IPool<T>
    {
        /// <summary>
        /// Retrieves a stored object of type <typeparamref name="T"/>, or creates it if it does not exist.
        /// </summary>
        /// <returns>The stored object, or a new object, of type <typeparamref name="T"/>.</returns>
        public T Get();

        /// <summary>
        /// Returns the object to the pool.
        /// </summary>
        /// <param name="obj">The object to return, of type <typeparamref name="T"/>.</param>
        public void Return(T obj);
    }
}
