// -----------------------------------------------------------------------
// <copyright file="PickingUpItem.cs" company="Exiled Team">
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

    using Searching;

    /// <summary>
    /// Patches <see cref="ItemSearchCompletor.Complete"/>.
    /// Adds the <see cref="Player.PickingUpItem"/> and <see cref="Player.Interacting2536"/> events.
    /// </summary>
    [HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
    internal static class PickingUpItem
    {
        private static bool Prefix(ItemSearchCompletor __instance)
        {
            try
            {
                // Remove after christmas update.
                if (__instance.PresentPickup != null)
                {
                    var evPresent = new Interacting2536EventArgs(API.Features.Player.Get(__instance.Hub), __instance.PresentPickup);
                    Player.OnInteracting2536(evPresent);

                    if (!evPresent.IsAllowed)
                    {
                        return false;
                    }

                    SCP_2536_Controller.singleton.Apply2536Scenario(__instance.Hub, evPresent.Scenario);
                    __instance.PresentPickup.ThisPresentsScenario = SCP_2536_Controller.Valid2536Scenario.BeenOpened;
                    __instance.PresentPickup.RpcOpenPresent();
                    return false;
                }

                var ev = new PickingUpItemEventArgs(API.Features.Player.Get(__instance.Hub.gameObject), __instance.TargetPickup);

                Player.OnPickingUpItem(ev);

                // Allow future pick up of this item
                if (!ev.IsAllowed)
                    __instance.TargetPickup.InUse = false;

                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.PickingUpItem: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
