// -----------------------------------------------------------------------
// <copyright file="Abilities.cs" company="Exiled Team">
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

    using Exiled.API.Features.Pools;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Permissions.Extensions;

    /// <inheritdoc />
    internal sealed class Abilities : ICommand
    {
        /// <inheritdoc />
        public string Command => "abilities";

        /// <inheritdoc />
        public string[] Aliases => new[] { "a" };

        /// <inheritdoc />
        public string Description => "Lists all abilities on the server.";

        /// <inheritdoc />
        public bool SanitizeResponse { get; }

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customroles.list.abilities"))
            {
                response = "Permission Denied, required: customroles.list.abilities";
                return false;
            }

            if (CustomAbility.Registered.Count == 0)
            {
                response = "There are no custom abilities currently on this server.";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

            builder.Append("[Registered custom roles (").Append(CustomRole.Registered.Count).AppendLine(")]");

            foreach (CustomAbility ability in CustomAbility.Registered.OrderBy(r => r.Name))
                builder.Append('[').Append(ability.Name).Append(" (").Append(ability.Description).Append(')').AppendLine("]");

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}