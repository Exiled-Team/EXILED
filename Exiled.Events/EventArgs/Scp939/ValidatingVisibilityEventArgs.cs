// -----------------------------------------------------------------------
// <copyright file="ValidatingVisibilityEventArgs.cs" company="Exiled Team">
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
    ///     Contains all information before SCP-939 sees the player.
    /// </summary>
    public class ValidatingVisibilityEventArgs : IScp939Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ValidatingVisibilityEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="target">
        ///     The target being shown to SCP-939.
        /// </param>
        /// <param name="isAllowed">
        ///     Whether or not SCP-939 is allowed to view the player.
        /// </param>
        public ValidatingVisibilityEventArgs(ReferenceHub player, ReferenceHub target, bool isAllowed)
        {
            Player = Player.Get(player);
            Scp939 = Player.Role.As<Scp939Role>();
            Target = Player.Get(target);
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player being shown to SCP-939.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        ///     Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }

        /// <summary>
        ///    Gets or sets a value indicating whether or not SCP-939 is allowed to view the player.
        /// </summary>
        public bool IsAllowed { get; set;  }
    }
}