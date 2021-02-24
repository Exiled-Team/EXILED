// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;

    using Interactables.Interobjects.DoorUtils;

    using UnityEngine;

    /// <summary>
    /// A collection of API methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Tries to get the <see cref="Transform"/> of the door used for a specific <see cref="SpawnLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> to check.</param>
        /// <returns>The <see cref="Transform"/> used for that spawn location. Can be null.</returns>
        public static Transform GetDoor(this SpawnLocation location)
        {
            if (!SpawnLocationData.DoorNames.TryGetValue(location, out string doorName))
                return null;

            return DoorNametagExtension.NamedDoors.TryGetValue(doorName, out var nametag) ? nametag.transform : null;
        }

        /// <summary>
        /// Tries to get the <see cref="Vector3"/> used for a specific <see cref="SpawnLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="SpawnLocation"/> to check.</param>
        /// <returns>The <see cref="Vector3"/> used for that spawn location. Can be <see cref="Vector3.zero"/>.</returns>
        public static Vector3 GetPosition(this SpawnLocation location)
        {
            Transform transform = location.GetDoor();

            if (transform == null)
                return default;

            return transform.position + (Vector3.up * 1.5f) + (transform.forward * (SpawnLocationData.ReversedLocations.Contains(location) ? -3f : 3f));
        }

        /// <summary>
        /// Converts a <see cref="Vector3"/> to a <see cref="Vector"/>.
        /// </summary>
        /// <param name="vector3">The <see cref="Vector3"/> to be converted.</param>
        /// <returns>Returns the <see cref="Vector"/>.</returns>
        public static Vector ToVector(this Vector3 vector3) => new Vector(vector3.x, vector3.y, vector3.z);

        /// <summary>
        /// Reloads the current <see cref="Player"/>'s weapon.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> who's weapon should be reloaded.</param>
        public static void ReloadWeapon(this Player player) => player.ReferenceHub.weaponManager.RpcReload(player.ReferenceHub.weaponManager.curWeapon);
    }
}
