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
        /// <summary>
        /// Implicitly converts a <see cref="NullableObject"/> to a boolean value.
        /// </summary>
        /// <param name="object">The <see cref="NullableObject"/> to convert.</param>
        /// <returns><see langword="true"/> if the <see cref="NullableObject"/> is not <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public static implicit operator bool(NullableObject @object) => @object != null;
    }
}
