// -----------------------------------------------------------------------
// <copyright file="VoiceChattingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Assets._Scripts.Dissonance;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

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
        /// <param name="radio">
        ///     <inheritdoc cref="Radio" />
        /// </param>
        /// <param name="dissonanceUserSetup">
        ///     <inheritdoc cref="DissonanceUserSetup" />
        /// </param>
        /// <param name="isVoiceChatting">
        ///     <inheritdoc cref="IsVoiceChatting" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public VoiceChattingEventArgs(Player player, Radio radio, DissonanceUserSetup dissonanceUserSetup, bool isVoiceChatting, bool isAllowed = true)
        {
            Player = player;
            Radio = radio;
            DissonanceUserSetup = dissonanceUserSetup;
            IsVoiceChatting = isVoiceChatting;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the player's <see cref="API.Features.Items.Radio" /> component.
        /// </summary>
        public Radio Radio { get; }

        /// <summary>
        ///     Gets the <see cref="API.Features.Items.Radio" />'s DissonanceUserSetup.
        /// </summary>
        public DissonanceUserSetup DissonanceUserSetup { get; }

        /// <summary>
        ///     Gets a value indicating whether or not the player is voicechatting.
        /// </summary>
        public bool IsVoiceChatting { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can voicechat.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's voicechatting.
        /// </summary>
        public Player Player { get; }
    }
}