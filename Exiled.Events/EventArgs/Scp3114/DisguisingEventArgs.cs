// -----------------------------------------------------------------------
// <copyright file="DisguisingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp3114
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    using PlayerRoles.Ragdolls;

    /// <summary>
    /// Contains all information before SCP-3114 disguises to a new role.
    /// </summary>
    public class DisguisingEventArgs : IScp3114Event, IDeniableEvent, IRagdollEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisguisingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="ragdoll">
        /// <inheritdoc cref="Ragdoll" />
        /// </param>
        public DisguisingEventArgs(ReferenceHub player, DynamicRagdoll ragdoll)
        {
            Player = Player.Get(player);
            Scp3114 = Player.Role.As<Scp3114Role>();
            Ragdoll = Ragdoll.Get(ragdoll);
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }

        /// <inheritdoc/>
        public Ragdoll Ragdoll { get; }
    }
}