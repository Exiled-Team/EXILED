// -----------------------------------------------------------------------
// <copyright file="ItemInfo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;

namespace Exiled.CustomItems.Commands
{
    using System;

    using CommandSystem;

    using Exiled.CustomItems.API;

    /// <summary>
    /// The command to view info about a specific item.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ItemInfo : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "citeminfo";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "cinfo" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets more information about the specified item.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string[] args = arguments.Array;
            if (args == null || args.Length < 2)
            {
                response = "You must specify an item to get info from.";
                return false;
            }

            API.TryGetItem(args[1], out CustomItem item);
            if (item == null)
            {
                response = $"Invalid item: {args[1]}.";
                return false;
            }

            StringBuilder builder = NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();
            builder.AppendLine($"<color=#e6ac00>-</color> <color=#00d639>{item.Name}</color> <color=#05c4eb>({item.Id})</color>\n - {item.Description}\n{item.Type}\nSpawn Locations:");
            foreach (DynamicItemSpawn spawnLoc in item.SpawnProperties.DynamicSpawnLocations)
                builder.AppendLine($"{spawnLoc.Name} - {spawnLoc.Location.TryGetLocation()} Chance: {spawnLoc.Chance}");
            foreach (StaticItemSpawn spawnLoc in item.SpawnProperties.StaticSpawnLocations)
                builder.AppendLine($"{spawnLoc.Name} - {spawnLoc.Position} Chance: {spawnLoc.Chance}");
            builder.AppendLine($"Spawn Limit: {item.SpawnProperties.Limit}");

            response = NorthwoodLib.Pools.StringBuilderPool.Shared.ToStringReturn(builder);
            return true;
        }
    }
}
