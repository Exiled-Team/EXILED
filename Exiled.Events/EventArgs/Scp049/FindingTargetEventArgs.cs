// -----------------------------------------------------------------------
// <copyright file="FindingTargetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp049
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before SCP-049 find a target to activate good sense of the doctor.
    /// </summary>
    public class FindingTargetEventArgs : IScp049Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindingTargetEventArgs" /> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        public FindingTargetEventArgs(Player player, Player target)
        {
            Player = player;
            Scp049 = player.Role.As<Scp049Role>();
            Target = target;
        }

        /// <inheritdoc/>
        public Scp049Role Scp049 { get; }

        /// <summary>
        /// Gets the player who is playing as SCP-049.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the player who will be affected by the sense ability.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the target can be affected.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}