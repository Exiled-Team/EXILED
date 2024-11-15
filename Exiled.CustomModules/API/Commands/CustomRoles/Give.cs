// -----------------------------------------------------------------------
// <copyright file="Give.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomRoles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.Permissions.Extensions;

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
        public string Description { get; } = "Gives the specified custom role (using ID) to the indicated player(s).";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("customroles.give"))
                {
                    response = "Permission denied, customroles.give is required.";
                    return false;
                }

                if (arguments.Count < 2)
                {
                    response = "give <Custom Role> [Nickname / PlayerID / UserID / all / *]";
                    return false;
                }

                if (!CustomRole.TryGet(arguments.At(0), out CustomRole role) &&
                    (!uint.TryParse(arguments.At(0), out uint id) ||
                     !CustomRole.TryGet(id, out role)) && role is null)
                {
                    response = $"Custom role {arguments.At(0)} not found!";
                    return false;
                }

                if (arguments.Count == 2)
                {
                    Pawn player = Player.Get(arguments.At(1)).Cast<Pawn>();

                    if (player is null)
                    {
                        response = "Player not found.";
                        return false;
                    }

                    role.Spawn(player, false, force: true);
                    response = $"{role.Name} given to {player.Nickname}.";
                    return true;
                }

                string identifier = string.Join(" ", arguments.Skip(1));

                switch (identifier)
                {
                    case "*":
                    case "all":
                        List<Pawn> players = ListPool<Player>.Pool.Get(Player.List).Select(player => player.Cast<Pawn>()).ToList();
                        role.Spawn(players, true);

                        response = $"Custom role {role.Name} given to all players.";
                        ListPool<Pawn>.Pool.Return(players);
                        return true;
                    default:
                        if (Player.Get(identifier).Cast<Pawn>() is not Pawn ply)
                        {
                            response = $"Unable to find a player: {identifier}";
                            return false;
                        }

                        role.Spawn(ply);
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