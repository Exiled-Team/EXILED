// -----------------------------------------------------------------------
// <copyright file="Attach.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomEscapes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The command to attach a custom escape to a player(s).
    /// </summary>
    internal sealed class Attach : ICommand
    {
        private Attach()
        {
        }

        /// <summary>
        /// Gets the <see cref="Attach"/> instance.
        /// </summary>
        public static Attach Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "attach";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "a" };

        /// <inheritdoc/>
        public string Description { get; } = "Attach the specified custom escape to a player(s).";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("customescapes.attach"))
                {
                    response = "Permission denied, customescapes.attach is required.";
                    return false;
                }

                if (arguments.Count < 2)
                {
                    response = "attach [Custom Escape ID > Nickname / PlayerID / UserID / all / *]";
                    return false;
                }

                if (!CustomEscape.TryGet(arguments.At(0), out CustomEscape escape) && (!uint.TryParse(arguments.At(0), out uint id) || !CustomEscape.TryGet(id, out escape)) && escape is null)
                {
                    response = $"Custom Escape {arguments.At(0)} not found!";
                    return false;
                }

                if (arguments.Count == 2)
                {
                    Player player = Player.Get(arguments.At(1));

                    if (player is null)
                    {
                        response = "Player not found.";
                        return false;
                    }

                    escape.Attach(player);
                    response = $"{escape.Name} ({escape.Id}) has been attached to {player.Nickname}.";
                    return true;
                }

                string identifier = string.Join(" ", arguments.Skip(1));

                switch (identifier)
                {
                    case "*":
                    case "all":
                        List<Player> players = ListPool<Player>.Pool.Get(Player.List).ToList();

                        foreach (Player ply in players)
                            escape.Attach(ply);

                        response = $"{escape.Name} ({escape.Id}) attached to all players.";
                        ListPool<Player>.Pool.Return(players);
                        return true;
                    default:
                        if (Player.Get(identifier) is null)
                        {
                            response = $"Unable to find the player: {identifier}";
                            return false;
                        }

                        escape.Attach(Player.Get(identifier));
                        response = $"{escape.Name} ({escape.Id}) has been attached to {Player.Get(identifier).Nickname}.";
                        return true;
                }
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