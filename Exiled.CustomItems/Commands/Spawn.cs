// -----------------------------------------------------------------------
// <copyright file="Spawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands
{
    using System;

    using CommandSystem;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    using UnityEngine;

    /// <summary>
    /// The command to spawn a specific item.
    /// </summary>
    internal sealed class Spawn : ICommand
    {
        private Spawn()
        {
        }

        /// <summary>
        /// Gets the <see cref="Info"/> instance.
        /// </summary>
        public static Spawn Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "spawn";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "sp" };

        /// <inheritdoc/>
        public string Description { get; } = "Spawn an item at the specified Spawn Location, coordinates, or at the designated player's feet.";

        /// <inheritdoc />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.spawn"))
            {
                response = "Permission Denied, required: customitems.spawn";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "spawn [Custom item name] [Location name]\nspawn [Custom item name] [Nickname/PlayerID/UserID]\nspawn [Custom item name] [X] [Y] [Z]";
                return false;
            }

            if (!CustomItem.TryGet(arguments.At(0), out CustomItem? item))
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
                        response = "Invalid coordinates selected.";
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

            response = $"{item?.Name} ({item?.Type}) has been spawned at {position}.";
            return true;
        }
    }
}