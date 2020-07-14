// -----------------------------------------------------------------------
// <copyright file="Test.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Commands
{
    using System;

    using CommandSystem;

    using Exiled.API.Features;

    using RemoteAdmin;

    /// <summary>
    /// This is an example of how commands should be made.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Test : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "test";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { "t" };

        /// <inheritdoc/>
        public string Description { get; } = "A simple test command.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is PlayerCommandSender player)
                response = $"{Player.Get(player.SenderId).Nickname} sent the command!";
            else
                response = "The command has been sent from the server console!";

            return true;
        }
    }
}
