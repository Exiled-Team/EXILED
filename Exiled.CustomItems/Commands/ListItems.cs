// -----------------------------------------------------------------------
// <copyright file="ListItems.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands
{
    using System;
    using CommandSystem;
    using Exiled.CustomItems.API;

    /// <summary>
    /// The command to list all installed items.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ListItems : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "citemlist";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "clist" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets a list of all custom items currently installed and enabled on the server.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string message = string.Empty;
            foreach (CustomItem item in CustomItems.Singleton.ItemManagers)
                message += $"{item.Name}({item.Id})\n";

            response = string.IsNullOrEmpty(message) ? "There are no custom items currently on this server." : message;

            return true;
        }
    }
}
