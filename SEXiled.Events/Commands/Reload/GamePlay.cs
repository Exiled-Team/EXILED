// -----------------------------------------------------------------------
// <copyright file="GamePlay.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Commands.Reload
{
    using System;

    using CommandSystem;

    using SEXiled.Permissions.Extensions;

    using static GameCore.ConfigFile;

    /// <summary>
    /// The reload gameplay command.
    /// </summary>
    public class GamePlay : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="GamePlay"/> command.
        /// </summary>
        public static GamePlay Instance { get; } = new GamePlay();

        /// <inheritdoc/>
        public string Command { get; } = "gameplay";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { "gm" };

        /// <inheritdoc/>
        public string Description { get; } = "Reloads gameplay configs.";

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
