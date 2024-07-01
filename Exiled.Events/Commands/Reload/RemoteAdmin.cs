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

    using Exiled.Permissions.Extensions;

    using Loader;

    /// <summary>
    /// The reload remoteadmin command.
    /// </summary>
    public class RemoteAdmin : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="RemoteAdmin"/> command.
        /// </summary>
        public static RemoteAdmin Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "remoteadmin";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "ra" };

        /// <inheritdoc/>
        public string Description { get; } = "Reloads remote admin configs.";

        /// <inheritdoc />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ee.reloadremoteadmin"))
            {
                response = "You can't reload remote admin configs, you don't have \"ee.reloadremoteadmin\" permission.";
                return false;
            }

            ConfigManager.ReloadRemoteAdmin();

            Handlers.Server.OnReloadedRA();

            response = "Remote admin configs reloaded.";
            return true;
        }
    }
}