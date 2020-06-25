// -----------------------------------------------------------------------
// <copyright file="Permissions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions
{
    using System;
    using Exiled.API.Features;
    using Exiled.Permissions.Events;

    /// <summary>
    /// Handles all plugin-related permissions, for executing commands, doing actions and so on.
    /// </summary>
    public sealed class Permissions : Plugin<Config>
    {
        private static readonly Lazy<Permissions> LazyInstance = new Lazy<Permissions>(() => new Permissions());
        private Command command;

        private Permissions()
        {
        }

        /// <summary>
        /// Gets the permissions instance.
        /// </summary>
        public static Permissions Instance => LazyInstance.Value;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            command = new Command();

            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += command.OnSendingRemoteAdminCommand;

            Extensions.Permissions.Create();
            Extensions.Permissions.Reload();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Extensions.Permissions.Groups.Clear();

            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand -= command.OnSendingRemoteAdminCommand;

            command = null;
        }

        /// <inheritdoc/>
        public override void OnReloaded()
        {
        }
    }
}
