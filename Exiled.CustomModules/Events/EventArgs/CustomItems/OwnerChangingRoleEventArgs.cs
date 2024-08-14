// -----------------------------------------------------------------------
// <copyright file="OwnerChangingRoleEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomItems
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.Events.EventArgs.Player;
    using PlayerRoles;

    /// <summary>
    /// Contains all information of a <see cref="API.Features.CustomItems.CustomItem"/> before a <see cref="Player"/> changes role.
    /// </summary>
    public class OwnerChangingRoleEventArgs : ChangingRoleEventArgs, ICustomItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerChangingRoleEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="customItem"><inheritdoc cref="CustomItem"/></param>
        /// <param name="itemBehaviour"><inheritdoc cref="ItemBehaviour"/></param>
        /// <param name="ev">The <see cref="ChangingRoleEventArgs"/> instance.</param>
        public OwnerChangingRoleEventArgs(Item item, CustomItem customItem, ItemBehaviour itemBehaviour, ChangingRoleEventArgs ev)
            : this(item, customItem, itemBehaviour, ev.Player, ev.NewRole, ev.ShouldPreserveInventory, (RoleChangeReason)ev.Reason)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerChangingRoleEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="customItem"><inheritdoc cref="CustomItem"/></param>
        /// <param name="itemBehaviour"><inheritdoc cref="ItemBehaviour"/></param>
        /// <param name="player"><inheritdoc cref="ChangingRoleEventArgs.Player"/></param>
        /// <param name="newRole"><inheritdoc cref="ChangingRoleEventArgs.NewRole"/></param>
        /// <param name="shouldPreserveInventory"><inheritdoc cref="ChangingRoleEventArgs.ShouldPreserveInventory"/></param>
        /// <param name="reason"><inheritdoc cref="ChangingRoleEventArgs.Reason"/></param>
        public OwnerChangingRoleEventArgs(Item item, CustomItem customItem, ItemBehaviour itemBehaviour, Player player, RoleTypeId newRole, bool shouldPreserveInventory, RoleChangeReason reason)
            : base(player, newRole, reason, RoleSpawnFlags.All)
        {
            if (item is null)
                Log.Debug("Item is null", true);

            if (player is null)
                Log.Debug("Player is null", true);

            Item = item;
            CustomItem = customItem;
            ItemBehaviour = itemBehaviour;
            ShouldPreserveInventory = shouldPreserveInventory;
        }

        /// <inheritdoc/>
        public Item Item { get; }

        /// <inheritdoc/>
        public CustomItem CustomItem { get; }

        /// <inheritdoc/>
        public ItemBehaviour ItemBehaviour { get; }
    }
}