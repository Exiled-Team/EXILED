// -----------------------------------------------------------------------
// <copyright file="DoorExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using Interactables.Interobjects.DoorUtils;

    using UnityEngine;

    /// <summary>
    /// Contains extensions related to <see cref="DoorVariant"/>.
    /// </summary>
    public static class DoorExtensions
    {
        /// <summary>
        /// Breaks the specified door, if it is not already broken.
        /// </summary>
        /// <param name="door">The <see cref="DoorVariant"/> to break.</param>
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

        /// <summary>
        /// Indicates when the door can be broken.
        /// </summary>
        /// <param name="door">The <see cref="DoorVariant"/>.</param>
        /// <returns>true if the door can be broken; otherwise, false.</returns>
        public static bool IsBreakable(this DoorVariant door) => door is IDamageableDoor dDoor && !dDoor.IsDestroyed;

        /// <summary>
        /// Gets a nametag of a door.
        /// </summary>
        /// <param name="door">The <see cref="DoorVariant"/>.</param>
        /// <returns>A nametag of the door or null.</returns>
        public static string GetNametag(this DoorVariant door) => door.TryGetComponent<DoorNametagExtension>(out var name) ? name.GetName : null;
    }
}
