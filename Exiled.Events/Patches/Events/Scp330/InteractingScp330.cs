// -----------------------------------------------------------------------
// <copyright file="InteractingScp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
    using System;
#pragma warning disable SA1118
#pragma warning disable SA1313

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CustomPlayerEffects;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using Footprinting;

    using HarmonyLib;

    using Interactables.Interobjects;

    using InventorySystem;
    using InventorySystem.Items.Usables.Scp330;
    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="Scp330Interobject.ServerInteract"/> method to add the <see cref="Handlers.Scp330.InteractingScp330"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330Interobject), nameof(Scp330Interobject.ServerInteract))]
    internal static class InteractingScp330
    {
        private static bool Prefix(Scp330Interobject __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                Footprint footprint = new(ply);
                float num = 0.1f;
                int num2 = 0;
                foreach (Footprint footprint2 in __instance._takenCandies)
                {
                    if (footprint2.Equals(footprint))
                    {
                        num = Mathf.Min(num, (float)footprint2.Stopwatch.Elapsed.TotalSeconds);
                        num2++;
                    }
                }

                if (num < 0.1f)
                    return false;

                InteractingScp330EventArgs ev = new(Player.Get(ply), Scp330Candies.GetRandom(), num2, ply.characterClassManager.IsHuman());
                Handlers.Scp330.OnInteractingScp330(ev);

                if (!ev.IsAllowed)
                    return false;

                if (!ServerProcessPickup(ply, null, ev.Candy, out Scp330Bag x))
                {
                    Scp330SearchCompletor.ShowOverloadHint(ply, x != null);
                    return false;
                }

                __instance.RpcMakeSound();
                if (ev.ShouldSever)
                {
                    ply.playerEffectsController.EnableEffect<SeveredHands>(0f, false);
                    return false;
                }

                __instance._takenCandies.Add(footprint);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(InteractingScp330).FullName}.{nameof(Prefix)}:\n{ex}");
                return true;
            }
        }

        private static bool ServerProcessPickup(ReferenceHub ply, Scp330Pickup pickup, CandyKindID candy, out Scp330Bag bag)
        {
            if (!Scp330Bag.TryGetBag(ply, out bag))
            {
                ushort num = pickup is null ? ushort.MinValue : pickup.Info.Serial;
                return ply.inventory.ServerAddItem(ItemType.SCP330, num, pickup) != null;
            }

            bool result = false;
            if (pickup is null)
            {
                result = bag.TryAddSpecific(candy);
            }
            else
            {
                while (pickup.StoredCandies.Count > 0 && bag.TryAddSpecific(pickup.StoredCandies[0]))
                {
                    result = true;
                    pickup.StoredCandies.RemoveAt(0);
                }
            }

            if (bag.AcquisitionAlreadyReceived)
            {
                bag.ServerRefreshBag();
            }

            return result;
        }
    }
}
