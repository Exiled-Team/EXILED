// -----------------------------------------------------------------------
// <copyright file="ExplosiveGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Enums;

    using Footprinting;

    using InventorySystem.Items.ThrowableProjectiles;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="ExplosionGrenade"/>.
    /// </summary>
    public class ExplosiveGrenade : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Throwable.Base"/></param>
        public ExplosiveGrenade(ThrowableItem itemBase)
            : base(itemBase)
        {
            Projectile = (ExplosionGrenade)Object.Instantiate(itemBase.Projectile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Throwable.Base"/></param>
        /// <param name="player"><inheritdoc cref="Item.Owner"/></param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        public ExplosiveGrenade(ItemType type, Player player = null)
            : this(player == null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(type, false) : (ThrowableItem)player.Inventory.CreateItemInstance(type, true))
        {
        }

        /// <summary>
        /// Gets the <see cref="ExplosionGrenade"/> for this item.
        /// </summary>
        public new ExplosionGrenade Projectile { get; }

        /// <summary>
        /// Gets or sets the maximum radius of the grenade.
        /// </summary>
        public float MaxRadius
        {
            get => Projectile._maxRadius;
            set => Projectile._maxRadius = value;
        }

        /// <summary>
        /// Gets or sets the multiplier for damage against <see cref="Side.Scp"/> players.
        /// </summary>
        public float ScpMultiplier
        {
            get => Projectile._scpDamageMultiplier;
            set => Projectile._scpDamageMultiplier = value;
        }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Burned"/> effect will last.
        /// </summary>
        public float BurnDuration
        {
            get => Projectile._burnedDuration;
            set => Projectile._burnedDuration = value;
        }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Deafened"/> effect will last.
        /// </summary>
        public float DeafenDuration
        {
            get => Projectile._deafenedDuration;
            set => Projectile._deafenedDuration = value;
        }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Concussed"/> effect will last.
        /// </summary>
        public float ConcussDuration
        {
            get => Projectile._concussedDuration;
            set => Projectile._concussedDuration = value;
        }

        /// <summary>
        /// Gets or sets how long the fuse will last.
        /// </summary>
        public float FuseTime
        {
            get => Projectile._fuseTime;
            set => Projectile._fuseTime = value;
        }

        /// <summary>
        /// Spawns an active grenade on the map at the specified location.
        /// </summary>
        /// <param name="position">The location to spawn the grenade.</param>
        /// <param name="owner">Optional: The <see cref="Player"/> owner of the grenade.</param>
        public void SpawnActive(Vector3 position, Player owner = null)
        {
            if (owner != null)
                Projectile.PreviousOwner = new Footprint(owner.ReferenceHub);

#if DEBUG
            Log.Debug($"Spawning active grenade: {FuseTime}");
#endif
            Projectile.transform.position = position;
            NetworkServer.Spawn(Projectile.gameObject);
            Projectile.RpcSetTime(FuseTime);
        }
    }
}
