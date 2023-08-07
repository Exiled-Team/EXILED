// -----------------------------------------------------------------------
// <copyright file="DoorListAdd.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313

    using Exiled.API.Features;
    using HarmonyLib;
    using Interactables.Interobjects;
    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Patch for adding doors to list.
    /// </summary>
    [HarmonyPatch]
    internal class DoorListAdd
    {
        [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.Start))]
        private static void Postfix(DoorVariant __instance)
        {
            _ = Door.Get(__instance);
        }

        [HarmonyPatch(typeof(AirlockController), nameof(AirlockController.Start))]
        private static void Postfix(AirlockController __instance)
        {
            _ = new Exiled.API.Features.Doors.AirlockController(__instance);
        }
    }
}