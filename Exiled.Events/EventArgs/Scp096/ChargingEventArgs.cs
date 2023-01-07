// -----------------------------------------------------------------------
// <copyright file="ChargingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using API.Features;

    using Interfaces;

    using PlayerRoles.PlayableScps.Scp096;

    /// <summary>
    ///     Contains all information before SCP-096 charges.
    /// </summary>
    public class ChargingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChargingEventArgs" /> class.
        /// </summary>
        /// <param name="scp096">
        ///     <inheritdoc cref="Scp096" />
        /// </param>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public ChargingEventArgs(Scp096Role scp096, Player player, bool isAllowed = true)
        {
            Scp096 = scp096;
            Player = player;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the SCP-096 instance.
        /// </summary>
        public Scp096Role Scp096 { get; }

        /// <summary>
        ///     Gets the player who is controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-096 can charge.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}