// -----------------------------------------------------------------------
// <copyright file="MovementDirection.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Indicates the direction a player is moving.
    /// </summary>
    public enum MovementDirection
    {
        /// <summary>
        /// The player is not moving.
        /// </summary>
        None,

        /// <summary>
        /// The player is moving forwards.
        /// </summary>
        Forward,

        /// <summary>
        /// The player is moving backwards.
        /// </summary>
        Backwards,

        /// <summary>
        /// The player is moving to the right.
        /// </summary>
        Right,

        /// <summary>
        /// The player is moving to the left.
        /// </summary>
        Left,
    }
}
