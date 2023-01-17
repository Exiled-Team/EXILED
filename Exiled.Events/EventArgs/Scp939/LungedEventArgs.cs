// -----------------------------------------------------------------------
// <copyright file="LungedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using Interfaces;
    using PlayerRoles.PlayableScps.Scp939;

    /// <summary>
    ///     Contains all information after SCP-939 uses its lunge ability.
    /// </summary>
    public class LungedEventArgs : IPlayerEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LungedEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="state">
        ///     <inheritdoc cref="State" />
        /// </param>
        /// <param name="target">
        ///     <inheritdoc cref="Target" />
        /// </param>
        public LungedEventArgs(ReferenceHub player, Scp939LungeState state, ReferenceHub target)
        {
            Player = Player.Get(player);
            State = state;
            Target = Player.Get(target);
        }

        /// <summary>
        ///     Gets the player who's Scp-939 hit.
        /// </summary>
        public Player Target { get; }

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