// -----------------------------------------------------------------------
// <copyright file="PluginManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.PluginManager
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The plugin manager.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PluginManager : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginManager"/> class.
        /// </summary>
        public PluginManager()
        {
            LoadGeneratedCommands();
        }

        /// <inheritdoc/>
        public override string Command { get; } = "pluginmanager";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = new[] { "plymanager", "plmanager", "pmanager", "plym" };

        /// <inheritdoc/>
        public override string Description { get; } = "Manage plugin. Enable, disable and show plugins.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Show.Instance);
            RegisterCommand(Enable.Instance);
            RegisterCommand(Disable.Instance);
            RegisterCommand(Patches.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please, specify a valid subcommand! Available ones: enable, disable, show";
            return false;
        }
    }
}
