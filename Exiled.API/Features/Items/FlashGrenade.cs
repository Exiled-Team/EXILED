// -----------------------------------------------------------------------
// <copyright file="FlashGrenade.cs" company="Exiled Team">
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

    using MEC;

    using Mirror;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A wrapper class for <see cref="FlashbangGrenade"/>.
    /// </summary>
    public class FlashGrenade : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlashGrenade"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="ThrowableItem"/> class.</param>
        public FlashGrenade(ThrowableItem itemBase)
            : base(itemBase)
        {
            FlashbangGrenade grenade = (FlashbangGrenade)Base.Projectile;
            BlindCurve = grenade._blindingOverDistance;
            SurfaceDistanceIntensifier = grenade._surfaceZoneDistanceIntensifier;
            DeafenCurve = grenade._deafenDurationOverDistance;
            FuseTime = grenade._fuseTime;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlashGrenade"/> class, as well as a new flash grenade item.
        /// </summary>
        /// <param name="player">The owner of the grenade. Leave <see langword="null"/> for no owner.</param>
        /// <remarks>The player parameter will always need to be defined if this grenade is custom using Exiled.CustomItems.</remarks>
        internal FlashGrenade(Player player = null)
            : this(player is null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(ItemType.GrenadeFlash, false) : (ThrowableItem)player.Inventory.CreateItemInstance(ItemType.GrenadeFlash, true))
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="AnimationCurve"/> for determining how long the <see cref="EffectType.Blinded"/> effect will last.
        /// </summary>
        public AnimationCurve BlindCurve { get; set; }

        /// <summary>
        /// Gets or sets the multiplier for damage against <see cref="Side.Scp"/> players.
        /// </summary>
        public float SurfaceDistanceIntensifier { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AnimationCurve"/> for determining how long the <see cref="EffectType.Deafened"/> effect will last.
        /// </summary>
        public AnimationCurve DeafenCurve { get; set; }

        /// <summary>
        /// Gets or sets how long the fuse will last.
        /// </summary>
        public float FuseTime { get; set; }

        /// <summary>
        /// Gets or sets all the currently known <see cref="EffectGrenade"/>:<see cref="Throwable"/> items.
        /// </summary>
        internal static Dictionary<FlashbangGrenade, FlashGrenade> GrenadeToItem { get; set; } = new();

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
            FlashbangGrenade grenade = (FlashbangGrenade)Object.Instantiate(Base.Projectile, position, Quaternion.identity);
            grenade.PreviousOwner = new Footprint(owner is not null ? owner.ReferenceHub : Server.Host.ReferenceHub);
            grenade._blindingOverDistance = BlindCurve;
            grenade._surfaceZoneDistanceIntensifier = SurfaceDistanceIntensifier;
            grenade._deafenDurationOverDistance = DeafenCurve;
            grenade._fuseTime = FuseTime;
            NetworkServer.Spawn(grenade.gameObject);
            grenade.ServerActivate();
        }

        /// <summary>
        /// Returns the FlashGrenade in a human readable format.
        /// </summary>
        /// <returns>A string containing FlashGrenade-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}|";

        /// <summary>
        /// Clones current <see cref="FlashGrenade"/> object.
        /// </summary>
        /// <returns> New <see cref="FlashGrenade"/> object. </returns>
        public override Item Clone()
        {
            FlashGrenade cloneableItem = new();

            cloneableItem.BlindCurve = BlindCurve;
            cloneableItem.SurfaceDistanceIntensifier = SurfaceDistanceIntensifier;
            cloneableItem.DeafenCurve = DeafenCurve;
            cloneableItem.FuseTime = FuseTime;

            return cloneableItem;
        }
    }
}
