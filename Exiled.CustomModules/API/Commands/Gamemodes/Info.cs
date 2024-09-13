// -----------------------------------------------------------------------
// <copyright file="Info.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.GameModes
{
    using System;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.CustomModules.API.Features.CustomGameModes;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The command to retrieve a specific gamemode information.
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
        public string Description { get; } = "Gets the information of the specified gamemode.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            try
            {
                if (!sender.CheckPermission("gamemodes.info"))
                {
                    response = "Permission denied, gamemodes.info is required.";
                    return false;
                }

                if (arguments.Count < 1)
                {
                    response = "info [Gamemode ID]";
                    return false;
                }

                if ((!(uint.TryParse(arguments.At(0), out uint id) && CustomGameMode.TryGet(id, out CustomGameMode gameMode)) && !CustomGameMode.TryGet(arguments.At(0), out gameMode)) || gameMode is null)
                {
                    response = $"{arguments.At(0)} is not a valid Custom Gamemode.";
                    return false;
                }

                StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();
                builder.Append("<color=#E6AC00>-</color> <color=#00D639>").Append(gameMode.Name)
                    .Append("</color> <color=#05C4E8>(").Append(gameMode.Id).Append(")</color>")
                    .AppendLine("- Is Enabled: ").Append(gameMode.IsEnabled)
                    .AppendLine("- Can Start Automatically: ").Append(gameMode.CanStartAuto);

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