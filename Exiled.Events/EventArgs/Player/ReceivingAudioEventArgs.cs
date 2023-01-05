// -----------------------------------------------------------------------
// <copyright file="ChangingRadioPresetEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Mirror;
using PlayerRoles.Voice;
using VoiceChat;
using VoiceChat.Networking;

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Enums;
    using Interfaces;

    using static InventorySystem.Items.Radio.RadioMessages;

    /// <summary>
    ///     Contains all information before radio preset is changed.
    /// </summary>
    public class ReceivingAudioEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangingRadioPresetEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="oldValue">
        ///     <inheritdoc cref="OldValue" />
        /// </param>
        /// <param name="newValue">
        ///     <inheritdoc cref="NewValue" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
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
        /// Players to send audio to if <see cref="CustomChat"/> is enabled.
        /// </summary>
        public HashSet<ReferenceHub> CustomConnectionPlayers { get;} = new();

        /// <summary>
        /// Set whether message should be shared to everyone or specific individuals with <see cref="CustomConnectionPlayers"/>
        /// </summary>
        public bool CustomChat { get; set; }

        /// <summary>
        /// Voice channel for player.
        /// </summary>
        public VoiceChatChannel Channel { get; set; }

        /// <summary>
        /// Current voice message from player.
        /// </summary>
        public VoiceMessage VoiceMsg { get; }

        /// <summary>
        /// Current connection of player.
        /// </summary>
        public NetworkConnection Connection { get; }

        /// <summary>
        ///  Current player voice role.
        /// </summary>
        public IVoiceRole VoiceRole { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the radio preset can be changed or not.
        ///     <remarks>Client radio graphics won't sync with <see cref="OldValue" />.</remarks>
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who's using the radio.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Check rate limit
        /// </summary>
        public bool CheckRateLimit { get; set; }

        /// <summary>
        /// Players who should not receive audio, Referencehub for efficiency.
        /// </summary>
        public HashSet<ReferenceHub> PlayersToNotReceiveAudio { get; } = new();

        /// <summary>
        /// Whether the current checks for allowing audio for each role should be ran
        /// </summary>
        public bool BypassAudioValidateReceive { get; set; }

        /// <summary>
        /// Bypass rules of send (NW checks of range/class/role/etc).
        /// </summary>
        public bool BypassAudioValidateSend { get; set; }
    }
}