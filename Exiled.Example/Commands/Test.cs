// -----------------------------------------------------------------------
// <copyright file="Test.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.Commands {
    using System;
    using System.Linq;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using RemoteAdmin;

    /// <summary>
    /// This is an example of how commands should be made.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Test : ICommand {
        /// <inheritdoc/>
        public string Command { get; } = "test";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { "t" };

        /// <inheritdoc/>
        public string Description { get; } = "A simple test command.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response) {
            Player player = Player.Get(((CommandSender)sender).SenderId);
            Log.Warn($"{player.Items.Count} -- {player.Inventory.UserInventory.Items.Count}");
            foreach (Pickup pickup in Map.Pickups)
                Log.Warn($"{pickup.Type} ({pickup.Serial}) -- {pickup.Position}");
            foreach (PocketDimensionTeleport teleport in Map.PocketDimensionTeleports)
                Log.Warn($"{teleport._type}");
            player.ClearInventory();
            response = player != null ? $"{player.Nickname} sent the command!" : "The command has been sent from the server console!";

            // Return true if the command was executed successfully; otherwise, false.
            return true;
        }
    }
}
