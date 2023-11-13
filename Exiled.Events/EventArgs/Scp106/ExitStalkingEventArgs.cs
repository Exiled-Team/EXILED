// -----------------------------------------------------------------------
// <copyright file="ExitStalkingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp106
{
    using System;

    using API.Features;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp106;

    using Scp106Role = API.Features.Roles.Scp106Role;

    /// <summary>
    ///     Contains all information before SCP-106 use the stalk ability.
    /// </summary>
    public class ExitStalkingEventArgs : IScp106Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitStalkingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ExitStalkingEventArgs(Player player, bool isAllowed = true)
        {
            Player = player;
            Scp106 = player.Role.As<Scp106Role>();
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who's controlling SCP-106.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp106Role Scp106 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not SCP-106 can stalk.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}