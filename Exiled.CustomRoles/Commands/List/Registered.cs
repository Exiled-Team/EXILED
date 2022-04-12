// -----------------------------------------------------------------------
// <copyright file="Registered.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands.List
{
    using System;
    using System.Linq;
    using System.Text;

    using CommandSystem;

    using Exiled.CustomRoles.API.Features;
    using Exiled.Permissions.Extensions;

    using NorthwoodLib.Pools;

    /// <inheritdoc />
    internal sealed class Registered : ICommand
    {
        private Registered()
        {
        }

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static Registered Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "registered";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "r" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets a list of registered custom roles.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customrols.list.registered"))
            {
                response = "Permission Denied, required: customroles.list.registered";
                return false;
            }

            if (CustomRoleBlueprint.Registered.Count == 0)
            {
                response = "There are no custom roles currently on this server.";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Shared.Rent().AppendLine();

            builder.Append("[Registered custom roles (").Append(CustomRoleBlueprint.Registered.Count).AppendLine(")]");

            foreach (CustomRoleBlueprint blueprint in CustomRoleBlueprint.Registered.OrderBy(r => r.Id))
                builder.Append('[').Append(blueprint.Id).Append(". ").Append(blueprint.Name).Append(" (").Append(blueprint.Role).Append(')').AppendLine("]");

            response = StringBuilderPool.Shared.ToStringReturn(builder);
            return true;
        }
    }
}
