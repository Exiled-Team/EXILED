// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomRoles
{
    using System;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.Permissions.Extensions;
    using PlayerRoles;

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
                response = "Permission denied, customroles.info is required.";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "info <Custom Role>";
                return false;
            }

            if ((!(uint.TryParse(arguments.At(0), out uint id) && CustomRole.TryGet(id, out CustomRole role)) && !CustomRole.TryGet(arguments.At(0), out role)) || role is null)
            {
                response = $"{arguments.At(0)} is not a valid custom role.";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

            builder.Append("<color=#E6AC00>-</color> <color=#00D639>").Append(role.Name)
                .Append("</color> <color=#05C4E8>(").Append(role.Id).Append(")</color>")
                .AppendLine("- Probability: ").Append(role.Probability)
                .Append("- ").AppendLine(role.Description)
                .AppendLine(role.Role.ToString())
                .Append("- Health: ").AppendLine(role.Settings.MaxHealth.ToString())
                .AppendLine("- Team: ");
            foreach (Team team in role.TeamsOwnership)
                builder.AppendLine(team.ToString());

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}