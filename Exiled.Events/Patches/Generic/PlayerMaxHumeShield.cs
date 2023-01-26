// -----------------------------------------------------------------------
// <copyright file="PlayerMaxHumeShield.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using Exiled.API.Features;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using HarmonyLib;
    using PlayerRoles.PlayableScps.HumeShield;

    /// <summary>
    /// Patch the <see cref="DynamicHumeShieldController.HsMax"/>.
    /// </summary>
    [HarmonyPatch(typeof(DynamicHumeShieldController), nameof(DynamicHumeShieldController.HsMax), MethodType.Getter)]
    internal static class PlayerMaxHumeShield
    {
        private static bool Postfix(DynamicHumeShieldController __instance, ref float __result)
        {
            Player player = Player.Get(__instance.Owner);

            if (player.MaxHS == default)
                __result = player.MaxHS;
            else
                __result = __instance.ShieldOverHealth.Evaluate(__instance._hp.NormalizedValue);

            return false;
        }
    }
}