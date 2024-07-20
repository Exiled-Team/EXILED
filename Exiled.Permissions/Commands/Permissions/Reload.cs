// -----------------------------------------------------------------------
// <copyright file="Reload.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Commands.Permissions
{
    using System;

    using CommandSystem;

    using Extensions;

    /// <summary>
    /// Reloads all permissions.
    /// </summary>
    public class Reload : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "reload";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "rld" };

        /// <inheritdoc/>
        public string Description { get; } = "Reloads all permissions";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ep.reload"))
            {
                response = "You can't reload permissions, you don't have \"ep.reload\" permission.";
                return false;
            }

            Extensions.Permissions.Reload();

            response = "Permissions have been successfully reloaded!";
            return true;
        }
    }
}