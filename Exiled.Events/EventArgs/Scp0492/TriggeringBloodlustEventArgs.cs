// -----------------------------------------------------------------------
// <copyright file="TriggeringBloodlustEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp0492
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a <see cref="Scp0492Role"/> enters Bloodlust.
    /// </summary>
    public class TriggeringBloodlustEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeringBloodlustEventArgs"/> class.
        /// </summary>
        /// <param name="player">The <see cref="API.Features.Player"/> triggering the event.</param>
        /// <param name="scp0492">The <see cref="API.Features.Player"/> who is SCP-049-2.</param>
        public TriggeringBloodlustEventArgs(Player player, Player scp0492)
        {
            Player = player;
            Scp0492 = scp0492;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="API.Features.Player"/> who is SCP-049-2.
        /// </summary>
        public Player Scp0492 { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; } = true;
    }
}