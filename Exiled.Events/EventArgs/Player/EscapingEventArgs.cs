// -----------------------------------------------------------------------
// <copyright file="EscapingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before a player escapes.
    /// </summary>
    public class EscapingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EscapingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        public EscapingEventArgs(Player player)
        {
            Player = player;
            if (player.IsCuffed)
            {
                switch (player.Role.Team)
                {
                    case Team.CDP:
                        NewRole = RoleType.NtfPrivate;
                        break;
                    case Team.RSC:
                        NewRole = RoleType.ChaosConscript;
                        break;
                }
            }
            else
            {
                switch (player.Role.Team)
                {
                    case Team.CDP:
                        NewRole = RoleType.ChaosConscript;
                        break;
                    case Team.RSC:
                        NewRole = RoleType.NtfSpecialist;
                        break;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the role that will be assigned when the player escapes.
        /// </summary>
        public RoleType NewRole { get; set; } = RoleType.Spectator;

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can escape.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the player who's escaping.
        /// </summary>
        public Player Player { get; }
    }
}