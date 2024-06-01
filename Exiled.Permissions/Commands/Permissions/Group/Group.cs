// -----------------------------------------------------------------------
// <copyright file="Group.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Commands.Permissions.Group
{
    using System;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features.Pools;

    using Extensions;

    /// <summary>
    /// Handles commands about permissions groups.
    /// </summary>
    public class Group : ParentCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        public Group()
        {
            LoadGeneratedCommands();
        }

        /// <inheritdoc/>
        public override string Command { get; } = "groups";

        /// <inheritdoc/>
        public override string[] Aliases { get; } = new[] { "grps" };

        /// <inheritdoc/>
        public override string Description { get; } = "Handles commands about permissions groups.";

        /// <inheritdoc/>
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Add());
            RegisterCommand(new Remove());
        }

        /// <inheritdoc/>
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ep.groupinfo"))
            {
                response = "You can't see group information, you don't have \"ep.groupinfo\" permission.";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "- EP GROUPS <NAME>";
                return false;
            }

            if (!Permissions.Groups.ContainsKey(arguments.At(0)))
            {
                response = $"Group {arguments.At(0)} does not exist.";
                return false;
            }

            Permissions.Groups.TryGetValue(arguments.At(0), out Features.Group group);

            StringBuilder stringBuilder = StringBuilderPool.Pool.Get();

            stringBuilder.AppendLine($"Group: {arguments.At(0)}");

            if (group is null)
            {
                stringBuilder.AppendLine($"Group is null.");
                response = stringBuilder.ToString();
                return false;
            }

            stringBuilder.AppendLine($"Default: {group.IsDefault}");

            if (group.Inheritance.Count != 0)
            {
                stringBuilder.AppendLine("Inheritance: ");

                foreach (string inheritance in group.Inheritance)
                    stringBuilder.AppendLine("- " + inheritance);
            }

            if (group.Inheritance.Count != 0)
            {
                stringBuilder.AppendLine("Permissions: ");

                foreach (string permission in group.Permissions)
                    stringBuilder.AppendLine($"- {permission}");
            }

            response = StringBuilderPool.Pool.ToStringReturn(stringBuilder);
            return true;
        }
    }
}