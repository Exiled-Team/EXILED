// -----------------------------------------------------------------------
// <copyright file="HazardList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313

    using Exiled.API.Features.Hazards;
    using HarmonyLib;
    using Hazards;

    /// <summary>
    /// Patch for controlling hazard list.
    /// </summary>
    [HarmonyPatch]
    internal class HazardList
    {
        [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.Start))]
        [HarmonyPostfix]
        private static void Adding(EnvironmentalHazard __instance) => Hazard.Get(__instance);

        [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnDestroy))]
        [HarmonyPostfix]
        private static void Removing(EnvironmentalHazard __instance) => Hazard.EnvironmentalHazardToHazard.Remove(__instance);
    }
}