// -----------------------------------------------------------------------
// <copyright file="ReceivingAudioEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System.Collections.Generic;

    using API.Features;
    using Exiled.API.Enums;
    using Interfaces;
    using Mirror;
    using PlayerRoles.Voice;
    using VoiceChat;
    using VoiceChat.Networking;

    using static InventorySystem.Items.Radio.RadioMessages;

    /// <summary>
    ///     Contains all information before radio preset is changed.
    /// </summary>
    public class ReceivingAudioEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivingAudioEventArgs"/> class.
        /// </summary>
        /// <param name="player"> <inheritdoc cref="ReceivingAudioEventArgs.Player" /></param>
        /// <param name="voiceRole"> <inheritdoc cref="ReceivingAudioEventArgs.VoiceRole" /></param>
        /// <param name="connection"> <inheritdoc cref="ReceivingAudioEventArgs.Connection" /></param>
        /// <param name="msg"> <inheritdoc cref="ReceivingAudioEventArgs.VoiceMsg" /></param>
        /// <param name="isAllowed"> <inheritdoc cref="ReceivingAudioEventArgs.IsAllowed" /></param>
        public ReceivingAudioEventArgs(Player player, IVoiceRole voiceRole, NetworkConnection connection, VoiceMessage msg, bool isAllowed = true)
        {
            Player = player;
            VoiceRole = voiceRole;
            Connection = connection;
            VoiceMsg = msg;
            CheckRateLimit = true;
            IsAllowed = isAllowed;
            BypassAudioValidateReceive = false;
            BypassAudioValidateSend = false;
            Channel = msg.Channel;
            CustomChat = false;
        }

        /// <summary>
        /// Gets players to send audio to if <see cref="CustomChat"/> is enabled.
        /// </summary>
        public HashSet<ReferenceHub> CustomConnectionPlayers { get; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether set whether message should be shared to everyone or specific individuals with <see cref="CustomConnectionPlayers"/>.
        /// </summary>
        public bool CustomChat { get; set; }

        /// <summary>
        /// Gets or sets voice channel for player.
        /// </summary>
        public VoiceChatChannel Channel { get; set; }

        /// <summary>
        /// Gets current voice message from player.
        /// </summary>
        public VoiceMessage VoiceMsg { get; }

        /// <summary>
        /// Gets current connection of player.
        /// </summary>
        public NetworkConnection Connection { get; }

        /// <summary>
        ///  Gets current player voice role.
        /// </summary>
        public IVoiceRole VoiceRole { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the audio can be send to other clients.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's using the radio.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets a value indicating whether check rate limit.
        /// </summary>
        public bool CheckRateLimit { get; set; }

        /// <summary>
        /// Gets players who should not receive audio, Referencehub for efficiency.
        /// </summary>
        public HashSet<ReferenceHub> PlayersToNotReceiveAudio { get; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether whether the current checks for allowing audio for each role should be ran.
        /// </summary>
        public bool BypassAudioValidateReceive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether bypass rules of send (NW checks of range/class/role/etc).
        /// </summary>
        public bool BypassAudioValidateSend { get; set; }
    }
}