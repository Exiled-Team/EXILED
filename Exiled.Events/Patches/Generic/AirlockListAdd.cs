// -----------------------------------------------------------------------
// <copyright file="AirlockListAdd.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313

    using HarmonyLib;
    using Interactables.Interobjects;

    /// <summary>
    /// Patch for adding <see cref="Exiled.API.Features.Doors.AirlockController"/> to list.
    /// </summary>
    [HarmonyPatch(typeof(AirlockController), nameof(AirlockController.Start))]
    internal class AirlockListAdd
    {
        private static void Postfix(AirlockController __instance)
        {
            _ = new Exiled.API.Features.Doors.AirlockController(__instance);
        }
    }
}