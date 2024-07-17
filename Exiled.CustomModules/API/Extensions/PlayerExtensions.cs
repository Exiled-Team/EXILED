// -----------------------------------------------------------------------
// <copyright file="PlayerExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Extensions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using PlayerRoles;

    /// <summary>
    /// A set of extensions for <see cref="Player"/>.
    /// </summary>
    public static class PlayerExtensions
    {
        /// <inheritdoc cref="CustomRole.Get(Pawn)"/>
        public static CustomRole Get(this Player player) => CustomRole.Get(player.Cast<Pawn>());

        /// <inheritdoc cref="CustomRole.Spawn(Pawn, CustomRole, bool, SpawnReason, RoleSpawnFlags)"/>
        public static bool Spawn(
            this Player player,
            CustomRole customRole,
            bool shouldKeepPosition = false,
            SpawnReason spawnReason = null,
            RoleSpawnFlags roleSpawnFlags = RoleSpawnFlags.All) =>
            CustomRole.Spawn(player.Cast<Pawn>(), customRole, shouldKeepPosition, spawnReason, roleSpawnFlags);

        /// <inheritdoc cref="CustomRole.Spawn{T}(Pawn)"/>
        public static bool Spawn<T>(this Player player)
            where T : CustomRole => CustomRole.Spawn<T>(player.Cast<Pawn>());

        /// <inheritdoc cref="CustomRole.Spawn(Pawn, string, bool, SpawnReason, RoleSpawnFlags)"/>
        public static bool Spawn(
            this Player player,
            string name,
            bool shouldKeepPosition = false,
            SpawnReason spawnReason = null,
            RoleSpawnFlags roleSpawnFlags = RoleSpawnFlags.All) =>
            CustomRole.Spawn(player.Cast<Pawn>(), name, shouldKeepPosition, spawnReason, roleSpawnFlags);

        /// <inheritdoc cref="CustomRole.Spawn(Pawn, uint, bool, SpawnReason, RoleSpawnFlags)"/>
        public static bool Spawn(
            this Player player,
            uint id,
            bool shouldKeepPosition = false,
            SpawnReason spawnReason = null,
            RoleSpawnFlags roleSpawnFlags = RoleSpawnFlags.All) =>
            CustomRole.Spawn(player.Cast<Pawn>(), id, shouldKeepPosition, spawnReason, roleSpawnFlags);

        /// <inheritdoc cref="CustomRole.TrySpawn(Pawn, CustomRole)"/>
        public static bool TrySpawn(this Player player, CustomRole customRole) => CustomRole.TrySpawn(player.Cast<Pawn>(), customRole);

        /// <inheritdoc cref="CustomRole.TrySpawn(Pawn, string)"/>
        public static bool TrySpawn(this Player player, string name) => CustomRole.TrySpawn(player.Cast<Pawn>(), name);

        /// <inheritdoc cref="CustomRole.TrySpawn(Pawn, uint)"/>
        public static bool TrySpawn(this Player player, uint id) => CustomRole.TrySpawn(player.Cast<Pawn>(), id);

        /// <inheritdoc cref="CustomRole.TryGet(Pawn, out CustomRole)"/>
        public static bool TryGet(this Player player, out CustomRole customRole) => CustomRole.TryGet(player.Cast<Pawn>(), out customRole);

        /// <inheritdoc cref="CustomItem.TryGive(Player, string, bool)"/>
        public static bool TryGive(this Player player, string name, bool displayMessage = true) => CustomItem.TryGive(player, name, displayMessage);

        /// <inheritdoc cref="CustomItem.TryGive(Player, uint, bool)"/>
        public static bool TryGive(this Player player, uint id, bool displayMessage = true) => CustomItem.TryGive(player, id, displayMessage);

        /// <inheritdoc cref="CustomItem.TryGive(Player, Type, bool)"/>
        public static bool TryGive(this Player player, Type type, bool displayMessage) => CustomItem.TryGive(player, type, displayMessage);
    }
}