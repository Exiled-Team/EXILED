// -----------------------------------------------------------------------
// <copyright file="ConfigValue.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.ConfigValue
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The config value command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class ConfigValue : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigValue"/> class.
        /// </summary>
        public ConfigValue()
        {
            LoadGeneratedCommands();
        }

        /// <inheritdoc/>
        public override string Command { get; } = "config_value";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = { "value", "cv", "cfgval" };

        /// <inheritdoc/>
        public override string Description { get; } = "Gets or sets a config value";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Get.Instance);
            RegisterCommand(Set.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please, specify a valid subcommand! Available ones: get, set";
            return false;
        }
    }
}