// -----------------------------------------------------------------------
// <copyright file="InteractingShootingTargetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;

    using AdminToys;

    using API.Enums;
    using API.Features;
    using API.Features.Toys;

    using Interfaces;

    using UnityEngine;

    /// <summary>
    /// Contains all information before a player interacts with a shooting target.
    /// </summary>
    public class InteractingShootingTargetEventArgs : IPlayerEvent, IDeniableEvent
    {
        private int autoResetTime;
        private int maxHp;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteractingShootingTargetEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="shootingTarget">
        /// <inheritdoc cref="ShootingTarget" />
        /// </param>
        /// <param name="targetButton">
        /// <inheritdoc cref="TargetButton" />
        /// </param>
        /// <param name="maxHp">
        /// <inheritdoc cref="NewMaxHp" />
        /// </param>
        /// <param name="autoResetTime">
        /// <inheritdoc cref="NewAutoResetTime" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public InteractingShootingTargetEventArgs(Player player, ShootingTarget shootingTarget, ShootingTargetButton targetButton, int maxHp, int autoResetTime, bool isAllowed = true)
        {
            Player = player;
            ShootingTarget = ShootingTargetToy.Get(shootingTarget);
            TargetButton = targetButton;
            IsAllowed = isAllowed;
            this.maxHp = maxHp;
            this.autoResetTime = autoResetTime;
        }

        /// <summary>
        /// Gets the shooting target being interacted with.
        /// </summary>
        public ShootingTargetToy ShootingTarget { get; }

        /// <summary>
        /// Gets the button the player interacted with.
        /// </summary>
        public ShootingTargetButton TargetButton { get; }

        /// <summary>
        /// Gets or sets the new max HP of the target.
        /// </summary>
        public int NewMaxHp
        {
            get => maxHp;
            set
            {
                if (!ShootingTarget.IsSynced)
                    throw new InvalidOperationException("Attempted to set MaxHp while target is in local mode. Set target's IsSynced to true before setting IsAllowed.");
                maxHp = Mathf.Clamp(value, 1, 256);
            }
        }

        /// <summary>
        /// Gets or sets the new auto reset time of the target.
        /// </summary>
        public int NewAutoResetTime
        {
            get => autoResetTime;
            set
            {
                if (!ShootingTarget.IsSynced)
                    throw new InvalidOperationException("Attempted to set AutoResetTime while target is in local mode. Set target's IsSynced to true before setting IsAllowed.");
                autoResetTime = Mathf.Clamp(value, 0, 10);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the interaction is allowed.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player interacting with the shooting target.
        /// </summary>
        public Player Player { get; }
    }
}