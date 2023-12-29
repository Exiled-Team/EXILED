// -----------------------------------------------------------------------
// <copyright file="Scp2176.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System.Diagnostics;

    using Exiled.API.Features.Pickups;

    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;

    using UnityEngine;

    using Scp2176Projectile = Pickups.Projectiles.Scp2176Projectile;

    /// <summary>
    /// A wrapper class for <see cref="Scp2176Projectile"/>.
    /// </summary>
    [DebuggerDisplay("Scp-2176")]
    public class Scp2176 : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp2176"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public Scp2176(ThrowableItem itemBase)
            : base(itemBase)
        {
            Projectile = (Scp2176Projectile)((Throwable)this).Projectile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp2176"/> class.
        /// </summary>
        /// <param name="player">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        internal Scp2176(Player player = null)
            : this((ThrowableItem)(player ?? Server.Host).Inventory.CreateItemInstance(new(ItemType.SCP2176, 0), true))
        {
        }

        /// <summary>
        /// Gets a <see cref="Scp2176Projectile"/> to change grenade properties.
        /// </summary>
        public new Scp2176Projectile Projectile { get; }

        /// <summary>
        /// Gets or sets how long the fuse will last.
        /// </summary>
        public float FuseTime
        {
            get => Projectile.FuseTime;
            set => Projectile.FuseTime = value;
        }

        /// <summary>
        /// Spawns an active grenade on the map at the specified location.
        /// </summary>
        /// <param name="position">The location to spawn the grenade.</param>
        /// <param name="owner">Optional: The <see cref="Player"/> owner of the grenade.</param>
        /// <returns>Spawned <see cref="Scp2176Projectile">grenade</see>.</returns>
        public Scp2176Projectile SpawnActive(Vector3 position, Player owner = null)
        {
#if DEBUG
            Log.Debug($"Spawning active grenade: {FuseTime}");
#endif
            ItemPickupBase ipb = Object.Instantiate(Projectile.Base, position, Quaternion.identity);

            ipb.Info = new PickupSyncInfo(Type, Weight, ItemSerialGenerator.GenerateNext());

            Scp2176Projectile grenade = (Scp2176Projectile)Pickup.Get(ipb);

            grenade.Base.gameObject.SetActive(true);

            grenade.FuseTime = FuseTime;

            grenade.PreviousOwner = owner ?? Server.Host;

            grenade.Spawn();

            grenade.Base.ServerActivate();

            return grenade;
        }

        /// <summary>
        /// Clones current <see cref="Scp2176"/> object.
        /// </summary>
        /// <returns> New <see cref="Scp2176"/> object. </returns>
        public override Item Clone() => new Scp2176()
        {
            FuseTime = FuseTime,
            PinPullTime = PinPullTime,
            Repickable = Repickable,
        };

        /// <summary>
        /// Returns the ExplosiveGrenade in a human readable format.
        /// </summary>
        /// <returns>A string containing ExplosiveGrenade-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}|";
    }
}