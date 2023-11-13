// -----------------------------------------------------------------------
// <copyright file="GainingLevelEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp079
{
    using API.Features;
    using Exiled.API.Features.Roles;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-079 gains a level.
    /// </summary>
    public class GainingLevelEventArgs : IScp079Event, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GainingLevelEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="newLevel">
        ///     <inheritdoc cref="NewLevel" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public GainingLevelEventArgs(Player player, int newLevel, bool isAllowed = true)
        {
            Player = player;
            Scp079 = player.Role.As<Scp079Role>();
            NewLevel = newLevel;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets SCP-079's new level.
        /// </summary>
        public int NewLevel { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the level is successfully granted.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's controlling SCP-079.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp079Role Scp079 { get; }
    }
}