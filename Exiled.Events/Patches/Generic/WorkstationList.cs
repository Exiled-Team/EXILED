// -----------------------------------------------------------------------
// <copyright file="WorkstationList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Attachments;

    /// <summary>
    /// Patches <see cref="WorkstationController.Start"/> to add a adding workstation into list.
    /// </summary>
    [HarmonyPatch(typeof(WorkstationController), nameof(WorkstationController.Start))]
    internal class WorkstationList
    {
        private static void Postfix(WorkstationController __instance) => Workstation.Get(__instance);
    }
}