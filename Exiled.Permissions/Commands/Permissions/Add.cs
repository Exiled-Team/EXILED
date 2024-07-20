// -----------------------------------------------------------------------
// <copyright file="Add.cs" company="Exiled Team">
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
    /// Adds a permission to a group.
    /// </summary>
    public class Add : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "add";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { };

        /// <inheritdoc/>
        public string Description { get; } = "Adds a permission to a group";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ep.addpermission"))
            {
                response = "You can't add a permission, you don't have \"ep.addpermission\" permission.";
                return false;
            }

            if (arguments.Count != 2)
            {
                response = "EP ADD <PERMISSION> <GROUP>";
                return false;
            }

            Extensions.Permissions.Reload();

            if (!Extensions.Permissions.Groups.ContainsKey(arguments.At(1)))
            {
                response = $"Group {arguments.At(1)} does not exist.";
                return false;
            }

            Extensions.Permissions.Groups.TryGetValue(arguments.At(1), out Features.Group group);

            if (!group.Permissions.Contains(arguments.At(0)))
            {
                group.Permissions.Add(arguments.At(0));
            }
            else
            {
                response = $"Permission {arguments.At(0)} already exists!";
                return false;
            }

            Extensions.Permissions.Save();
            Extensions.Permissions.Reload();

            response = $"Permission {arguments.At(0)} for group {arguments.At(1)} added successfully!.";
            return true;
        }
    }
}