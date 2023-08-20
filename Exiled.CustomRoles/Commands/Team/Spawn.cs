// -----------------------------------------------------------------------
// <copyright file="Spawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands.Team
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;
    using Respawning;
    using YamlDotNet.Core.Tokens;

    /// <summary>
    /// The command to spawn a specific team.
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
        public string[] Aliases { get; } = { "s" };

        /// <inheritdoc/>
        public string Description { get; } = "Spawn a specified custom team.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("customroles.spawn"))
                {
                    response = "Permission Denied, required: customroles.spawn";
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "info [Custom team name/Custom team ID]";
                    return false;
                }

                if ((!(uint.TryParse(arguments.At(0), out uint id) && CustomTeam.TryGet(id, out CustomTeam? team)) && !CustomTeam.TryGet(arguments.At(0), out team)) || team is null)
                {
                    response = $"{arguments.At(0)} is not a valid custom team.";
                    return false;
                }

                List<Player> players = Player.List.Where(p => RespawnManager.Singleton.CheckSpawnable(p.ReferenceHub)).ToList();
                players.ShuffleList();
                if (players.Count > team.ResapwnAmount && team.ResapwnAmount > 0)
                    players.RemoveRange(team.ResapwnAmount, players.Count - team.ResapwnAmount);
                team.Spawn(players);

                response = "Team respawned";
                return true;
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