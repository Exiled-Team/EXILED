// -----------------------------------------------------------------------
// <copyright file="SendingCommandEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    using PluginAPI.Commands;

    using RemoteAdmin;

    /// <summary>
    /// Contains all information before a player sends a propper RA command.
    /// </summary>
    public class SendingCommandEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendingCommandEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        /// <inheritdoc cref="Player" />
        /// </param>
        /// <param name="command">
        /// <inheritdoc cref="Command" />
        /// </param>
        /// <param name="querry">
        /// <inheritdoc cref="Querry" />
        /// </param>
        public SendingCommandEventArgs(Player player, ICommand command, string querry)
        {
            Command = command;
            Player = player;
            Querry = querry;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command can be executed.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        /// Gets command interface.
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// Gets query.
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// Gets the player who's sending the command.
        /// </summary>
        public Player Player { get; }
    }
}
