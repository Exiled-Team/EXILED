// -----------------------------------------------------------------------
// <copyright file="EscapingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs {
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all information before a player escapes.
    /// </summary>
    public class EscapingEventArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapingEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        public EscapingEventArgs(Player player) {
            Player = player;
            if (player.IsCuffed) {
                switch (player.Team) {
                    case Team.CDP:
                        NewRole = RoleType.NtfPrivate;
                        break;
                    case Team.RSC:
                        NewRole = RoleType.ChaosConscript;
                        break;
                }
            }
            else {
                switch (player.Team) {
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
        /// Gets the player who's escaping.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the role that will be assigned when the player escapes.
        /// </summary>
        public RoleType NewRole { get; set; } = RoleType.Spectator;

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can escape.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
