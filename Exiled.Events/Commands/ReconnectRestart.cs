// -----------------------------------------------------------------------
// <copyright file="ReconnectRestart.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands
{
    using System;

    using CommandSystem;

    using Exiled.API.Features;

    using MEC;

    using UnityEngine;

    /// <summary>
    /// The ReconnectRestart command.
    /// </summary>
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ReconnectRestart : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "reconnectrestart";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { "reconnectrs" };

        /// <inheritdoc/>
        public string Description { get; } = "Fakes the round restart and then restarts the server.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Round.Restart();

            Timing.CallDelayed(1.5f, Application.Quit);

            response = "The server is restarting...";

            return true;
        }
    }
}
