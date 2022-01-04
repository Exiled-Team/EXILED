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
        [System.Obsolete("Please use new ExplosiveGrenade(GrenadeType, Player) instead. This constructor will be removed soon.")]
        public ExplosiveGrenade(ItemType type, Player player = null)
            : this(player == null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(type, false) : (ThrowableItem)player.Inventory.CreateItemInstance(type, true))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class, as well as an explosive grenade item. This class should only be used for explosive grenades (Frag grenade and SCP-018). For the flash grenade, see the <see cref="FlashGrenade"/> class.
        /// </summary>
        /// <param name="type">The type of grenade. Should be either <see cref="GrenadeType.FragGrenade"/> or <see cref="GrenadeType.Scp018"/>; for flash grenades, see the <see cref="FlashGrenade"/> class.</param>
        /// <param name="player">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        public ExplosiveGrenade(GrenadeType type, Player player = null)
            : this(player == null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(type == GrenadeType.Scp018 ? ItemType.SCP018 : ItemType.GrenadeHE, false) : (ThrowableItem)player.Inventory.CreateItemInstance(type == GrenadeType.Scp018 ? ItemType.SCP018 : ItemType.GrenadeHE, true))
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
        internal static Dictionary<ExplosionGrenade, ExplosiveGrenade> GrenadeToItem { get; set; } = new Dictionary<ExplosionGrenade, ExplosiveGrenade>();

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
            grenade.PreviousOwner = new Footprint(owner != null ? owner.ReferenceHub : Server.Host.ReferenceHub);
            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}|";
        }
    }
}
