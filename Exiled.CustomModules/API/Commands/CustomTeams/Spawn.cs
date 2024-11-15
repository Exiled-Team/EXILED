// -----------------------------------------------------------------------
// <copyright file="Spawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomTeams
{
    using System;
    using System.Linq;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.Permissions.Extensions;
    using PlayerRoles;

    /// <summary>
    /// The command to spawn a custom team.
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
        public string[] Aliases { get; } = { "s" };

        /// <inheritdoc/>
        public string Description { get; } = "Spawns the specified custom team.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("customteams.spawn"))
                {
                    response = "Permission denied, customteams.spawn is required.";
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "spawn <Custom Team>";
                    return false;
                }

                if (!CustomTeam.TryGet(arguments.At(0), out CustomTeam team) &&
                    (!uint.TryParse(arguments.At(0), out uint id) ||
                     !CustomTeam.TryGet(id, out team)) && team is null)
                {
                    response = $"Custom team {arguments.At(0)} not found!";
                    return false;
                }

                if (!Player.Get(Team.Dead).Any())
                {
                    response = "There are no dead players to spawn the custom team.";
                    return false;
                }

                team.Respawn(true);

                response = $"Custom team {team.Name} has been spawned.";
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                response = "An error occurred when executing the command, check server console for more details.";
                return false;
            }
        }
    }
}