// -----------------------------------------------------------------------
// <copyright file="StranglingFinishedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp3114;

    using Scp3114Role = Exiled.API.Features.Roles.Scp3114Role;

    /// <summary>
    ///     Contains all information after SCP-3114 finishes strangling a player (whether or not they kill the player).
    /// </summary>
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable MemberCanBePrivate.Global
    public sealed class StranglingFinishedEventArgs : IScp3114Event
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StranglingFinishedEventArgs" /> class.
        /// </summary>
        /// <param name="instance">
        ///     The <see cref="Scp3114Strangle"/> instance which this is being instantiated from.
        /// </param>
        /// <param name="priorTarget">
        ///     The prior <see cref="Scp3114Strangle.StrangleTarget"/> who was being targeted.
        /// </param>
        /// <param name="cooldown">
        ///     The default cooldown that is used for the <see cref="Scp3114Strangle.Cooldown"/>.
        /// </param>
        public StranglingFinishedEventArgs(Scp3114Strangle instance, Scp3114Strangle.StrangleTarget priorTarget, float cooldown)
        {
            Player = Player.Get(instance.Owner);
            Scp3114 = Player.Role.As<Scp3114Role>();
            StrangleInfo = priorTarget;
            Target = Player.Get(priorTarget.Target);
            StrangleCooldown = cooldown;
        }

        /// <summary>
        ///     Gets or sets the cooldown that will be triggered for the <see cref="Scp3114Strangle"/> instance.
        /// </summary>
        public double StrangleCooldown { get; set; }

        /// <inheritdoc/>
        /// <summary>
        ///     The <see cref="Player"/> who is Scp-3114.
        /// </summary>
        public Player Player { get; set; }

        /// <inheritdoc cref="IScp3114Event.Scp3114"/>
        public Scp3114Role Scp3114 { get; }

        /// <summary>
        ///     Gets the <see cref="Player"/> who was being strangled.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        ///     Gets <see cref="Scp3114Strangle.StrangleTarget"/> information for the player who was being strangled.
        /// </summary>
        public Scp3114Strangle.StrangleTarget StrangleInfo { get; }
    }
}
