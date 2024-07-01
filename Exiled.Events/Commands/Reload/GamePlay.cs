// -----------------------------------------------------------------------
// <copyright file="GamePlay.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;

    using CommandSystem;

    using Exiled.Permissions.Extensions;

    using static GameCore.ConfigFile;

    /// <summary>
    /// The reload gameplay command.
    /// </summary>
    public class GamePlay : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="GamePlay"/> command.
        /// </summary>
        public static GamePlay Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "gameplay";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "gm" };

        /// <inheritdoc/>
        public string Description { get; } = "Reloads gameplay configs.";

        /// <inheritdoc />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ee.reloadgameplay"))
            {
                response = "You can't reload gameplay configs, you don't have \"ee.reloadgameplay\" permission.";
                return false;
            }

            ReloadGameConfigs();

            Handlers.Server.OnReloadedGameplay();

            response = "Gameplay configs reloaded successfully!";
            return true;
        }
    }
}