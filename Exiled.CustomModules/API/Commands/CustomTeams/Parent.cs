// -----------------------------------------------------------------------
// <copyright file="Parent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomTeams
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The main parent command for custom teams.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    internal sealed class Parent : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parent"/> class.
        /// </summary>
        public Parent()
        {
            LoadGeneratedCommands();
        }

        /// <inheritdoc/>
        public override string Command { get; } = "teams";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = { "tms" };

        /// <inheritdoc/>
        public override string Description { get; } = string.Empty;

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Spawn.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand! Available: ";
            return false;
        }
    }
}