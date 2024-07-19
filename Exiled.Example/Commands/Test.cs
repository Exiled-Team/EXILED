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
    using Exiled.API.Features.Pickups;

    /// <summary>
    /// This is an example of how commands should be made.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Test : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "test";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "t" };

        /// <inheritdoc/>
        public string Description { get; } = "A simple test command.";

        /// <inheritdoc cref="SanitizeResponse" />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            Log.Warn($"{player.Items.Count} -- {player.Inventory.UserInventory.Items.Count}");

            foreach (Player item in Player.List)
                Log.Warn(item);

            foreach (Pickup pickup in Pickup.List)
                Log.Warn($"{pickup.Type} ({pickup.Serial}) -- {pickup.Position}");

            foreach (PocketDimensionTeleport teleport in Map.PocketDimensionTeleports)
                Log.Warn($"{teleport._type}");

            player.ClearInventory();
            response = $"{player.Nickname} sent the command!";

            // Return true if the command was executed successfully; otherwise, false.
            return true;
        }
    }
}