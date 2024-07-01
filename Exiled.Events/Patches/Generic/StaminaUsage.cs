// -----------------------------------------------------------------------
// <copyright file="StaminaUsage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
#pragma warning disable IDE0051

    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using HarmonyLib;
    using InventorySystem;

    /// <summary>
    /// Patches <see cref="Inventory.StaminaUsageMultiplier"/>.
    /// Implements <see cref="Player.IsUsingStamina"/>, <see cref="FpcRole.IsUsingStamina"/> and <see cref="FpcRole.StaminaUsageMultiplier"/>.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.StaminaUsageMultiplier), MethodType.Getter)]
    internal class StaminaUsage
    {
        private static void Postfix(Inventory __instance, ref float __result)
        {
            if (Player.TryGet(__instance._hub, out Player player))
            {
                if (player.Role.Is(out FpcRole fpc))
                    __result *= fpc.IsUsingStamina ? fpc.StaminaUsageMultiplier : 0;
                __result *= player.IsUsingStamina ? 1 : 0;
            }
        }
    }
}