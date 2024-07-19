// -----------------------------------------------------------------------
// <copyright file="TpsCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands
{
    using System;

    using CommandSystem;
    using Exiled.API.Features;

    /// <summary>
    /// Command for showing current server TPS.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class TpsCommand : ICommand
    {
        /// <inheritdoc />
        public string Command { get; } = "tps";

        /// <inheritdoc />
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc />
        public string Description { get; } = "Shows the current TPS.";

        /// <inheritdoc cref="SanitizeResponse" />
        public bool SanitizeResponse { get; } = false;

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            double diff = Server.Tps / ServerStatic.ServerTickrate;
            string color = diff switch
            {
                > 0.9 => "green",
                > 0.5 => "yellow",
                _ => "red"
            };

            response = $"<color={color}>{Server.Tps}/{Server.MaxTps}</color>";
            return true;
        }
    }
}