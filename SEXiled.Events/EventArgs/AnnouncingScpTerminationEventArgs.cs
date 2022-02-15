// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTerminationEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;

    using SEXiled.API.Features;
    using SEXiled.API.Features.DamageHandlers;

    using AttackerDamageHandler = PlayerStatsSystem.AttackerDamageHandler;
    using CustomAttackerHandler = SEXiled.API.Features.DamageHandlers.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// Contains all informations before C.A.S.S.I.E announces an SCP termination.
    /// </summary>
    public class AnnouncingScpTerminationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingScpTerminationEventArgs"/> class.
        /// </summary>
        /// <param name="scp"><inheritdoc cref="Player"/></param>
        /// <param name="damageHandlerBase"><inheritdoc cref="DamageHandler"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AnnouncingScpTerminationEventArgs(Player scp, DamageHandlerBase damageHandlerBase, bool isAllowed = true)
        {
            Player = scp;
            Role = scp.ReferenceHub.characterClassManager.CurRole;
            Handler = new CustomDamageHandler(scp, damageHandlerBase);
            Killer = Handler.BaseIs(out CustomAttackerHandler customAttackerHandler) ? customAttackerHandler.Attacker : null;
            TerminationCause = damageHandlerBase.CassieDeathAnnouncement.Announcement;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player the announcement is being played for.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player who killed the SCP.
        /// </summary>
        public Player Killer { get; }

        /// <summary>
        /// Gets the killed <see cref="Role"/>.
        /// </summary>
        public Role Role { get; }

        /// <summary>
        /// Gets or sets the <see cref="CustomDamageHandler"/>.
        /// </summary>
        public CustomDamageHandler Handler { get; set; }

        /// <summary>
        /// Gets or sets the termination cause.
        /// </summary>
        public string TerminationCause { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the SCP termination will be announced by C.A.S.S.I.E.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
