// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTerminationEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Map
{
    using API.Features;
    using API.Features.DamageHandlers;
    using API.Features.Roles;

    using Interfaces;

    using CustomAttackerHandler = API.Features.DamageHandlers.AttackerDamageHandler;
    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    ///     Contains all information before C.A.S.S.I.E announces an SCP termination.
    /// </summary>
    public class AnnouncingScpTerminationEventArgs : IAttackerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AnnouncingScpTerminationEventArgs" /> class.
        /// </summary>
        /// <param name="scp">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="damageHandlerBase">
        ///     <inheritdoc cref="DamageHandler" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public AnnouncingScpTerminationEventArgs(Player scp, DamageHandlerBase damageHandlerBase, bool isAllowed = true)
        {
            Player = scp;
            Role = scp.Role;
            DamageHandler = new CustomDamageHandler(scp, damageHandlerBase);
            Attacker = DamageHandler.BaseIs(out CustomAttackerHandler customAttackerHandler) ? customAttackerHandler.Attacker : null;
            TerminationCause = damageHandlerBase.CassieDeathAnnouncement.Announcement;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the killed <see cref="API.Features.Roles.Role" />.
        /// </summary>
        public Role Role { get; }

        /// <summary>
        ///     Gets or sets the termination cause.
        /// </summary>
        public string TerminationCause { get; set; }

        /// <summary>
        ///     Gets the player the announcement is being played for.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the player who killed the SCP.
        /// </summary>
        public Player Attacker { get; }

        /// <summary>
        ///     Gets or sets the <see cref="CustomDamageHandler" />.
        /// </summary>
        public CustomDamageHandler DamageHandler { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the SCP termination will be announced by C.A.S.S.I.E.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}