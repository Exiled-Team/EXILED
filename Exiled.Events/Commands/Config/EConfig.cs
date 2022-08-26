// -----------------------------------------------------------------------
// <copyright file="EConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Config
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The config command.
    /// </summary>
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class EConfig : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EConfig"/> class.
        /// </summary>
        public EConfig()
        {
            LoadGeneratedCommands();
        }

        /// <inheritdoc/>
        public override string Command { get; } = "econfig";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = new[] { "ecfg" };

        /// <inheritdoc/>
        public override string Description { get; } = "Changes from one config distribution to another one.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Merge.Instance);
            RegisterCommand(Split.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please, specify a valid subcommand! Available ones: merge, split";
            return false;
        }
    }
}