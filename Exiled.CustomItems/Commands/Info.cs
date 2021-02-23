// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands
{
    using System;
    using System.Text;

    using CommandSystem;

    using Exiled.CustomItems.API;
    using Exiled.CustomItems.API.Features;
    using Exiled.CustomItems.API.Spawn;
    using Exiled.Permissions.Extensions;

    using NorthwoodLib.Pools;

    /// <summary>
    /// The command to view info about a specific item.
    /// </summary>
    internal sealed class Info : ICommand
    {
        private Info()
        {
        }

        /// <summary>
        /// Gets the <see cref="Info"/> instance.
        /// </summary>
        public static Info Instance { get; } = new Info();

        /// <inheritdoc/>
        public string Command { get; } = "info";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "i" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets more information about the specified custom item.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.info"))
            {
                response = "Permission Denied, required: customitems.info";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "info [Custom item name/Custom item ID]";
                return false;
            }

            if (Extensions.TryGetItem(arguments.At(1), out CustomItem item))
            {
                response = $"{arguments.At(1)} is not a valid custom item.";
                return false;
            }

            StringBuilder message = StringBuilderPool.Shared.Rent();

            message.AppendLine($"<color=#e6ac00>-</color> <color=#00d639>{item.Name}</color> <color=#05c4eb>({item.Id})</color>\n - {item.Description}\n{item.Type}\nSpawn Locations:");

            foreach (DynamicSpawnPoint spawnPoint in item.SpawnProperties.DynamicSpawnPoints)
                message.AppendLine($"{spawnPoint.Name} - {spawnPoint.Position} Chance: {spawnPoint.Chance}");

            foreach (StaticSpawnPoint spawnPoint in item.SpawnProperties.StaticSpawnPoints)
                message.AppendLine($"{spawnPoint.Name} - {spawnPoint.Position} Chance: {spawnPoint.Chance}");

            message.AppendLine($"Spawn Limit: {item.SpawnProperties.Limit}");

            response = StringBuilderPool.Shared.ToStringReturn(message);
            return true;
        }
    }
}
