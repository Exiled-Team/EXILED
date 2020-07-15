// -----------------------------------------------------------------------
// <copyright file="Plugins.cs" company="Exiled Team">
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
    /// The reload plugins command.
    /// </summary>
    public class Plugins : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "plugins";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { "pl" };

        /// <inheritdoc/>
        public string Description { get; } = "Reloads all plugins.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            sender.Respond("Reloading plugins...");

            Loader.LoadPlugins();

            response = "Plugins have been reloaded successfully!";

            return true;
        }
    }
}
