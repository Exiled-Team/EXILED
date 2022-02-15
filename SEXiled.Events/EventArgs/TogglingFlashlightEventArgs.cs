// -----------------------------------------------------------------------
// <copyright file="TogglingFlashlightEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;
    using SEXiled.API.Features.Items;

    using InventorySystem.Items.Flashlight;

    /// <summary>
    /// Contains all information before a player toggles the flashlight.
    /// </summary>
    public class TogglingFlashlightEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TogglingFlashlightEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="flashlight"><inheritdoc cref="Flashlight"/></param>
        /// <param name="newState"><inheritdoc cref="NewState"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public TogglingFlashlightEventArgs(Player player, FlashlightItem flashlight, bool newState, bool isAllowed = true)
        {
            Player = player;
            Flashlight = (Flashlight)Item.Get(flashlight);
            NewState = newState;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's toggling the flashlight.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Items.Flashlight"/> being toggled.
        /// </summary>
        public Flashlight Flashlight { get; }

#pragma warning disable SA1623 // Property summary documentation should match accessors
        /// <summary>
        /// Gets or sets the new flashlight state.
        /// </summary>
        public bool NewState { get; set; }
#pragma warning restore SA1623 // Property summary documentation should match accessors

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can toggle the flashlight.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
