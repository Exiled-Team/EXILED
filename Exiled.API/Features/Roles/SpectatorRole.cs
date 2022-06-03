// -----------------------------------------------------------------------
// <copyright file="SpectatorRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System;

    /// <summary>
    /// Defines a role that represents a human class.
    /// </summary>
    public class SpectatorRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpectatorRole"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal SpectatorRole(Player player)
        {
            Owner = player;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the player died.
        /// </summary>
        public DateTime DeathTime => new(Owner.ReferenceHub.characterClassManager.DeathTime);

        /// <summary>
        /// Gets the total amount of time the player has been dead.
        /// </summary>
        public TimeSpan DeadTime => DateTime.UtcNow - DeathTime;

        /// <summary>
        /// Gets or sets currently spectated player by this <see cref="Player"/>. May be <see langword="null"/>.
        /// </summary>
        public Player SpectatedPlayer
        {
            get
            {
                Player spectatedPlayer = Player.Get(Owner.ReferenceHub.spectatorManager.CurrentSpectatedPlayer);
                return spectatedPlayer != Owner ? spectatedPlayer : null;
            }

            [Obsolete("Client side feature.", true)]
            set
            {
                if (Owner.IsAlive)
                    throw new InvalidOperationException("The spectated player cannot be set on an alive player.");

                Owner.ReferenceHub.spectatorManager.CurrentSpectatedPlayer = value.ReferenceHub;
                Owner.ReferenceHub.spectatorManager.CmdSendPlayer(value.Id);
            }
        }

        /// <inheritdoc/>
        internal override RoleType RoleType => RoleType.Spectator;
    }
}
