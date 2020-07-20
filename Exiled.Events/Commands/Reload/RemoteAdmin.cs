// -----------------------------------------------------------------------
// <copyright file="RemoteAdmin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;

    using CommandSystem;

    using Exiled.Loader;

    /// <summary>
    /// The reload remoteadmin command.
    /// </summary>
    public class RemoteAdmin : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "remoteadmin";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { "ra" };

        /// <inheritdoc/>
        public string Description { get; } = "Reloads remote admin configs.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            ConfigManager.ReloadRemoteAdmin();

            Handlers.Server.OnReloadedRA();

            response = "Remote admin configs reloaded.";

            return true;
        }
    }
}
