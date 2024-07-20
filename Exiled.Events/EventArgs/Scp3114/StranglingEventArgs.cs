// -----------------------------------------------------------------------
// <copyright file="StranglingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using PlayerRoles.PlayableScps.Scp3114;

    using HumanRole = PlayerRoles.HumanRole;
    using Scp3114Role = API.Features.Roles.Scp3114Role;

    /// <summary>
    /// Contains all information before strangling a player.
    /// </summary>
    public class StranglingEventArgs : IScp3114Event, IDeniableEvent
    {
        private readonly Scp3114Strangle strangle;
        private Player target = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="StranglingEventArgs"/> class.
        /// </summary>
        /// <param name="strangle">The <see cref="Scp3114Strangle"/> instance.</param>
        /// <param name="player">The <see cref="API.Features.Player"/> triggering the event.</param>
        /// <param name="target">The <see cref="API.Features.Player"/> being targeted.</param>
        public StranglingEventArgs(Scp3114Strangle strangle, Player player, Scp3114Strangle.StrangleTarget target)
        {
            this.strangle = strangle;
            Scp3114 = player.Role.As<Scp3114Role>();
            Player = player;
            Target = Player.Get(target.Target);
            StrangleTarget = target;
        }

        /// <inheritdoc />
        public Scp3114Role Scp3114 { get; }

        /// <inheritdoc />
        public Player Player
        {
            get => target;
            set
            {
                target = value;

                if (!target)
                {
                    StrangleTarget = default(Scp3114Strangle.StrangleTarget);
                    return;
                }

                StrangleTarget = new(target.ReferenceHub, strangle.GetStranglePosition(target.ReferenceHub.roleManager.CurrentRole as HumanRole), Player.Position);
            }
        }

        /// <summary>
        /// Gets the target player.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets the <see cref="Scp3114Strangle.StrangleTarget"/>. This value is updated when <see cref="Target"/> is changed.
        /// </summary>
        public Scp3114Strangle.StrangleTarget? StrangleTarget { get; private set; }

        /// <inheritdoc />
        public bool IsAllowed { get; set; } = true;
    }
}