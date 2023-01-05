// -----------------------------------------------------------------------
// <copyright file="VoiceChattingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using PlayerRoles.Voice;

    /// <summary>
    ///     Contains all information after a player presses the voicechat key.
    /// </summary>
    public class VoiceChattingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="VoiceChattingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="voiceModule">
        ///     <inheritdoc cref="VoiceModule" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public VoiceChattingEventArgs(Player player, VoiceModuleBase voiceModule, bool isAllowed = true)
        {
            Player = player;
            VoiceModule = voiceModule;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player who's voicechatting.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets the <see cref="Player"/>'s <see cref="VoiceModuleBase" />.
        /// </summary>
        public VoiceModuleBase VoiceModule { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can voicechat.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}