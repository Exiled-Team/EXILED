// -----------------------------------------------------------------------
// <copyright file="TypeCastObject.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    /// <summary>
    /// The interface which allows defined objects to be cast to each other.
    /// </summary>
    /// <typeparam name="T1">The type of the object to cast.</typeparam>
    public abstract class TypeCastObject<T1>
        where T1 : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeCastObject{T1}"/> class.
        /// </summary>
        public TypeCastObject()
        {
        }

        /// <summary>
        /// Unsafely casts the current <typeparamref name="T1"/> instance to the specified <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The type to which to cast the <typeparamref name="T1"/> instance.</typeparam>
        /// <returns>The cast <typeparamref name="T1"/> instance.</returns>
        public T Cast<T>()
            where T : class => this as T1 as T;

        /// <summary>
        /// Safely casts the current <typeparamref name="T1"/> instance to the specified <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The type to which to cast the <typeparamref name="T1"/> instance.</typeparam>
        /// <param name="param">The cast object.</param>
        /// <returns><see langword="true"/> if the <typeparamref name="T1"/> instance was successfully cast; otherwise, <see langword="false"/>.</returns>
        public bool Cast<T>(out T param)
            where T : class
        {
            param = default;

            if (this as T1 is not T cast)
                return false;

            param = cast;
            return true;
        }
    }
}