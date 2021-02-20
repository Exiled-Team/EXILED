// -----------------------------------------------------------------------
// <copyright file="SpawnItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands
{
    using System;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using Exiled.Permissions.Extensions;

    using UnityEngine;

    /// <summary>
    /// The command to spawn a specific item.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class SpawnItem : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "citemspawn";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "cspawn" };

        /// <inheritdoc/>
        public string Description { get; } = "Spawn an item at the specified Spawn Location, coordinates, or at the designated player's feet.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("citems.spawn"))
            {
                response = "Permission Denied.";

                return false;
            }

            string[] args = arguments.Array;

            if (args == null || args.Length < 3)
            {
                response = "You must specify an item to spawn and a location/player to spawn it at.";
                return false;
            }

            if (!CustomItemApi.TryGetItem(args[1], out CustomItem item))
            {
                response = $"Invalid item: {args[1]}";
                return false;
            }

            Vector3 spawnPos = Vector3.zero;

            if (Enum.TryParse(args[2], out SpawnLocation location))
            {
                spawnPos = location.TryGetLocation();
            }
            else if (Player.Get(args[2]) is Player player)
            {
                spawnPos = player.Position;
            }
            else if (args.Length > 4)
            {
                if (!float.TryParse(args[2], out float x) || !float.TryParse(args[3], out float y) || !float.TryParse(args[4], out float z))
                {
                    response = "Invalid coordinates selected.";
                    return false;
                }

                spawnPos = new Vector3(x, y, z);
            }

            if (spawnPos == Vector3.zero)
            {
                response = $"Unable to find spawn location: {args[2]}";
                return false;
            }

            item.Spawn(spawnPos);
            response = $"{item.Name} has been spawned at {spawnPos}.";

            return true;
        }
    }
}
