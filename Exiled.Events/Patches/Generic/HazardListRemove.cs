// -----------------------------------------------------------------------
// <copyright file="HazardListRemove.cs" company="Exiled Team">
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

    using TemporaryHazard = Hazards.TemporaryHazard;

    /// <summary>
    /// Patches <see cref="TemporaryHazard.ServerDestroy()"/> and <see cref="EnvironmentalHazard.OnDestroy()"/>.
    /// </summary>
    [HarmonyPatch]
    internal class HazardListRemove
    {
        [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnDestroy))]
        [HarmonyPatch(typeof(TemporaryHazard), nameof(TemporaryHazard.ServerDestroy))]
        private static void Postfix(EnvironmentalHazard __instance) => Hazard.EnvironmentalHazardToHazard.Remove(__instance);
    }
}