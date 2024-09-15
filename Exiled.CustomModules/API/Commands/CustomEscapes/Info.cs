// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.CustomEscapes
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.CustomModules.API.Features.CustomEscapes;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The command to retrieve a specific custom escape information.
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
        public string Description { get; } = "Gets the information of the specified custom escape.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("customescapes.info"))
                {
                    response = "Permission denied, customescapes.info is required.";
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "info [Custom Escape ID]";
                    return false;
                }

                if ((!(uint.TryParse(arguments.At(0), out uint id) && CustomEscape.TryGet(id, out CustomEscape escape)) && !CustomEscape.TryGet(arguments.At(0), out escape)) || escape is null)
                {
                    response = $"{arguments.At(0)} is not a valid Custom Escape.";
                    return false;
                }

                StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

                builder.Append("<color=#E6AC00>-</color> <color=#00D639>").Append(escape.Name)
                    .Append("</color> <color=#05C4E8>(").Append(escape.Id).Append(")</color>")
                    .AppendLine("- Is Enabled: ").Append(escape.IsEnabled)
                    .AppendLine("- Attached To: ");

                foreach (KeyValuePair<Player, CustomEscape> managers in CustomEscape.Manager)
                {
                    if (managers.Value != escape)
                        continue;

                    builder.Append("  - ").Append(managers.Key.Nickname).Append(" (").Append(managers.Key.UserId).Append(")").AppendLine();
                }

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