// -----------------------------------------------------------------------
// <copyright file="DyingEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SEXiled.API.Features;
    using SEXiled.API.Features.DamageHandlers;
    using SEXiled.API.Features.Items;

    using CustomAttackerHandler = SEXiled.API.Features.DamageHandlers.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// Contains all information before a player dies.
    /// </summary>
    public class DyingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DyingEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="damageHandler"><inheritdoc cref="DamageHandler"/></param>
        public DyingEventArgs(Player target, DamageHandlerBase damageHandler)
        {
            Handler = new CustomDamageHandler(target, damageHandler);
            ItemsToDrop = new List<Item>(target.Items.ToList());
            Killer = Handler.BaseIs(out CustomAttackerHandler attackerDamageHandler) ? attackerDamageHandler.Attacker : null;
            Target = target;
        }

        /// <summary>
        /// Gets the killing player.
        /// </summary>
        public Player Killer { get; }

        /// <summary>
        /// Gets the dying player.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets the list of items to be dropped.
        /// </summary>
        public List<Item> ItemsToDrop { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CustomDamageHandler"/>.
        /// </summary>
        public CustomDamageHandler Handler { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can be killed.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
