// -----------------------------------------------------------------------
// <copyright file="LungingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp939;

    /// <summary>
    ///     Contains all information before SCP-939 uses its lunge ability.
    /// </summary>
    public class LungingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LungingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="state">
        ///     <inheritdoc cref="State" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public LungingEventArgs(ReferenceHub player, Scp939LungeState state, bool isAllowed = true)
        {
            Player = Player.Get(player);
            State = state;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a list of players to ignore 939 lunge damage to.
        /// </summary>
        public static HashSet<Player> LungeTargetsToIgnore { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not SCP-939 can lunge.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets scp939's lunge state. If you deny event, lunge state changes to this.
        /// </summary>
        public Scp939LungeState State { get; set; }
    }
}