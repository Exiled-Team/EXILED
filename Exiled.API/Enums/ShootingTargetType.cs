// -----------------------------------------------------------------------
// <copyright file="ShootingTargetType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Shooting target types present in the game.
    /// </summary>
    /// <seealso cref="Features.Toys.ShootingTargetToy.Type"/>
    public enum ShootingTargetType
    {
        /// <summary>
        /// Unknown target.
        /// </summary>
        Unknown,

        /// <summary>
        /// Radial sport target.
        /// </summary>
        Sport,

        /// <summary>
        /// D-Class target.
        /// </summary>
        ClassD,

        /// <summary>
        /// Binary target.
        /// </summary>
        Binary,
    }
}