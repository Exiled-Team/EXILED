// -----------------------------------------------------------------------
// <copyright file="NullableObject.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;

    /// <summary>
    /// The base class which defines nullable objects.
    /// </summary>
    public abstract class NullableObject
    {
// #pragma warning disable SA1401
//         /// <summary>
//         /// Indicates whether the object was destroyed.
//         /// </summary>
//         protected bool destroyedValue = false;
//
//         /// <summary>
//         /// Indicates whether the object is being destroyed.
//         /// </summary>
//         protected bool isDestroying = false;
// #pragma warning restore SA1401
//
//         public delegate void DestroyedEventHandler(NullableObject sender);
//
//         public event DestroyedEventHandler Destroyed;

        /// <summary>
        /// Implicitly converts a <see cref="NullableObject"/> to a boolean value.
        /// </summary>
        /// <param name="object">The <see cref="NullableObject"/> to convert.</param>
        /// <returns><see langword="true"/> if the <see cref="NullableObject"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public static implicit operator bool(NullableObject @object) => @object != null;

        // public void Destroy()
        // {
        //     Destroy(true);
        //     GC.SuppressFinalize(this);
        // }
        //
        // protected virtual void Destroy(bool destroying)
        // {
        //     if (!destroyedValue && !isDestroying)
        //     {
        //         if (destroying)
        //         {
        //             isDestroying = true;
        //         }
        //
        //         destroyedValue = true;
        //
        //         Destroyed?.Invoke(this);
        //     }
        // }
    }
}
