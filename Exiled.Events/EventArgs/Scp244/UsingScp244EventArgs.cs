// -----------------------------------------------------------------------
// <copyright file="UsingScp244EventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp244
{
    using API.Features;
    using API.Features.Items;

    using Interfaces;

    using InventorySystem.Items.Usables.Scp244;

    /// <summary>
    /// Contains all information before SCP-244 is used.
    /// </summary>
    public class UsingScp244EventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingScp244EventArgs" /> class.
        /// </summary>
        /// <param name="scp244">
        /// <inheritdoc cref="Scp244" />
        /// </param>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public UsingScp244EventArgs(Scp244Item scp244, Player player, bool isAllowed = true)
        {
            Scp244 = (Scp244)Item.Get(scp244);
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the Scp244 instance.
        /// </summary>
        public Scp244 Scp244 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the SCP-244 can be used.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the player who's using the SCP-244.
        /// </summary>
        public Player Player { get; }
    }
}
