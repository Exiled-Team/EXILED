// -----------------------------------------------------------------------
// <copyright file="Reload.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;
    using CommandSystem;

    /// <summary>
    /// The reload command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Reload : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reload"/> class.
        /// </summary>
        public Reload() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command { get; } = "reload";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = new string[] { "rld" };

        /// <inheritdoc/>
        public override string Description { get; } = "Reload plugins, plugin configs, gameplay and remote admin configs.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Configs());
            RegisterCommand(new Plugins());
            RegisterCommand(new GamePlay());
            RegisterCommand(new RemoteAdmin());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please, specify a valid subcommand! Available ones: plugins, gameplay, pluginconfigs, remoteadminconfigs";
            return false;
        }
    }
}
