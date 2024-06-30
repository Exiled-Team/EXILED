// -----------------------------------------------------------------------
// <copyright file="ExplosiveGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System.Diagnostics;

    using Enums;

    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pickups.Projectiles;

    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;
    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for <see cref="ExplosionGrenade"/>.
    /// </summary>
    [DebuggerDisplay("ExplosiveGrenade")]
    public class ExplosiveGrenade : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public ExplosiveGrenade(ThrowableItem itemBase)
            : base(itemBase)
        {
            Projectile = (ExplosionGrenadeProjectile)((Throwable)this).Projectile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the grenade.</param>
        /// <param name="player">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        internal ExplosiveGrenade(ItemType type, Player player = null)
            : this((ThrowableItem)(player ?? Server.Host).Inventory.CreateItemInstance(new(type, 0), true))
        {
        }

        /// <summary>
        /// Gets a <see cref="ExplosionGrenadeProjectile"/> to change grenade properties.
        /// </summary>
        public new ExplosionGrenadeProjectile Projectile { get; }

        /// <summary>
        /// Gets or sets the maximum radius of the grenade.
        /// </summary>
        public float MaxRadius
        {
            get => Projectile.MaxRadius;
            set => Projectile.MaxRadius = value;
        }

        /// <summary>
        /// Gets or sets the multiplier for damage against <see cref="Side.Scp"/> players.
        /// </summary>
        public float ScpDamageMultiplier
        {
            get => Projectile.ScpDamageMultiplier;
            set => Projectile.ScpDamageMultiplier = value;
        }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Burned"/> effect will last.
        /// </summary>
        public float BurnDuration
        {
            get => Projectile.BurnDuration;
            set => Projectile.BurnDuration = value;
        }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Deafened"/> effect will last.
        /// </summary>
        public float DeafenDuration
        {
            get => Projectile.DeafenDuration;
            set => Projectile.DeafenDuration = value;
        }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Concussed"/> effect will last.
        /// </summary>
        public float ConcussDuration
        {
            get => Projectile.ConcussDuration;
            set => Projectile.ConcussDuration = value;
        }

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
        /// <returns>Spawned <see cref="ExplosionGrenadeProjectile">grenade</see>.</returns>
        public ExplosionGrenadeProjectile SpawnActive(Vector3 position, Player owner = null)
        {
#if DEBUG
            Log.Debug($"Spawning active grenade: {FuseTime}");
#endif
            ItemPickupBase ipb = Object.Instantiate(Projectile.Base, position, Quaternion.identity);

            ipb.Info = new PickupSyncInfo(Type, Weight, ItemSerialGenerator.GenerateNext());

            ExplosionGrenadeProjectile grenade = (ExplosionGrenadeProjectile)Pickup.Get(ipb);

            grenade.Base.gameObject.SetActive(true);

            grenade.MaxRadius = MaxRadius;
            grenade.ScpDamageMultiplier = ScpDamageMultiplier;
            grenade.BurnDuration = BurnDuration;
            grenade.DeafenDuration = DeafenDuration;
            grenade.ConcussDuration = ConcussDuration;
            grenade.FuseTime = FuseTime;

            grenade.PreviousOwner = owner ?? Server.Host;

            grenade.Spawn();

            grenade.Base.ServerActivate();

            return grenade;
        }

        /// <summary>
        /// Returns the ExplosiveGrenade in a human readable format.
        /// </summary>
        /// <returns>A string containing ExplosiveGrenade-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}|";

        /// <summary>
        /// Clones current <see cref="ExplosiveGrenade"/> object.
        /// </summary>
        /// <returns> New <see cref="ExplosiveGrenade"/> object. </returns>
        public override Item Clone() => new ExplosiveGrenade(Type)
        {
            MaxRadius = MaxRadius,
            ScpDamageMultiplier = ScpDamageMultiplier,
            BurnDuration = BurnDuration,
            DeafenDuration = DeafenDuration,
            ConcussDuration = ConcussDuration,
            FuseTime = FuseTime,
            PinPullTime = PinPullTime,
            Repickable = Repickable,
        };

        /// <inheritdoc/>
        internal override void ReadPickupInfo(Pickup pickup)
        {
            base.ReadPickupInfo(pickup);
            if (pickup is ExplosiveGrenadePickup explosiveGrenadePickup)
            {
                MaxRadius = explosiveGrenadePickup.MaxRadius;
                ScpDamageMultiplier = explosiveGrenadePickup.ScpDamageMultiplier;
                BurnDuration = explosiveGrenadePickup.BurnDuration;
                DeafenDuration = explosiveGrenadePickup.DeafenDuration;
                ConcussDuration = explosiveGrenadePickup.ConcussDuration;
                FuseTime = explosiveGrenadePickup.FuseTime;
            }
        }
    }
}