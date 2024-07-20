// -----------------------------------------------------------------------
// <copyright file="Registered.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands.List
{
    using System;
    using System.Linq;
    using System.Text;

    using CommandSystem;

    using Exiled.API.Features.Pools;
    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    /// <inheritdoc/>
    internal sealed class Registered : ICommand
    {
        private Registered()
        {
        }

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static Registered Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "registered";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "r", "reg" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets a list of registered custom items.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.list.registered"))
            {
                response = "Permission Denied, required: customitems.list.registered";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "list registered";
                return false;
            }

            if (CustomItem.Registered.Count == 0)
            {
                response = "There are no custom items currently on this server.";
                return false;
            }

            StringBuilder message = StringBuilderPool.Pool.Get().AppendLine();

            message.Append("[Registered custom items (").Append(CustomItem.Registered.Count).AppendLine(")]");

            foreach (CustomItem customItem in CustomItem.Registered.OrderBy(item => item.Id))
                message.Append('[').Append(customItem.Id).Append(". ").Append(customItem.Name).Append(" (").Append(customItem.Type).Append(')').AppendLine("]");

            response = StringBuilderPool.Pool.ToStringReturn(message);
            return true;
        }
    }
}