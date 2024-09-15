// -----------------------------------------------------------------------
// <copyright file="End.cs" company="Exiled Team">
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
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The command to end a gamemode.
    /// </summary>
    internal sealed class End : ICommand
    {
        private End()
        {
        }

        /// <summary>
        /// Gets the <see cref="End"/> command instance.
        /// </summary>
        public static End Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "end";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "e" };

        /// <inheritdoc/>
        public string Description { get; } = "Ends the running gamemode.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("gamemodes.end"))
                {
                    response = "Permission denied, gamemodes.end is required.";
                    return false;
                }

                World world = World.Get();

                if (world.GameState)
                {
                    world.GameState.End(true);

                    response = "The gamemode has been ended.";
                    return true;
                }

                response = "There is no gamemode running.";
                return false;
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