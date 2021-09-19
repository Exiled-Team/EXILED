// -----------------------------------------------------------------------
// <copyright file="RoleChangedIgnoresLite.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using HarmonyLib;

    using InventorySystem;

    /// <summary>
    /// Patches <see cref="InventoryItemProvider.RoleChanged"/> to make it no longer ignore the 'lite' bool.
    /// </summary>
    [HarmonyPatch(typeof(InventoryItemProvider), nameof(InventoryItemProvider.RoleChanged))]
    internal static class RoleChangedIgnoresLite
    {
        private static bool Prefix(ReferenceHub ply, RoleType prevRole, RoleType newRole, bool lite, CharacterClassManager.SpawnReason spawnReason) => !lite;
    }
}
