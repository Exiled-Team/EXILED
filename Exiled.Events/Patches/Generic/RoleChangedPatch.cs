// -----------------------------------------------------------------------
// <copyright file="RoleChangedPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes {
    using HarmonyLib;

    using InventorySystem;

    /// <summary>
    /// Patches <see cref="InventoryItemProvider.RoleChanged"/> to help override in <see cref="EventArgs.ChangingRoleEventArgs.Items"/> and <see cref="EventArgs.ChangingRoleEventArgs.Ammo"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryItemProvider), nameof(InventoryItemProvider.RoleChanged))]
    internal static class RoleChangedPatch {
        private static bool Prefix() => false;
    }
}
