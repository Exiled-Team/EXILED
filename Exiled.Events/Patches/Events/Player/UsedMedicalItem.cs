// -----------------------------------------------------------------------
// <copyright file="UsedMedicalItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="ConsumableAndWearableItems.SendRpc(ConsumableAndWearableItems.HealAnimation, int)"/>.
    /// Adds the <see cref="Player.MedicalItemUsed"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.SendRpc))]
    internal static class UsedMedicalItem
    {
        private static void Prefix(ConsumableAndWearableItems __instance, ConsumableAndWearableItems.HealAnimation healAnimation, int mid)
        {
            try
            {
                if (healAnimation == ConsumableAndWearableItems.HealAnimation.DequipMedicalItem)
                {
                    var ev = new UsedMedicalItemEventArgs(API.Features.Player.Get(__instance.gameObject), __instance.usableItems[mid].inventoryID);

                    Player.OnMedicalItemUsed(ev);
                }
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.UsedMedicalItem: {e}\n{e.StackTrace}");
            }
        }
    }
}
