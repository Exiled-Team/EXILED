// -----------------------------------------------------------------------
// <copyright file="List.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomEscapes.List
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The command to list all registered custom escapes.
    /// </summary>
    internal sealed class List : ParentCommand
    {
        private List()
        {
            LoadGeneratedCommands();
        }

        /// <summary>
        /// Gets the <see cref="List"/> command instance.
        /// </summary>
        public static List Instance { get; } = new();

        /// <inheritdoc/>
        public override string Command { get; } = "list";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = { "l" };

        /// <inheritdoc/>
        public override string Description { get; } = "Gets a list of all currently registered custom escapes.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Registered.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Invalid subcommand! Available: registered.";
            return false;
        }
    }
}