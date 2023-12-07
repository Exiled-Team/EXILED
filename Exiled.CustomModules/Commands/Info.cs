// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Commands
{
    using System;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features.Pools;
    using Exiled.CustomModules.API.Features;
    using Exiled.Permissions.Extensions;

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
        public string Description { get; } = "Gets more information about the specified custom role.";

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

            if ((!(uint.TryParse(arguments.At(0), out uint id) && CustomRole.TryGet(id, out CustomRole? role)) &&
                !CustomRole.TryGet(arguments.At(0), out role)) || role is null)
            {
                response = $"{arguments.At(0)} is not a valid custom role.";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

            builder.Append("<color=#E6AC00>-</color> <color=#00D639>").Append(role.Name)
                .Append("</color> <color=#05C4E8>(").Append(role.Id).Append(")</color>")
                .Append("- ").AppendLine(role.Description)
                .AppendLine(role.Role.ToString())
                .Append("- Health: ").AppendLine(role.Settings.MaxHealth.ToString()).AppendLine();

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}