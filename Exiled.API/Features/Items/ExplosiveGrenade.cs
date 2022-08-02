// -----------------------------------------------------------------------
// <copyright file="ExplosiveGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System.Collections.Generic;

    using Exiled.API.Enums;

    using Footprinting;

    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for <see cref="ExplosionGrenade"/>.
    /// </summary>
    public class ExplosiveGrenade : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public ExplosiveGrenade(ThrowableItem itemBase)
            : base(itemBase)
        {
            ExplosionGrenade grenade = (ExplosionGrenade)Base.Projectile;
            MaxRadius = grenade._maxRadius;
            ScpMultiplier = grenade._scpDamageMultiplier;
            BurnDuration = grenade._burnedDuration;
            DeafenDuration = grenade._deafenedDuration;
            ConcussDuration = grenade._concussedDuration;
            FuseTime = grenade._fuseTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the grenade.</param>
        /// <param name="player">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        internal ExplosiveGrenade(ItemType type, Player player = null)
            : this(player is null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(type, false) : (ThrowableItem)player.Inventory.CreateItemInstance(type, true))
        {
        }

        /// <summary>
        /// Gets or sets the maximum radius of the grenade.
        /// </summary>
        public float MaxRadius { get; set; }

        /// <summary>
        /// Gets or sets the multiplier for damage against <see cref="Side.Scp"/> players.
        /// </summary>
        public float ScpMultiplier { get; set; }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Burned"/> effect will last.
        /// </summary>
        public float BurnDuration { get; set; }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Deafened"/> effect will last.
        /// </summary>
        public float DeafenDuration { get; set; }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Concussed"/> effect will last.
        /// </summary>
        public float ConcussDuration { get; set; }

        /// <summary>
        /// Gets or sets how long the fuse will last.
        /// </summary>
        public float FuseTime { get; set; }

        /// <summary>
        /// Gets or sets all the currently known <see cref="EffectGrenade"/>:<see cref="Throwable"/> items.
        /// </summary>
        internal static Dictionary<ExplosionGrenade, ExplosiveGrenade> GrenadeToItem { get; set; } = new();

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
            ExplosionGrenade grenade = (ExplosionGrenade)Object.Instantiate(Base.Projectile, position, Quaternion.identity);
            grenade._maxRadius = MaxRadius;
            grenade._scpDamageMultiplier = ScpMultiplier;
            grenade._burnedDuration = BurnDuration;
            grenade._deafenedDuration = DeafenDuration;
            grenade._concussedDuration = ConcussDuration;
            grenade._fuseTime = FuseTime;
            grenade.PreviousOwner = new Footprint((owner ?? Server.Host).ReferenceHub);
            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
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
        public override Item Clone()
        {
            ExplosiveGrenade cloneableItem = new(Type)
            {
                MaxRadius = MaxRadius,
                ScpMultiplier = ScpMultiplier,
                BurnDuration = BurnDuration,
                DeafenDuration = DeafenDuration,
                ConcussDuration = ConcussDuration,
                FuseTime = FuseTime,
            };

            return cloneableItem;
        }
    }
}
