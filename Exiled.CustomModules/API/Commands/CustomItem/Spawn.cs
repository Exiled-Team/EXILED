// -----------------------------------------------------------------------
// <copyright file="Spawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomItem
{
    using System;

    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using UnityEngine;

    using CustomItem = Exiled.CustomModules.API.Features.CustomItems.CustomItem;

    /// <summary>
    /// The command to spawn a specific item.
    /// </summary>
    internal sealed class Spawn : ICommand
    {
        private Spawn()
        {
        }

        /// <summary>
        /// Gets the <see cref="Spawn"/> instance.
        /// </summary>
        public static Spawn Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "spawn";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "sp" };

        /// <inheritdoc/>
        public string Description { get; } = "Spawns an item at the specified spawn location, coordinates, or at the designated player's position.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.spawn"))
            {
                response = "Permission denied, customitems.spawn is required.";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "spawn <Custom Item> [Location name]" +
                           "\nspawn <Custom Item> [Nickname / PlayerID / UserID]" +
                           "\nspawn <Custom Item> [X] [Y] [Z]";
                return false;
            }

            if (!(uint.TryParse(arguments.At(0), out uint id) &&
                  CustomItem.TryGet(id, out CustomItem item)) &&
                !CustomItem.TryGet(arguments.At(0), out item))
            {
                response = $" {arguments.At(0)} is not a valid custom item.";
                return false;
            }

            Vector3 position;

            if (Enum.TryParse(arguments.At(1), out SpawnLocationType location))
            {
                position = location.GetPosition();
            }
            else
            {
                if (Player.Get(arguments.At(1)) is Player player)
                {
                    if (player.IsDead)
                    {
                        response = $"Cannot spawn custom items under dead players!";
                        return false;
                    }

                    position = player.Position;
                }
                else if (arguments.Count > 3)
                {
                    if (!float.TryParse(arguments.At(1), out float x) || !float.TryParse(arguments.At(2), out float y) || !float.TryParse(arguments.At(3), out float z))
                    {
                        response = "Invalid coordinates.";
                        return false;
                    }

                    position = new Vector3(x, y, z);
                }
                else
                {
                    response = $"Unable to find spawn location: {arguments.At(1)}.";
                    return false;
                }
            }

            item?.Spawn(position);

            response = $"{item?.Name} ({item?.ItemType}) has been spawned at {position}.";
            return true;
        }
    }
}