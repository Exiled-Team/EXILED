// -----------------------------------------------------------------------
// <copyright file="DancingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before SCP-3114 starts or stops dancing.
    /// </summary>
    public class DancingEventArgs : IDeniableEvent, IScp3114Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DancingEventArgs"/> class.
        /// </summary>
        /// <param name="newState"><inheritdoc cref="NewState"/></param>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public DancingEventArgs(bool newState, Player player, bool isAllowed = true)
        {
            Player = player;
            Scp3114 = Player.Role.As<Scp3114Role>();
            NewState = newState;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not SCP-3114 will dance.
        /// </summary>
        public bool NewState { get; }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }
    }
}