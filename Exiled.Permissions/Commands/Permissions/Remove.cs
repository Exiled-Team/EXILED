// -----------------------------------------------------------------------
// <copyright file="Remove.cs" company="Exiled Team">
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
    /// Removes a permission from a group.
    /// </summary>
    public class Remove : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "remove";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "rmv" };

        /// <inheritdoc/>
        public string Description { get; } = "Adds a permission to a group";

        /// <inheritdoc />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ep.removepermission"))
            {
                response = "You can't remove a permission, you don't have \"ep.removepermission\" permission.";
                return false;
            }

            if (arguments.Count != 2)
            {
                response = "EP REMOVE <PERMISSION> <GROUP>";
                return false;
            }

            Extensions.Permissions.Reload();

            if (!Extensions.Permissions.Groups.ContainsKey(arguments.At(1)))
            {
                response = $"Group {arguments.At(1)} does not exist.";
                return false;
            }

            Extensions.Permissions.Groups.TryGetValue(arguments.At(1), out Features.Group group);

            if (group.Permissions.Contains(arguments.At(0)))
            {
                group.Permissions.Remove(arguments.At(0));
            }
            else
            {
                response = $"Permission {arguments.At(0)} doesn't exist!";
                return false;
            }

            Extensions.Permissions.Save();
            Extensions.Permissions.Reload();

            response = $"Permission {arguments.At(0)} for group {arguments.At(1)} removed successfully!.";
            return true;
        }
    }
}