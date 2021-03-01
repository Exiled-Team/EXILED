// -----------------------------------------------------------------------
// <copyright file="Spawned.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.Commands.List
{
    using System;
    using System.Text;

    using CommandSystem;

    using Exiled.CustomItems.API.Features;
    using Exiled.Permissions.Extensions;

    using NorthwoodLib.Pools;

    /// <inheritdoc/>
    internal sealed class Spawned : ICommand
    {
        private Spawned()
        {
        }

        /// <summary>
        /// Gets the command instance.
        /// </summary>
        public static Spawned Instance { get; } = new Spawned();

        /// <inheritdoc/>
        public string Command { get; } = "spawned";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "s", "sp" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets a list of spawned custom items.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("customitems.list.spawned"))
            {
                response = "Permission Denied, required: customitems.list.spawned";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "list spawned";
                return false;
            }

            StringBuilder message = StringBuilderPool.Shared.Rent();

            int count = 0;

            foreach (CustomItem customItem in CustomItem.Registered)
            {
                if (customItem.Spawned.Count == 0)
                    continue;

                message.AppendLine()
                    .Append('[').Append(customItem.Id).Append(". ").Append(customItem.Name).Append(" (").Append(customItem.Type).Append(')')
                    .Append(" {").Append(customItem.Spawned.Count).AppendLine("}]").AppendLine();

                count += customItem.Spawned.Count;

                foreach (Pickup spawned in customItem.Spawned)
                    message.AppendLine(spawned.position.ToString());
            }

            if (message.Length == 0)
                message.Append("There are no spawned custom items.");
            else
                message.Insert(0, Environment.NewLine + "[Spawned custom items (" + count + ")]" + Environment.NewLine);

            response = StringBuilderPool.Shared.ToStringReturn(message);
            return true;
        }
    }
}
