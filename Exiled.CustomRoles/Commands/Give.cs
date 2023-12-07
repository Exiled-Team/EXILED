// -----------------------------------------------------------------------
// <copyright file="Give.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.CustomModules.API.Features;
    using Exiled.Permissions.Extensions;

    using RemoteAdmin;

    /// <summary>
    /// The command to give a role to player(s).
    /// </summary>
    internal sealed class Give : ICommand
    {
        private Give()
        {
        }

        /// <summary>
        /// Gets the <see cref="Give"/> command instance.
        /// </summary>
        public static Give Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "give";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "g" };

        /// <inheritdoc/>
        public string Description { get; } = "Gives the specified custom role to the indicated player(s).";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("customroles.give"))
                {
                    response = "Permission Denied, required: customroles.give";
                    return false;
                }

                if (arguments.Count == 0)
                {
                    response = "give <Custom role name/Custom role ID> [Nickname/PlayerID/UserID/all/*]";
                    return false;
                }

                if (!CustomRole.TryGet(arguments.At(0), out CustomRole? role) || role is null)
                {
                    response = $"Custom role {arguments.At(0)} not found!";
                    return false;
                }

                if (arguments.Count == 1)
                {
                    if (sender is PlayerCommandSender playerCommandSender)
                    {
                        Pawn player = Player.Get(playerCommandSender).Cast<Pawn>();

                        role.ForceSpawn(player);
                        response = $"{role.Name} given to {player.Nickname}.";
                        return true;
                    }

                    response = "Failed to provide a valid player.";
                    return false;
                }

                string identifier = string.Join(" ", arguments.Skip(1));

                switch (identifier)
                {
                    case "*":
                    case "all":
                        List<Player> players = ListPool<Player>.Pool.Get(Player.List);

                        foreach (Player player in players)
                            role.ForceSpawn(player.Cast<Pawn>());

                        response = $"Custom role {role.Name} given to all players.";
                        ListPool<Player>.Pool.Return(players);
                        return true;
                    default:
                        if (Player.Get(identifier) is not Player ply)
                        {
                            response = $"Unable to find a player: {identifier}";
                            return false;
                        }

                        role.ForceSpawn(ply.Cast<Pawn>());
                        response = $"{role.Name} given to {ply.Nickname}.";
                        return true;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                response = "Error";
                return false;
            }
        }
    }
}