// -----------------------------------------------------------------------
// <copyright file="StranglingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp3114;

    using Scp3114Role = Exiled.API.Features.Roles.Scp3114Role;

    /// <summary>
    ///     Contains all information before SCP-3114 strangles a player.
    /// </summary>
    public sealed class StranglingEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StranglingEventArgs" /> class.
        /// </summary>
        /// <param name="instance">
        ///     The <see cref="Scp3114Strangle"/> instance which this is being instantiated from.
        /// </param>
        /// <param name="target">
        ///     The <see cref="Player"/> being targeted.
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IDeniableEvent.IsAllowed" />
        /// </param>
        public StranglingEventArgs(Scp3114Strangle instance, Scp3114Strangle.StrangleTarget target, bool isAllowed = true)
        {
            Player = Player.Get(instance.Owner);
            Scp3114 = Player.Role.As<Scp3114Role>();
            StrangleInfo = target;
            Target = Player.Get(target.Target);
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        /// <summary>
        ///     The <see cref="Player"/> who is Scp-3114.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }

        /// <summary>
        ///     Gets <see cref="Scp3114Strangle.StrangleTarget"/> information for the player who was being strangled.
        /// </summary>
        public Scp3114Strangle.StrangleTarget StrangleInfo { get; }

        /// <summary>
        ///     Gets the <see cref="Player"/> being strangled.
        /// </summary>
        public Player Target { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}