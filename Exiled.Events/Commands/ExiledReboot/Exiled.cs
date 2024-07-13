// -----------------------------------------------------------------------
// <copyright file="Exiled.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.ExiledReboot
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The Exiled Reboot parent command.
    /// </summary>
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Exiled : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Exiled"/> class.
        /// </summary>
        public Exiled()
        {
            LoadGeneratedCommands();
        }

        /// <inheritdoc/>
        public override string Command { get; } = "exiled";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = new[] { "ex" };

        /// <inheritdoc/>
        public override string Description { get; } = "Perform critical actions on behalf of Exiled Reboot.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Reboot.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Please, specify a valid subcommand! Available ones: reboot";
            return false;
        }
    }
}