// -----------------------------------------------------------------------
// <copyright file="List.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomItem.List
{
    using System;

    using CommandSystem;

    /// <summary>
    /// The command to list all installed items.
    /// </summary>
    internal sealed class List : ParentCommand
    {
        private List()
        {
            LoadGeneratedCommands();
        }

        /// <summary>
        /// Gets the <see cref="Info"/> instance.
        /// </summary>
        public static List Instance { get; } = new();

        /// <inheritdoc/>
        public override string Command { get; } = "list";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = { "s", "l", "show", "sh" };

        /// <inheritdoc/>
        public override string Description { get; } = "Gets a list of all currently registered custom items.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Registered.Instance);
            RegisterCommand(Tracked.Instance);
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"Invalid subcommand! Available: registered, insideinventories";
            return false;
        }
    }
}