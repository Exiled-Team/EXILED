// -----------------------------------------------------------------------
// <copyright file="Give.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomItem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    using CustomItem = Exiled.CustomModules.API.Features.CustomItems.CustomItem;

    /// <summary>
    /// The command to give a player an item.
    /// </summary>
    internal sealed class Give : ICommand
    {
        private Give()
        {
        }

        /// <summary>
        /// Gets the <see cref="Give"/> instance.
        /// </summary>
        public static Give Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "give";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "g" };

        /// <inheritdoc/>
        public string Description { get; } = "Gives a custom item.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.give"))
            {
                response = "Permission Denied, required: customitems.give";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "give <Custom item ID> [Nickname/PlayerID/UserID/all/*]";
                return false;
            }

            if (!CustomItem.TryGet(arguments.At(0), out CustomItem item) && (!uint.TryParse(arguments.At(0), out uint id) || !CustomItem.TryGet(id, out item)) && item is null)
            {
                response = $"Custom item {arguments.At(0)} not found!";
                return false;
            }

            if (arguments.Count == 1)
            {
                if (sender is PlayerCommandSender playerCommandSender)
                {
                    Player player = Player.Get(playerCommandSender.SenderId);

                    if (!CheckEligible(player))
                    {
                        response = "You cannot receive custom items!";
                        return false;
                    }

                    item?.Give(player);
                    response = $"{item?.Name} given to {player.Nickname} ({player.UserId})";
                    return true;
                }

                response = "Failed to provide a valid player, please follow the syntax.";
                return false;
            }

            string identifier = string.Join(" ", arguments.Skip(1));

            switch (identifier)
            {
                case "*":
                case "all":
                    List<Player> eligiblePlayers = Player.List.Where(CheckEligible).ToList();
                    foreach (Player ply in eligiblePlayers)
                        item?.Give(ply);

                    response = $"Custom item {item?.Name} given to all players who can receive them ({eligiblePlayers.Count} players)";
                    return true;
                default:
                    if (Player.Get(identifier) is not { } player)
                    {
                        response = $"Unable to find player: {identifier}.";
                        return false;
                    }

                    if (!CheckEligible(player))
                    {
                        response = "Player cannot receive custom items!";
                        return false;
                    }

                    item?.Give(player);
                    response = $"{item?.Name} given to {player.Nickname} ({player.UserId})";
                    return true;
            }
        }

        /// <summary>
        /// Checks if the player is eligible to receive custom items.
        /// </summary>
        private bool CheckEligible(Player player) => player.IsAlive && !player.IsCuffed && (player.Items.Count < 8);
    }
}