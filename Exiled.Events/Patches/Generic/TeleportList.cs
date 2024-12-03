// -----------------------------------------------------------------------
// <copyright file="TeleportList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
#pragma warning disable SA1402
    using API.Features;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PocketDimensionTeleport.Awake"/>.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.Awake))]
    internal class TeleportList
    {
        private static void Postfix(PocketDimensionTeleport __instance) => Map.TeleportsValue.Add(__instance);
    }

    /// <summary>
    /// Patches <see cref="PocketDimensionTeleport.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnDestroy))]
    internal class TeleportListRemove
    {
        private static void Postfix(PocketDimensionTeleport __instance) => Map.TeleportsValue.Remove(__instance);
    }
}