// -----------------------------------------------------------------------
// <copyright file="Registered.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Commands.GameModes.List
{
    using System;
    using System.Linq;
    using System.Text;

    using CommandSystem;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.CustomModules.API.Features.CustomGameModes;
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
        public string Description { get; } = "Gets a list of registered gamemodes.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("gamemodes.list.registered"))
            {
                response = "Permission denied, gamemodes.list.registered is required.";
                return false;
            }

            if (!CustomGameMode.List.Any())
            {
                response = "There are no gamemodes currently on this server.";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Pool.Get().AppendLine();

            builder.Append("[Registered gamemodes (").Append(CustomGameMode.List.Count()).AppendLine(")]");

            foreach (CustomGameMode gameMode in CustomGameMode.List.OrderBy(r => r.Id))
                builder.Append('[').Append(gameMode.Id).Append(". ").Append(gameMode.Name).AppendLine("]");

            response = StringBuilderPool.Pool.ToStringReturn(builder);
            return true;
        }
    }
}