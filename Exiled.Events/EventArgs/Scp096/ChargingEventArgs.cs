// -----------------------------------------------------------------------
// <copyright file="ChargingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    /// Contains all information before SCP-096 charges.
    /// </summary>
    public class ChargingEventArgs : IScp096Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChargingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChargingEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            Scp096 = player.Role.As<Scp096Role>();
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Scp096Role Scp096 { get; }

        /// <summary>
        /// Gets the player who is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-096 can charge.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}