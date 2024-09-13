// -----------------------------------------------------------------------
// <copyright file="Start.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.GameModes
{
    using System;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Features.CustomGameModes;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The command to start a gamemode.
    /// </summary>
    internal sealed class Start : ICommand
    {
        private Start()
        {
        }

        /// <summary>
        /// Gets the <see cref="Start"/> command instance.
        /// </summary>
        public static Start Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "start";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "s" };

        /// <inheritdoc/>
        public string Description { get; } = "Starts the specified custom gamemode.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("gamemodes.start"))
                {
                    response = "Permission denied, gamemodes.start is required.";
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "start [Gamemode ID]";
                    return false;
                }

                if (CustomGameMode.TryGet(arguments.At(0), out CustomGameMode gameMode) && (!uint.TryParse(arguments.At(0), out uint id) || !CustomGameMode.TryGet(id, out gameMode)) && gameMode is null)
                {
                    response = $"Custom GameMode {arguments.At(0)} not found!";
                    return false;
                }

                World.Get().Start(gameMode, true);

                response = $"Started: <{gameMode.Id}> {gameMode.Name}.";
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