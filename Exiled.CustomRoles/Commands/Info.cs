// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands
{
    using System;
    using System.Text;

    using CommandSystem;

    using Exiled.CustomRoles.API.Features;
    using Exiled.Permissions.Extensions;

    using NorthwoodLib.Pools;

    /// <summary>
    /// The command to view info about a specific role.
    /// </summary>
    internal sealed class Info : ICommand
    {
        private Info()
        {
        }

        /// <summary>
        /// Gets the <see cref="Info"/> instance.
        /// </summary>
        public static Info Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "info";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "i" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets more information about the specified custom item.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customroles.info"))
            {
                response = "Permission Denied, required: customroles.info";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "info [Custom role name/Custom role ID]";
                return false;
            }

            if (!(int.TryParse(arguments.At(0), out int id) && CustomRoleBlueprint.TryGet(id, out CustomRoleBlueprint blueprint)) &&
                !CustomRoleBlueprint.TryGet(arguments.At(0), out blueprint))
            {
                response = $"{arguments.At(0)} is not a valid custom role.";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Shared.Rent().AppendLine();
            builder.Append("<color=#E6AC00>-</color> <color=#00D639>").Append(blueprint.Name)
                .Append("</color> <color=#05C4E8>(").Append(blueprint.Id).Append(")</color>")
                .Append("- ").AppendLine(blueprint.Description)
                .AppendLine(blueprint.Role.ToString())
                .Append("- Health: ").AppendLine(blueprint.MaxHealth.ToString()).AppendLine();

            response = StringBuilderPool.Shared.ToStringReturn(builder);
            return true;
        }
    }
}
