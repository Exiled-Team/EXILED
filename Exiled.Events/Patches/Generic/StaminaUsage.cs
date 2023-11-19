// -----------------------------------------------------------------------
// <copyright file="StaminaUsage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313

    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem;

    /// <summary>
    /// Patches <see cref="Inventory.StaminaUsageMultiplier"/>.
    /// Implements <see cref="Player.IsUsingStamina"/> and <see cref="Player.StaminaUsageMultiplier"/>.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.StaminaUsageMultiplier), MethodType.Getter)]
    internal class StaminaUsage
    {
        private static void Postfix(Inventory __instance, ref float __result)
        {
            if (Player.TryGet(__instance._hub, out Player player))
                __result *= player.IsUsingStamina ? player.StaminaUsageMultiplier : 0;
        }
    }
}