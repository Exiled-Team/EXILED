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
    public class TriggeringBloodlustEventArgs : IPlayerEvent, ITargetEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggeringBloodlustEventArgs"/> class.
        /// </summary>
        /// <param name="target">The <see cref="API.Features.Player"/> triggering the event.</param>
        /// <param name="scp0492">The <see cref="API.Features.Player"/> who is SCP-049-2.</param>
        public TriggeringBloodlustEventArgs(Player target, Player scp0492)
        {
            Player = scp0492;
            Target = target;
        }

        /// <inheritdoc />
        public Player Player { get; }

        /// <inheritdoc />
        public Player Target { get; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; } = true;
    }
}