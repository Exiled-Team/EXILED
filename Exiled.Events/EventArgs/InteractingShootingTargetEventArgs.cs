// -----------------------------------------------------------------------
// <copyright file="InteractingShootingTargetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.Firearms.Utilities;
    using InventorySystem.Items.Pickups;

    /// <summary>
    /// Contains all information before a player interacts with a shooting target.
    /// </summary>
    public class InteractingShootingTargetEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingShootingTargetEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// /// <param name="shootingTarget"><inheritdoc cref="ShootingTarget"/></param>
        /// /// <param name="targetButton"><inheritdoc cref="TargetButton"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InteractingShootingTargetEventArgs(Player player, ShootingTarget shootingTarget, ShootingTarget.TargetButton targetButton, bool isAllowed = false)
        {
            Player = player;
            ShootingTarget = shootingTarget;
            TargetButton = targetButton;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player interacting with the shooting target.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the shooting target being interacted with.
        /// </summary>
        public ShootingTarget ShootingTarget { get; }

        /// <summary>
        /// Gets the button the player interacted with.
        /// </summary>
        public ShootingTarget.TargetButton TargetButton { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the interaction is allowed.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
