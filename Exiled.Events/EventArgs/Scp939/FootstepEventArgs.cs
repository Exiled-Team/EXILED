// -----------------------------------------------------------------------
// <copyright file="FootstepEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-939 uses its lunge ability.
    /// </summary>
    public class FootstepEventArgs : IScp939Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FootstepEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public FootstepEventArgs(Exiled.API.Features.Player player, bool isAllowed = true)
        {
            Player = player;
            Scp939 = Player.Role.As<Scp939Role>();
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the OnFootStepPlayed can begin.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
