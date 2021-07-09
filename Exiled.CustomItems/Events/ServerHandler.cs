// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems
{
    using System.Collections.Generic;
    using System.Text;

    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;

    using NorthwoodLib.Pools;

    /// <summary>
    /// Handles Server events for the CustomItems API.
    /// </summary>
    internal sealed class ServerHandler
    {
        /// <inheritdoc cref="Events.Handlers.Server.OnSendingRemoteAdminCommand"/>
        public void OnRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            if (ev.Name.ToLower() == "gban-kick" && ev.Sender.ReferenceHub.queryProcessor._sender.ServerRoles.RaEverywhere)
            {
                if (!(Player.Get(string.Join(" ", ev.Arguments)) is Player player))
                    return;

                if (!CustomItem.TryGet(player, out IEnumerable<CustomItem> customItems))
                    return;

                StringBuilder builder = StringBuilderPool.Shared.Rent();

                foreach (CustomItem item in customItems)
                {
                    if (item.ShouldMessageOnGban)
                    {
                        builder.Append(item.Name).Append(" - ").Append(item.Description).AppendLine();
                    }
                }

                string itemNames = StringBuilderPool.Shared.ToStringReturn(builder);

                if (!string.IsNullOrEmpty(itemNames))
                {
                    ev.Sender.SendConsoleMessage(
                        $"{player.Nickname} has been globally banned while in possession of the following items: {itemNames}\n" +
                        "Note: The creator(s) of these items have specifically flagged these items as doing something that may cause suspicion of hacking.",
                        "red");
                }
            }
        }
    }
}
