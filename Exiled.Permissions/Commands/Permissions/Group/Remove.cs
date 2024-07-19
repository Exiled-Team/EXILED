// -----------------------------------------------------------------------
// <copyright file="Remove.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Commands.Permissions.Group
{
    using System;

    using CommandSystem;

    using Extensions;

    /// <summary>
    /// Removes a group from a permission.
    /// </summary>
    public class Remove : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "remove";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "rmv" };

        /// <inheritdoc/>
        public string Description { get; } = "Removes a group from permission.";

        /// <inheritdoc cref="SanitizeResponse" />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ep.removegroup"))
            {
                response = "You can't add a new group, you don't have \"ep.removegroup\" permission.";
                return false;
            }

            if (arguments.Count == 1)
            {
                Permissions.Reload();

                if (!Permissions.Groups.ContainsKey(arguments.At(0)))
                {
                    response = $"Group {arguments.At(0)} does not exists.";
                    return false;
                }

                Permissions.Groups.Remove(arguments.At(0));
                Permissions.Save();

                Permissions.Reload();

                response = $"Group {arguments.At(0)} has been removed.";
                return true;
            }
            else
            {
                response = "EP GROUPS REMOVE <NAME>";
                return false;
            }
        }
    }
}