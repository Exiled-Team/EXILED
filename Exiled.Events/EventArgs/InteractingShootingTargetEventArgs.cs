// -----------------------------------------------------------------------
// <copyright file="InteractingShootingTargetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before a player interacts with a shooting target.
    /// </summary>
    public class InteractingShootingTargetEventArgs : EventArgs
    {
        private bool isAllowed;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingShootingTargetEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// /// <param name="shootingTarget"><inheritdoc cref="ShootingTarget"/></param>
        /// /// <param name="targetButton"><inheritdoc cref="TargetButton"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public InteractingShootingTargetEventArgs(Player player, InventorySystem.Items.Firearms.Utilities.ShootingTarget shootingTarget, ShootingTargetButton targetButton, bool isAllowed = true)
        {
            Player = player;
            ShootingTarget = ShootingTarget.Get(shootingTarget);
            TargetButton = targetButton;
            this.isAllowed = isAllowed;
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
        public ShootingTargetButton TargetButton { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the interaction is allowed.
        /// </summary>
        public bool IsAllowed
        {
            get => isAllowed;
            set
            {
                if (!ShootingTarget.IsSynced)
                    throw new InvalidOperationException("Attempted to set IsAllowed while target is in local mode. Set target's IsSynced to true before setting IsAllowed.");
                isAllowed = value;
            }
        }
    }
}
