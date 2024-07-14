// -----------------------------------------------------------------------
// <copyright file="SavingVoiceEventArgs.cs" company="Exiled Team">
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
    /// Contains all information before SCP-939 plays a stolen player's voice.
    /// </summary>
    public class SavingVoiceEventArgs : IScp939Event, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavingVoiceEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="stolen">
        /// The player who's voice was stolen.
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public SavingVoiceEventArgs(ReferenceHub player, ReferenceHub stolen, bool isAllowed = true)
        {
            Player = Player.Get(player);
            Scp939 = Player.Role.As<Scp939Role>();
            Stolen = Player.Get(stolen);
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-939 can play the stolen voice.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets the players who's voice was stolen.
        /// </summary>
        public Player Stolen { get; }

        /// <summary>
        /// Gets the player who's controlling SCP-939.
        /// </summary>
        public Player Player { get; }

        /// <inheritdoc/>
        public Scp939Role Scp939 { get; }
    }
}