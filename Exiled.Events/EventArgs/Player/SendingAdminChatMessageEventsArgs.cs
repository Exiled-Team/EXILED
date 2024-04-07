// -----------------------------------------------------------------------
// <copyright file="SendingAdminChatMessageEventsArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information before a player sends a message in AdminChat.
    /// </summary>
    public class SendingAdminChatMessageEventsArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendingAdminChatMessageEventsArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="message">
        /// <inheritdoc cref="Message" />
        /// </param>
        /// <param name="isAllowed">
        /// <inheritdoc cref="IsAllowed" />
        /// </param>
        public SendingAdminChatMessageEventsArgs(Player player, string message, bool isAllowed)
        {
            Player = player;
            Message = message;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup can be searched.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        /// Gets or sets the message which is being sent.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the player who's sending the message.
        /// </summary>
        public Player Player { get; }
    }
}
