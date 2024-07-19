// -----------------------------------------------------------------------
// <copyright file="Permissions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;

    using CommandSystem;
    using Exiled.API.Interfaces;
    using Exiled.Events.Handlers;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The reload permissions command.
    /// </summary>
    public class Permissions : ICommand, IPermissioned
    {
        /// <summary>
        /// Gets static instance of the <see cref="Permissions"/> command.
        /// </summary>
        public static Permissions Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "permissions";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "perms" };

        /// <inheritdoc/>
        public string Description { get; } = "Reload permissions.";

        /// <inheritdoc cref="SanitizeResponse" />
        public bool SanitizeResponse { get; }

        /// <inheritdoc />
        public string Permission { get; } = "ee.reloadpermissions";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Exiled.Permissions.Extensions.Permissions.Reload();
            Server.OnReloadedPermissions();

            response = "Permissions have been reloaded successfully!";
            return true;
        }
    }
}