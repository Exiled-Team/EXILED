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

    /// <summary>
    ///     Contains all information before SCP-3114 it's diguised to a new Roles.
    /// </summary>
    public class DisguisingEventArgs : IScp3114Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DisguisingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        public DisguisingEventArgs(ReferenceHub player)
        {
            Player = Player.Get(player);
            Scp3114 = Player.Role.As<Scp3114Role>();
        }

        /// <inheritdoc/>
        public bool IsAllowed { get; set; } = true;

        /// <inheritdoc/>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp3114Role Scp3114 { get; }
    }
}