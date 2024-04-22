// -----------------------------------------------------------------------
// <copyright file="BlinkingRequestEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp173
{
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;

    using Interfaces;

    using Scp173Role = API.Features.Roles.Scp173Role;

    /// <summary>
    /// Contains all information before server handle SCP-173 blink network message.
    /// </summary>
    public class BlinkingRequestEventArgs : IScp173Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlinkingRequestEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="targets">
        /// <inheritdoc cref="Targets" />
        /// </param>
        public BlinkingRequestEventArgs(Player player, HashSet<ReferenceHub> targets)
        {
            Player = player;
            Scp173 = player.Role.As<Scp173Role>();
            Targets = targets.Select(target => Player.Get(target)).ToList();
        }

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}" /> of players who have triggered SCP-173.
        /// </summary>
        public IReadOnlyCollection<Player> Targets { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is allowed to blink.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets the player who controlling SCP-173.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp173Role Scp173 { get; }
    }
}
