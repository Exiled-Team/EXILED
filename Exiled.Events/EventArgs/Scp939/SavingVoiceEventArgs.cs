// -----------------------------------------------------------------------
// <copyright file="SavingVoiceEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Scp939
{
    using API.Features;
    using Interfaces;

    /// <summary>
    ///     Contains all information before SCP-939 plays a stolen player's voice.
    /// </summary>
    public class SavingVoiceEventArgs : IPlayerEvent, ITargetEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SavingVoiceEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="stolen">
        ///     The player who's voice was stolen.
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public SavingVoiceEventArgs(ReferenceHub player, ReferenceHub stolen, bool isAllowed = true)
        {
            Player = Player.Get(player);
            Target = Player.Get(stolen);
            IsAllowed = isAllowed;
        }

        /// <inheritdoc />
        public bool IsAllowed { get; set; }

        /// <inheritdoc />
        public Player Target { get; }

        /// <inheritdoc />
        public Player Player { get; }
    }
}