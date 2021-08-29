// -----------------------------------------------------------------------
// <copyright file="FlashGrenade.cs" company="Exiled Team">
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
    /// A wrapper class for <see cref="FlashbangGrenade"/>.
    /// </summary>
    public class FlashGrenade : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlashGrenade"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Throwable.Base"/></param>
        public FlashGrenade(ThrowableItem itemBase)
            : base(itemBase)
        {
            Projectile = (FlashbangGrenade)Object.Instantiate(itemBase.Projectile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlashGrenade"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Throwable.Base"/></param>
        /// <param name="player"><inheritdoc cref="Item.Owner"/></param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        public FlashGrenade(ItemType type, Player player = null)
            : this(player == null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(type, false) : (ThrowableItem)player.Inventory.CreateItemInstance(type, true))
        {
        }

        /// <summary>
        /// Gets the <see cref="ExplosionGrenade"/> for this item.
        /// </summary>
        public new FlashbangGrenade Projectile { get; }

        /// <summary>
        /// Gets or sets the <see cref="AnimationCurve"/> for determining how long the <see cref="EffectType.Blinded"/> effect will last.
        /// </summary>
        public AnimationCurve BlindCurve
        {
            get => Projectile._blindingOverDistance;
            set => Projectile._blindingOverDistance = value;
        }

        /// <summary>
        /// Gets or sets the multiplier for damage against <see cref="Side.Scp"/> players.
        /// </summary>
        public float SurfaceDistanceIntensifier
        {
            get => Projectile._surfaceZoneDistanceIntensifier;
            set => Projectile._surfaceZoneDistanceIntensifier = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="AnimationCurve"/> for determining how long the <see cref="EffectType.Deafened"/> effect will last.
        /// </summary>
        public AnimationCurve DeafenCurve
        {
            get => Projectile._deafenDurationOverDistance;
            set => Projectile._deafenDurationOverDistance = value;
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
