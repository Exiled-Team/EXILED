// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTerminationEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    using PlayerStatsSystem;

    /// <summary>
    /// Contains all informations before C.A.S.S.I.E announces an SCP termination.
    /// </summary>
    public class AnnouncingScpTerminationEventArgs : EventArgs
    {
        private string terminationCause;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingScpTerminationEventArgs"/> class.
        /// </summary>
        /// <param name="scp"><inheritdoc cref="Player"/></param>
        /// <param name="damageHandlerBase"><inheritdoc cref="DamageHandler"/></param>
        public AnnouncingScpTerminationEventArgs(Player scp, DamageHandlerBase damageHandlerBase)
        {
            Player = scp;
            Role = scp.ReferenceHub.characterClassManager.CurRole;
            DamageHandler = damageHandlerBase;
            Killer = damageHandlerBase is AttackerDamageHandler attackerDamageHandler ? API.Features.Player.Get(attackerDamageHandler.Attacker.Hub) : null;
            terminationCause = damageHandlerBase.CassieDeathAnnouncement;
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
        /// Gets or sets the <see cref="DamageHandlerBase"/>.
        /// </summary>
        public DamageHandlerBase DamageHandler { get; set; }

        /// <summary>
        /// Gets or sets the termination cause.
        /// </summary>
        public string TerminationCause
        {
            get => terminationCause;
            set => terminationCause = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the SCP termination will be announced by C.A.S.S.I.E.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
