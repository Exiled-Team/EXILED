// -----------------------------------------------------------------------
// <copyright file="AttackingDoorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp1507
{
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before SCP-1507 attacks door.
    /// </summary>
    public class AttackingDoorEventArgs : IScp1507Event, IDeniableEvent, IDoorEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttackingDoorEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="door"><inheritdoc cref="Door"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public AttackingDoorEventArgs(Player player, Door door, bool isAllowed = true)
        {
            Player = player;
            Scp1507 = player.Role.As<Scp1507Role>();
            Door = door;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp1507Role Scp1507 { get; }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; }

        /// <inheritdoc/>
        public Door Door { get; }
    }
}