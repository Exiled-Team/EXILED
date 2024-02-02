// -----------------------------------------------------------------------
// <copyright file="OwnerEscapingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomItems
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.CustomModules.API.Features.CustomItems.Items;
    using Exiled.Events.EventArgs.Player;

    using PlayerRoles;

    using Respawning;

    /// <summary>
    /// Contains all information of a <see cref="API.Features.CustomItems.CustomItem"/>  before a <see cref="Player"/> escapes.
    /// </summary>
    public class OwnerEscapingEventArgs : EscapingEventArgs, ICustomItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerEscapingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="customItem"><inheritdoc cref="CustomItem"/></param>
        /// <param name="itemBehaviour"><inheritdoc cref="ItemBehaviour"/></param>
        /// <param name="ev">The <see cref="EscapingEventArgs"/> instance.</param>
        public OwnerEscapingEventArgs(Item item, CustomItem customItem, ItemBehaviour itemBehaviour, EscapingEventArgs ev)
            : this(item, customItem, itemBehaviour, ev.Player, ev.NewRole, ev.EscapeScenario, ev.RespawnTickets)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerEscapingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="customItem"><inheritdoc cref="CustomItem"/></param>
        /// <param name="itemBehaviour"><inheritdoc cref="ItemBehaviour"/></param>
        /// <param name="player"><inheritdoc cref="EscapingEventArgs.Player"/></param>
        /// <param name="newRole"><inheritdoc cref="EscapingEventArgs.NewRole"/></param>
        /// <param name="escapeScenario"><inheritdoc cref="EscapingEventArgs.EscapeScenario"/></param>
        /// <param name="respawnTickets"><inheritdoc cref="EscapingEventArgs.RespawnTickets"/></param>
        public OwnerEscapingEventArgs(Item item, CustomItem customItem, ItemBehaviour itemBehaviour, Player player, RoleTypeId newRole, EscapeScenario escapeScenario, KeyValuePair<SpawnableTeamType, float> respawnTickets = default)
            : base(player, newRole, escapeScenario, respawnTickets)
        {
            Item = item;
            CustomItem = customItem;
            ItemBehaviour = itemBehaviour;
        }

        /// <inheritdoc/>
        public Item Item { get; }

        /// <inheritdoc/>
        public CustomItem CustomItem { get; }

        /// <inheritdoc/>
        public ItemBehaviour ItemBehaviour { get; }
    }
}