// -----------------------------------------------------------------------
// <copyright file="Configs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;
    using CommandSystem;
    using Exiled.Loader;

    /// <summary>
    /// The reload configs command.
    /// </summary>
    public class Configs : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "configs";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { "cfgs" };

        /// <inheritdoc/>
        public string Description { get; } = "Reload plugin configs.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool haveBeenReloaded = ConfigManager.Reload();

            response = "Plugin configs have been reloaded successfully!";

            return haveBeenReloaded;
        }
    }
}
