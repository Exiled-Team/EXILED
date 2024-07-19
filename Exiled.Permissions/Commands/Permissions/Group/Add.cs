// -----------------------------------------------------------------------
// <copyright file="Add.cs" company="Exiled Team">
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
    /// Adds a group to a permission.
    /// </summary>
    public class Add : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "add";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { };

        /// <inheritdoc/>
        public string Description { get; } = "Adds a group to a permission.";

        /// <inheritdoc cref="SanitizeResponse" />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ep.addgroup"))
            {
                response = "You can't add a new group, you don't have \"ep.addgroup\" permission.";
                return false;
            }

            if (arguments.Count == 1)
            {
                Permissions.Reload();

                if (Permissions.Groups.ContainsKey(arguments.At(0)))
                {
                    response = $"Group {arguments.At(0)} already exists.";
                    return false;
                }

                Permissions.Groups.Add(arguments.At(0), new Features.Group());
                Permissions.Save();

                response = $"Group {arguments.At(0)} has been added.";
                return true;
            }
            else
            {
                response = "EP GROUPS ADD <NAME>";
                return false;
            }
        }
    }
}