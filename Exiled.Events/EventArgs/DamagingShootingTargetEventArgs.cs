// -----------------------------------------------------------------------
// <copyright file="DamagingShootingTargetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using InventorySystem.Items;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a player damages a shooting target.
    /// </summary>
    public class DamagingShootingTargetEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DamagingShootingTargetEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="shootingTarget"><inheritdoc cref="ShootingTarget"/></param>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="attackerFootprint"><inheritdoc cref="AttackerFootprint"/></param>
        /// <param name="hitLocation"><inheritdoc cref="HitLocation"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DamagingShootingTargetEventArgs(Player player, InventorySystem.Items.Firearms.Utilities.ShootingTarget shootingTarget, IDamageDealer item, Footprinting.Footprint attackerFootprint, Vector3 hitLocation, bool isAllowed = true)
        {
            Player = player;
            ShootingTarget = ShootingTarget.Get(shootingTarget);
            Item = API.Features.Items.Item.Get(item as ItemBase);
            AttackerFootprint = attackerFootprint;
            HitLocation = hitLocation;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player damaging the shooting target.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the shooting target being damaged.
        /// </summary>
        public ShootingTarget ShootingTarget { get; }

        /// <summary>
        /// Gets the item dealing the damage.
        /// </summary>
        public API.Features.Items.Item Item { get; }

        /// <summary>
        /// Gets the attacker's footprint.
        /// </summary>
        public Footprinting.Footprint AttackerFootprint { get; }

        /// <summary>
        /// Gets the exact world location the bullet impacted the target.
        /// </summary>
        public Vector3 HitLocation { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the damage is allowed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
