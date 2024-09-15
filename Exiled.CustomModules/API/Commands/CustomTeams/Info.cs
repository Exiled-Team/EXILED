// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomTeams
{
    using System;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The command to retrieve a specific custom team information.
    /// </summary>
    internal sealed class Info : ICommand
    {
        private Info()
        {
        }

        /// <summary>
        /// Gets the <see cref="Info"/> command instance.
        /// </summary>
        public static Info Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "info";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "i" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets the information of the specified custom team.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("customteams.info"))
                {
                    response = "Permission denied, customteams.info is required.";
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "info <Custom Team>";
                    return false;
                }

                if ((!(uint.TryParse(arguments.At(0), out uint id) && CustomTeam.TryGet(id, out CustomTeam team)) && !CustomTeam.TryGet(arguments.At(0), out team)) || team is null)
                {
                    response = $"{arguments.At(0)} is not a valid custom team.";
                    return false;
                }

                StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

                builder.Append("<color=#E6AC00>-</color> <color=#00D639>").Append(team.Name)
                    .Append("</color> <color=#05C4E8>(").Append(team.Id).Append(")</color>")
                    .AppendLine("- Is Enabled: ").Append(team.IsEnabled)
                    .AppendLine("- Probability: ").Append(team.Probability)
                    .AppendLine("- Display Color: ").Append(team.DisplayColor)
                    .AppendLine("- Size: ").Append(team.Size);

                response = StringBuilderPool.Pool.ToStringReturn(builder);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                response = "An error occurred when executing the command, check server console for more details.";
                return false;
            }
        }
    }
}