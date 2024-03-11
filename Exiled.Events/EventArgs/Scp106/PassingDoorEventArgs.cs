// -----------------------------------------------------------------------
// <copyright file="PassingDoorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp106
{
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains information before SCP-106 passes a door.
    /// </summary>
    public class PassingDoorEventArgs : IScp106Event, IDoorEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PassingDoorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="door"><inheritdoc cref="Door"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public PassingDoorEventArgs(Player player, Door door, bool isAllowed = true)
        {
            Player = player;
            Scp106 = player.Role.As<Scp106Role>();
            Door = door;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp106Role Scp106 { get; }

        /// <inheritdoc/>
        public Door Door { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }
    }
}