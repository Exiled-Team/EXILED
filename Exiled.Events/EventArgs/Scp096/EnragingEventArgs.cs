// -----------------------------------------------------------------------
// <copyright file="EnragingEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before SCP-096 gets enraged.
    /// </summary>
    public class EnragingEventArgs : IScp096Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnragingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="initialDuration">
        /// <inheritdoc cref="InitialDuration" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public EnragingEventArgs(Player player, float initialDuration, bool isAllowed = true)
        {
            Player = player;
            Scp096 = player.Role.As<Scp096Role>();
            InitialDuration = initialDuration;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Scp096Role Scp096 { get; }

        /// <summary>
        /// Gets the player who's controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the SCP-096 rage initial duration.
        /// </summary>
        public float InitialDuration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-096 can be enraged.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}