// -----------------------------------------------------------------------
// <copyright file="Reboot.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.ExiledReboot
{
    using System;
    using System.IO;

    using CommandSystem;
    using global::Exiled.Loader;
    using MEC;
    using PluginAPI.Helpers;

    /// <summary>
    /// The Exiled Reboot command.
    /// </summary>
    public class Reboot : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="Reboot"/> command.
        /// </summary>
        public static Reboot Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "reboot";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "ack", "exboot" };

        /// <inheritdoc/>
        public string Description { get; } = "Enables Exiled Reboot.";

        /// <inheritdoc />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Exiled Reboot has been enabled. Logs will be suppressed until the next round. To permanently disable them, set the 'Reboot' config to true. Join our new Discord at discord.gg/exiledreboot for updates.";
            return true;
        }
    }
}