// -----------------------------------------------------------------------
// <copyright file="CalmingDownEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp096
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before SCP-096 calms down.
    /// </summary>
    public class CalmingDownEventArgs : IScp096Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CalmingDownEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's controlling SCP-096.</param>
        /// <param name="shouldClearEnragedTimeLeft"><inheritdoc cref="ShouldClearEnragedTimeLeft"/></param>
        /// <param name="isAllowed">Indicates whether or not SCP-096 can calm down.</param>
        public CalmingDownEventArgs(Player player, bool shouldClearEnragedTimeLeft, bool isAllowed = true)
        {
            Player = player;
            Scp096 = player.Role.As<Scp096Role>();
            ShouldClearEnragedTimeLeft = shouldClearEnragedTimeLeft;
            IsAllowed = isAllowed;
        }

        /// <inheritdoc/>
        public Scp096Role Scp096 { get; }

        /// <summary>
        ///     Gets the player who's controlling SCP-096.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether SCP-096 enrage time left should be cleared or not.
        /// </summary>
        public bool ShouldClearEnragedTimeLeft { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-096 can be enraged.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}