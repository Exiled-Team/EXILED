// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Commands.Team
{
    using System;
    using System.Linq;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features.Pools;
    using Exiled.CustomRoles.API.Features;
    using Exiled.Permissions.Extensions;
    using Utils.NonAllocLINQ;

    /// <summary>
    /// The command to view info about a specific team.
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
        public string Description { get; } = "Gets more information about the specified custom team.";

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
                response = "info [Custom team name/Custom team ID]";
                return false;
            }

            if ((!(uint.TryParse(arguments.At(0), out uint id) && CustomTeam.TryGet(id, out CustomTeam? team)) && !CustomTeam.TryGet(arguments.At(0), out team)) || team is null)
            {
                response = $"{arguments.At(0)} is not a valid custom team.";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

            builder.Append("<color=#E6AC00>-</color> <color=#00D639>").Append(team.Name)
                .Append("</color> <color=#05C4E8>(").Append(team.Id).Append(")</color>")
                .Append("- ").AppendLine(team.Description);
            team.Roles.ForEach(role => builder.AppendLine($"{role.Name} ({role.Id})"));
            builder.Append("- MaxRespawn: ").AppendLine(team.ResapwnAmount.ToString()).AppendLine();

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}