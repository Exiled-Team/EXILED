// -----------------------------------------------------------------------
// <copyright file="Permissions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Commands.Permissions
{
    using System;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features.Pools;

    /// <summary>
    /// Handles commands about permissions.
    /// </summary>
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Permissions : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Permissions"/> class.
        /// </summary>
        public Permissions() => LoadGeneratedCommands();

        /// <inheritdoc/>
        public override string Command { get; } = "permissions";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = new[] { "ep" };

        /// <inheritdoc/>
        public override string Description { get; } = "Handles commands about permissions";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Reload());
            RegisterCommand(new Group.Group());
            RegisterCommand(new Add());
            RegisterCommand(new Remove());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder stringBuilder = StringBuilderPool.Pool.Get();

            stringBuilder.AppendLine("Available commands: ");
            stringBuilder.AppendLine("- EP RELOAD - Reloads permissions.");
            stringBuilder.AppendLine("- EP GROUP ADD <NAME> - Adds a group.");
            stringBuilder.AppendLine("- EP GROUP REMOVE <NAME> - Removes a group.");
            stringBuilder.AppendLine("- EP REMOVE <PERMISSION> <GROUP> - Adds a permission to a group.");
            stringBuilder.AppendLine("- EP ADD <PERMISSION> <GROUP> - Removes a permission from a group.");

            response = StringBuilderPool.Pool.ToStringReturn(stringBuilder);
            return false;
        }
    }
}