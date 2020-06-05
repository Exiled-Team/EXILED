// -----------------------------------------------------------------------
// <copyright file="UsedMedicalItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="ConsumableAndWearableItems.SendRpc(ConsumableAndWearableItems.HealAnimation, int)"/>.
    /// Adds the <see cref="Player.MedicalItemUsed"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.SendRpc))]
    public class UsedMedicalItem
    {
        /// <summary>
        /// Prefix of <see cref="ConsumableAndWearableItems.SendRpc(ConsumableAndWearableItems.HealAnimation, int)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="ConsumableAndWearableItems"/> instance.</param>
        /// <param name="animation">The heal animation.</param>
        /// <param name="mid">The medical item id.</param>
        public static void Prefix(ConsumableAndWearableItems __instance, ConsumableAndWearableItems.HealAnimation animation, int mid)
        {
            if (animation == ConsumableAndWearableItems.HealAnimation.DequipMedicalItem)
                Player.OnMedicalItemUsed(new UsedMedicalItemEventArgs(API.Features.Player.Get(__instance.gameObject), __instance.usableItems[mid].inventoryID));
        }
    }
}
