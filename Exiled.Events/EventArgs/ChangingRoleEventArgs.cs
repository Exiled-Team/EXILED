// -----------------------------------------------------------------------
// <copyright file="ChangingRoleEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;

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

            ConsolidatePlayerInformation();
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
        public List<ItemType> Items { get; } = new();

        /// <summary>
        /// Gets the base ammo values for the new role. (Changing this will overwrite their current inventory if Lite is true!).
        /// </summary>
        public Dictionary<ItemType, ushort> Ammo { get; } = new();

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

        /// <summary>
        /// Conslidates all the player information to be easily access through helper functions later on.
        /// </summary>
        private void ConsolidatePlayerInformation()
        {
            if (Player.InstantiatedRolesToPlayers.TryGetValue(Player.Role, out HashSet<Player> previousRolePlayers))
            {
                previousRolePlayers.Remove(Player);
            }

            if (Player.InstantiatedRolesToPlayers.TryGetValue(NewRole, out HashSet<Player> newRolePlayers))
            {
                newRolePlayers.Add(Player);
            }
            else
            {
                Player.InstantiatedRolesToPlayers.Add(NewRole, new HashSet<Player>() { Player });
            }

            if (Player.InstantiatedTeamToPlayers.TryGetValue(Player.Role.Team, out HashSet<Player> previousTeamPlayers))
            {
                previousTeamPlayers.Remove(Player);
            }

            if (Player.InstantiatedTeamToPlayers.TryGetValue(NewRole.GetTeam(), out HashSet<Player> newTeamPlayers))
            {
                newTeamPlayers.Add(Player);
            }
            else
            {
                Player.InstantiatedTeamToPlayers.Add(NewRole.GetTeam(), new HashSet<Player>() { Player });
            }

            if (Player.InstantiatedSideToPlayers.TryGetValue(Player.Role.Side, out HashSet<Player> previousSidePlayers))
            {
                previousSidePlayers.Remove(Player);
            }

            if (Player.InstantiatedSideToPlayers.TryGetValue(NewRole.GetSide(), out HashSet<Player> newSidePlayers))
            {
                newSidePlayers.Add(Player);
            }
            else
            {
                Player.InstantiatedSideToPlayers.Add(NewRole.GetSide(), new HashSet<Player>() { Player });
            }
        }
    }
}
