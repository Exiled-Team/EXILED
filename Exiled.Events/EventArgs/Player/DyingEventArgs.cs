// -----------------------------------------------------------------------
// <copyright file="DyingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;
    using API.Features.DamageHandlers;
    using API.Features.Items;

    using Interfaces;

    using CustomAttackerHandler = API.Features.DamageHandlers.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    ///     Contains all information before a player dies.
    /// </summary>
    public class DyingEventArgs : IAttackerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DyingEventArgs" /> class.
        /// </summary>
        /// <param name="target">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="damageHandler">
        ///     <inheritdoc cref="DamageHandler" />
        /// </param>
        public DyingEventArgs(Player target, DamageHandlerBase damageHandler)
        {
            DamageHandler = new CustomDamageHandler(target, damageHandler);
            Player = target;
#pragma warning disable CS0618
            ItemsToDrop = Player.Items.ToList();
#pragma warning restore CS0618
            Attacker = DamageHandler.BaseIs(out CustomAttackerHandler attackerDamageHandler) ? attackerDamageHandler.Attacker : null;
        }

        /// <summary>
        ///     Gets or sets the list of items to be dropped.
        /// </summary>
        public List<Item> ItemsToDrop
        {
            get;
            [Obsolete("This setter has been deprecated")]
            set;
        }

        /// <summary>
        ///     Gets the dying player.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets the <see cref="CustomDamageHandler" />.
        /// </summary>
        public CustomDamageHandler DamageHandler { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can be killed.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the killing player.
        /// </summary>
        public Player Attacker { get; }
    }
}