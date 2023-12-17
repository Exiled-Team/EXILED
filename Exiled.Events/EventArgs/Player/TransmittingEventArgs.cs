// -----------------------------------------------------------------------
// <copyright file="TransmittingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using PlayerRoles.Voice;

    using VoiceChat;

    /// <summary>
    /// Contains all information regarding the player using the radio.
    /// </summary>
    public class TransmittingEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransmittingEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="voiceModule">
        /// <inheritdoc cref="VoiceModule" />
        /// </param>
        public TransmittingEventArgs(Player player, VoiceModuleBase voiceModule)
        {
            Player = player;
            VoiceModule = voiceModule;
            IsTransmitting = voiceModule == null ? false : voiceModule.ServerIsSending && voiceModule.CurrentChannel == VoiceChatChannel.Radio;
        }

        /// <summary>
        /// Gets the player who's transmitting.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets the <see cref="Player"/>'s <see cref="VoiceModuleBase" />.
        /// </summary>
        public VoiceModuleBase VoiceModule { get; }

        /// <summary>
        /// Gets a value indicating whether or not the player is transmitting.
        /// </summary>
        public bool IsTransmitting { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can transmit.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}