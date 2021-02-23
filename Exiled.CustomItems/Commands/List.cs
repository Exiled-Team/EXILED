// -----------------------------------------------------------------------
// <copyright file="List.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands
{
    using System;
    using System.Text;

    using CommandSystem;

    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    using NorthwoodLib.Pools;

    /// <summary>
    /// The command to list all installed items.
    /// </summary>
    internal sealed class List : ICommand
    {
        private List()
        {
        }

        /// <summary>
        /// Gets the <see cref="Info"/> instance.
        /// </summary>
        public static List Instance { get; } = new List();

        /// <inheritdoc/>
        public string Command { get; } = "list";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "s", "l", "show", "sh" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets a list of all currently registered custom items.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.list"))
            {
                response = "Permission Denied, required: customitems.list";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "list";
                return false;
            }

            if (CustomItems.Instance.ItemManagers.Count == 0)
            {
                response = "There are no custom items currently on this server.";
                return false;
            }

            StringBuilder message = StringBuilderPool.Shared.Rent().AppendLine();

            message.Append("[CUSTOM ITEMS (").Append(CustomItems.Instance.ItemManagers).Append(")]").AppendLine();

            foreach (CustomItem item in CustomItems.Instance.ItemManagers)
                message.Append(item.Name).Append(" (").Append(item.Id).AppendLine();

            response = StringBuilderPool.Shared.ToStringReturn(message);
            return true;
        }
    }
}
