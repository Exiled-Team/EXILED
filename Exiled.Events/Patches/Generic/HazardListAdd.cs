// -----------------------------------------------------------------------
// <copyright file="HazardListAdd.cs" company="Exiled Team">
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
    /// Patch for adding hazards.
    /// </summary>
    [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.Start))]
    internal class HazardListAdd
    {
        private static void Postfix(EnvironmentalHazard __instance) => _ = Hazard.Get(__instance);
    }
}