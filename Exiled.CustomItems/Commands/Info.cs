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

    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Spawn;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

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
        public static Info Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "info";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "i" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets more information about the specified custom item.";

        /// <inheritdoc />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.info"))
            {
                response = "Permission Denied, required: customitems.info";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "info [Custom item name/Custom item ID]";
                return false;
            }

            if (!(uint.TryParse(arguments.At(0), out uint id) && CustomItem.TryGet(id, out CustomItem? item)) &&
                !CustomItem.TryGet(arguments.At(0), out item))
            {
                response = $"{arguments.At(0)} is not a valid custom item.";
                return false;
            }

            StringBuilder message = StringBuilderPool.Pool.Get().AppendLine();

            message.Append("<color=#E6AC00>-</color> <color=#00D639>").Append(item?.Name).Append("</color> <color=#05C4EB>(").Append(item?.Id).AppendLine(")</color>")
                .Append("- ").AppendLine(item?.Description)
                .AppendLine(item?.Type.ToString())
                .Append("- Spawn Limit: ").AppendLine(item?.SpawnProperties?.Limit.ToString()).AppendLine()
                .Append("[Spawn Locations (").Append(item?.SpawnProperties?.DynamicSpawnPoints.Count + item?.SpawnProperties?.StaticSpawnPoints.Count).AppendLine(")]");

            foreach (DynamicSpawnPoint spawnPoint in item?.SpawnProperties?.DynamicSpawnPoints!)
                message.Append(spawnPoint.Name).Append(' ').Append(spawnPoint.Position).Append(" Chance: ").Append(spawnPoint.Chance).AppendLine("%");

            foreach (StaticSpawnPoint spawnPoint in item.SpawnProperties.StaticSpawnPoints)
                message.Append(spawnPoint.Name).Append(' ').Append(spawnPoint.Position).Append(" Chance: ").Append(spawnPoint.Chance).AppendLine("%");

            response = StringBuilderPool.Pool.ToStringReturn(message);
            return true;
        }
    }
}