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

    /// <summary>
    /// Contains all informations before C.A.S.S.I.E announces an SCP termination.
    /// </summary>
    public class AnnouncingScpTerminationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnouncingScpTerminationEventArgs"/> class.
        /// </summary>
        /// <param name="killer"><inheritdoc cref="Killer"/></param>
        /// <param name="role"><inheritdoc cref="Role"/></param>
        /// <param name="hitInfo"><inheritdoc cref="HitInfo"/></param>
        /// <param name="terminationCause"><inheritdoc cref="TerminationCause"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AnnouncingScpTerminationEventArgs(Player killer, Role role, PlayerStats.HitInfo hitInfo, string terminationCause, bool isAllowed = true)
        {
            Killer = killer;
            Role = role;
            HitInfo = hitInfo;
            TerminationCause = terminationCause;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the player who killed the SCP.
        /// </summary>
        public Player Killer { get; }

        /// <summary>
        /// Gets the killed <see cref="Role"/>.
        /// </summary>
        public Role Role { get; }

        /// <summary>
        /// Gets or sets the hit info.
        /// </summary>
        public PlayerStats.HitInfo HitInfo { get; set; }

        /// <summary>
        /// Gets or sets the termination cause.
        /// </summary>
        public string TerminationCause { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the SCP termination will be announced by C.A.S.S.I.E.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
