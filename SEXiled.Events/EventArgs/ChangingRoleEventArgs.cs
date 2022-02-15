// -----------------------------------------------------------------------
// <copyright file="ChangingRoleEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;

    using SEXiled.API.Enums;
    using SEXiled.API.Features;

    /// <summary>
    /// Contains all information before a player's <see cref="RoleType"/> changes.
    /// </summary>
    public class ChangingRoleEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangingRoleEventArgs"/> class.
        /// </summary>
        /// <param name="player"><inheritdoc cref="Player"/></param>
        /// <param name="newRole"><inheritdoc cref="NewRole"/></param>
        /// <param name="shouldPreservePosition"><inheritdoc cref="Lite"/></param>
        /// <param name="reason"><inheritdoc cref="Reason"/></param>
        public ChangingRoleEventArgs(Player player, RoleType newRole, bool shouldPreservePosition, CharacterClassManager.SpawnReason reason)
        {
            Player = player;
            NewRole = newRole;
            if (InventorySystem.Configs.StartingInventories.DefinedInventories.ContainsKey(newRole))
            {
                foreach (ItemType itemType in InventorySystem.Configs.StartingInventories.DefinedInventories[newRole].Items)
                    Items.Add(itemType);
                foreach (KeyValuePair<ItemType, ushort> ammoPair in InventorySystem.Configs.StartingInventories.DefinedInventories[newRole].Ammo)
                    Ammo.Add(ammoPair.Key, ammoPair.Value);
            }

            Lite = shouldPreservePosition;
            Reason = (SpawnReason)reason;
        }

        /// <summary>
        /// Gets the player whose <see cref="RoleType"/> is changing.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        /// Gets or sets the new player's role.
        /// </summary>
        public RoleType NewRole { get; set; }

        /// <summary>
        /// Gets base items that the player will receive. (Changing this will overwrite their current inventory if Lite is true!).
        /// </summary>
        public List<ItemType> Items { get; } = new List<ItemType>();

        /// <summary>
        /// Gets the base ammo values for the new role. (Changing this will overwrite their current inventory if Lite is true!).
        /// </summary>
        public Dictionary<ItemType, ushort> Ammo { get; } = new Dictionary<ItemType, ushort>();

        /// <summary>
        /// Gets or sets the reason for their class change.
        /// </summary>
        public SpawnReason Reason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the position and items has to be preserved after changing the role.
        /// </summary>
        public bool Lite { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can continue.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
