// -----------------------------------------------------------------------
// <copyright file="StaminaRegen.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313

    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using HarmonyLib;
    using InventorySystem;

    /// <summary>
    /// Patches <see cref="Inventory.StaminaRegenMultiplier"/>.
    /// Implements <see cref="FpcRole.StaminaRegenMultiplier"/>.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.StaminaRegenMultiplier), MethodType.Getter)]
    internal class StaminaRegen
    {
        private static void Postfix(Inventory __instance, ref float __result)
        {
            if (Player.TryGet(__instance._hub, out Player player) && player.Role.Is(out FpcRole fpc))
                __result *= fpc.StaminaRegenMultiplier;
        }
    }
}