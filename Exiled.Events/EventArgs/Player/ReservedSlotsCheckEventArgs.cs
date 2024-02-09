// -----------------------------------------------------------------------
// <copyright file="ReservedSlotsCheckEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Enums;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information when checking if a player has a reserved slot.
    /// </summary>
    public class ReservedSlotsCheckEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReservedSlotsCheckEventArgs" /> class.
        /// </summary>
        /// <param name="userId">
        /// <inheritdoc cref="UserId" />
        /// </param>
        /// <param name="hasReservedSlot">
        /// <inheritdoc cref="HasReservedSlot" />
        /// </param>
        public ReservedSlotsCheckEventArgs(string userId, bool hasReservedSlot)
        {
            UserId = userId;
            HasReservedSlot = hasReservedSlot;
        }

        /// <summary>
        /// Gets the UserID of the player that is being checked.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Gets a value indicating whether the player has a reserved slot in the base game system.
        /// </summary>
        public bool HasReservedSlot { get; }

        /// <summary>
        /// Gets or sets the event result.
        /// </summary>
        public ReservedSlotEventResult Result { get; set; } = ReservedSlotEventResult.UseBaseGameSystem;
    }
}