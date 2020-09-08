// -----------------------------------------------------------------------
// <copyright file="ActivatingWorkstation.cs" company="Exiled Team">
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

    using UnityEngine;

    /// <summary>
    /// Patch the <see cref="WorkStation.ConnectTablet(UnityEngine.GameObject)"/>.
    /// Adds the <see cref="Player.ActivatingWorkstation"/> event.
    /// </summary>
    [HarmonyPatch(typeof(WorkStation), nameof(WorkStation.ConnectTablet))]
    internal static class ActivatingWorkstation
    {
        private static bool Prefix(WorkStation __instance, GameObject tabletOwner)
        {
            try
            {
                if (!__instance.CanPlace(tabletOwner) || __instance._animationCooldown > 0f)
                {
                    return false;
                }

                Inventory component = tabletOwner.GetComponent<Inventory>();
                foreach (Inventory.SyncItemInfo syncItemInfo in component.items)
                {
                    if (syncItemInfo.id == ItemType.WeaponManagerTablet)
                    {
                        ActivatingWorkstationEventArgs ev = new ActivatingWorkstationEventArgs(Exiled.API.Features.Player.Get(tabletOwner), __instance);
                        Player.OnActivatingWorkstation(ev);

                        if (ev.IsAllowed)
                        {
                            component.items.Remove(syncItemInfo);
                            __instance.NetworkisTabletConnected = true;
                            __instance._animationCooldown = 6.5f;
                            __instance.Network_playerConnected = tabletOwner;
                        }

                        break;
                    }
                }

                return false;
            }
            catch (Exception exception)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.ActivatingWorkstation: {exception}\n{exception.StackTrace}");
                return true;
            }
        }
    }
}
