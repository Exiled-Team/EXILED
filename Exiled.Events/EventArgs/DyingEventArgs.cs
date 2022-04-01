// -----------------------------------------------------------------------
// <copyright file="DyingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
#pragma warning disable CS0618
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using PlayerStatsSystem;

    /// <summary>
    /// Contains all information before a player dies.
    /// </summary>
    public class DyingEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="DyingEventArgs"/> class.
        /// </summary>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="damageHandler"><inheritdoc cref="DamageHandler"/></param>
        public DyingEventArgs(Player target, DamageHandlerBase damageHandler) {
            ItemsToDrop = new List<Item>(target.Items.ToList());
            Killer = damageHandler is AttackerDamageHandler attackerDamageHandler ? Player.Get(attackerDamageHandler.Attacker.Hub) : null;
            Target = target;
            Handler = new DamageHandler(target, damageHandler);
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
        /// Gets or sets the damage handler for the event.
        /// </summary>
        [Obsolete("Use DyingEventArgs.Handler", true)]
        public DamageHandlerBase DamageHandler { get => Handler.Base; set => Handler.Base = value; }

        /// <summary>
        /// Gets or sets the <see cref="API.Features.DamageHandler"/>.
        /// </summary>
        public DamageHandler Handler { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can be killed.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
