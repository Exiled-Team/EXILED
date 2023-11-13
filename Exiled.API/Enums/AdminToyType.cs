// -----------------------------------------------------------------------
// <copyright file="AdminToyType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for the different types of admin toys.
    /// </summary>
    /// <seealso cref="Features.Toys.AdminToy.ToyType"/>
    public enum AdminToyType
    {
        /// <summary>
        /// Primitive Object toy.
        /// </summary>
        PrimitiveObject,

        /// <summary>
        /// Light source toy.
        /// </summary>
        LightSource,

        /// <summary>
        /// ShootingTarget toy.
        /// </summary>
        ShootingTarget,
    }
}