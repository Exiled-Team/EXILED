// -----------------------------------------------------------------------
// <copyright file="InteractingScp330EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp330
{
    using API.Features;

    using Interfaces;

    using InventorySystem.Items.Usables.Scp330;

    /// <summary>
    ///     Contains all information before a player interacts with SCP-330.
    /// </summary>
    public class InteractingScp330EventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InteractingScp330EventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="usage">
        ///     <inheritdoc cref="UsageCount" />
        /// </param>
        public InteractingScp330EventArgs(Player player, int usage)
        {
            Player = player;
            Candy = Scp330Candies.GetRandom();
            UsageCount = usage;
            ShouldSever = usage >= 2;
            IsAllowed = Player.IsHuman;
        }

        /// <summary>
        ///     Gets a value indicating how many times this player has interacted with SCP-330.
        /// </summary>
        public int UsageCount { get; }

        /// <summary>
        /// Gets or sets a value indicating the type of candy that will be received from this interaction.
        /// </summary>
        public CandyKindID Candy { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the player's hands should get severed.
        /// </summary>
        public bool ShouldSever { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the player is allowed to interact with SCP-330.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the <see cref="API.Features.Player" /> triggering the event.
        /// </summary>
        public Player Player { get; }
    }
}