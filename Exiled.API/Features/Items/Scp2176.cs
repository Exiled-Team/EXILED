// -----------------------------------------------------------------------
// <copyright file="Scp2176.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Footprinting;

    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for <see cref="Scp2176Projectile"/>.
    /// </summary>
    public class Scp2176 : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp2176"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public Scp2176(ThrowableItem itemBase)
            : base(itemBase)
        {
            Scp2176Projectile grenade = (Scp2176Projectile)Base.Projectile;
            FuseTime = grenade._fuseTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp2176"/> class.
        /// </summary>
        /// <param name="player">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        internal Scp2176(Player player = null)
            : this(player is null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(ItemType.SCP2176, false) : (ThrowableItem)player.Inventory.CreateItemInstance(ItemType.SCP2176, true))
        {
        }

        /// <summary>
        /// Gets or sets how long the fuse will last.
        /// </summary>
        public float FuseTime { get; set; }

        /// <summary>
        /// Spawns an active grenade on the map at the specified location.
        /// </summary>
        /// <param name="position">The location to spawn the grenade.</param>
        /// <param name="owner">Optional: The <see cref="Player"/> owner of the grenade.</param>
        public void SpawnActive(Vector3 position, Player owner = null)
        {
#if DEBUG
            Log.Debug($"Spawning active grenade: {FuseTime}");
#endif
            Scp2176Projectile grenade = (Scp2176Projectile)Object.Instantiate(Base.Projectile, position, Quaternion.identity);
            grenade._fuseTime = FuseTime;
            grenade.PreviousOwner = new Footprint(owner is not null ? owner.ReferenceHub : Server.Host.ReferenceHub);
            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
        }

        /// <summary>
        /// Returns the ExplosiveGrenade in a human readable format.
        /// </summary>
        /// <returns>A string containing ExplosiveGrenade-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}|";
    }
}
