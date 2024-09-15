// -----------------------------------------------------------------------
// <copyright file="Enqueue.cs" company="Exiled Team">
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
    /// The command to enqueue gamemodes.
    /// </summary>
    internal sealed class Enqueue : ICommand
    {
        private Enqueue()
        {
        }

        /// <summary>
        /// Gets the <see cref="Enqueue"/> command instance.
        /// </summary>
        public static Enqueue Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "enqueue";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "eq" };

        /// <inheritdoc/>
        public string Description { get; } = "Enqueues the specified gamemode.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("gamemodes.enqueue"))
                {
                    response = "Permission denied, gamemodes.enqueue is required.";
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "Usage: enqueue <Custom Gamemode>";
                    return false;
                }

                if (CustomGameMode.TryGet(arguments.At(0), out CustomGameMode gameMode) &&
                    (!uint.TryParse(arguments.At(0), out uint id) ||
                     !CustomGameMode.TryGet(id, out gameMode)) && gameMode is null)
                {
                    response = $"Custom gamemode {arguments.At(0)} not found!";
                    return false;
                }

                World.Get().EnqueueGameMode(gameMode);

                response = $"Enqueued: <{gameMode.Id}> {gameMode.Name}";
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