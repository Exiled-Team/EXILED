// -----------------------------------------------------------------------
// <copyright file="ExplosiveGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items {
    using System.Collections.Generic;

    using Exiled.API.Enums;

    using Footprinting;

    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="ExplosionGrenade"/>.
    /// </summary>
    public class ExplosiveGrenade : Throwable {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Throwable.Base"/></param>
        public ExplosiveGrenade(ThrowableItem itemBase)
            : base(itemBase) {
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
        /// <param name="type"><inheritdoc cref="Throwable.Base"/></param>
        /// <param name="player"><inheritdoc cref="Item.Owner"/></param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        public ExplosiveGrenade(ItemType type, Player player = null)
            : this(player == null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(type, false) : (ThrowableItem)player.Inventory.CreateItemInstance(type, true)) {
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
        internal static Dictionary<ExplosionGrenade, ExplosiveGrenade> GrenadeToItem { get; set; } = new Dictionary<ExplosionGrenade, ExplosiveGrenade>();

        /// <summary>
        /// Spawns an active grenade on the map at the specified location.
        /// </summary>
        /// <param name="position">The location to spawn the grenade.</param>
        /// <param name="owner">Optional: The <see cref="Player"/> owner of the grenade.</param>
        public void SpawnActive(Vector3 position, Player owner = null) {
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
            grenade.PreviousOwner = new Footprint(owner != null ? owner.ReferenceHub : Server.Host.ReferenceHub);
            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
        }

        /// <inheritdoc/>
        public override string ToString() {
            return $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}|";
        }
    }
}
