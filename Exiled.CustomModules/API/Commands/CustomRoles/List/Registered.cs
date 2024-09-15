// -----------------------------------------------------------------------
// <copyright file="Registered.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomRoles.List
{
    using System;
    using System.Linq;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.Permissions.Extensions;

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
            if (!sender.CheckPermission("customroles.list.registered"))
            {
                response = "Permission denied, customroles.list.registered is required.";
                return false;
            }

            if (!CustomRole.List.Any())
            {
                response = "There are no custom roles currently on this server.";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

            builder.Append("[Registered custom roles (").Append(CustomRole.List.Count()).AppendLine(")]");

            foreach (CustomRole role in CustomRole.List.OrderBy(r => r.Id))
                builder.Append('[').Append(role.Id).Append(". ").Append(role.Name).Append(" (").Append(role.Role).Append(')').AppendLine("]");

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}