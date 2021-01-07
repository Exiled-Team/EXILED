// -----------------------------------------------------------------------
// <copyright file="DoorExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Contains extensions related to <see cref="DoorVariant"/>.
    /// </summary>
    public static class DoorExtensions
    {
        /// <summary>
        /// Breaks the specified door, if it is not already broken.
        /// </summary>
        /// <param name="door">The door to break.</param>
        /// <returns>True if the door was broken, false if it was unable to be broken, or was already broken before.</returns>
        public static bool BreakDoor(this DoorVariant door)
        {
            if (door is IDamageableDoor dmg && !dmg.IsDestroyed)
            {
                dmg.ServerDamage(ushort.MaxValue, DoorDamageType.ServerCommand);
                return true;
            }

            return false;
        }
    }
}
