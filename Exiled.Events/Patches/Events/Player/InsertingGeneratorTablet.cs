// -----------------------------------------------------------------------
// <copyright file="InsertingGeneratorTablet.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Generator079.Interact(GameObject, string)"/>.
    /// Adds the <see cref="Player.InsertingGeneratorTablet"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Generator079), nameof(Generator079.Interact))]
    internal class InsertingGeneratorTablet
    {
        private static bool Prefix(Generator079 __instance, GameObject person, string command)
        {
            if (command.StartsWith("EPS_TABLET"))
            {
                if (__instance.isTabletConnected || !__instance.isDoorOpen || __instance._localTime <= 0.0 ||
                    Generator079.mainGenerator.forcedOvercharge)
                    return false;
                Inventory component = person.GetComponent<Inventory>();
                foreach (Inventory.SyncItemInfo syncItemInfo in component.items)
                {
                    if (syncItemInfo.id == ItemType.WeaponManagerTablet)
                    {
                        var ev = new InsertingGeneratorTabletEventArgs(API.Features.Player.Get(person), __instance);

                        Player.OnInsertingGeneratorTablet(ev);

                        if (!ev.IsAllowed)
                            return false;

                        component.items.Remove(syncItemInfo);
                        __instance.NetworkisTabletConnected = true;
                        break;
                    }
                }
            }
            else if (command.StartsWith("EPS_CANCEL"))
            {
                if (!__instance.isTabletConnected)
                    return false;

                var ev = new EjectingGeneratorTabletEventArgs(API.Features.Player.Get(person), __instance);

                Player.OnEjectingGeneratorTablet(ev);

                if (ev.IsAllowed)
                    __instance.EjectTablet();
            }
            else if (command.StartsWith("EPS_DOOR"))
            {
                Inventory component = person.GetComponent<Inventory>();
                if (component == null || __instance._doorAnimationCooldown > 0.0 || __instance._deniedCooldown > 0.0)
                    return false;
                if (!__instance.isDoorUnlocked)
                {
                    var ev = new UnlockingGeneratorEventArgs(API.Features.Player.Get(person), __instance, person.GetComponent<ServerRoles>().BypassMode);

                    if (component.curItem > ItemType.KeycardJanitor)
                    {
                        foreach (string permission in component.GetItemByID(component.curItem).permissions)
                        {
                            if (permission == "ARMORY_LVL_2")
                                ev.IsAllowed = true;
                        }
                    }

                    Player.OnUnlockingGenerator(ev);

                    if (ev.IsAllowed)
                    {
                        __instance.NetworkisDoorUnlocked = true;
                        __instance._doorAnimationCooldown = 0.5f;
                    }
                    else
                    {
                        __instance.RpcDenied();
                    }
                }
                else
                {
                    OpeningGeneratorEventArgs ev;

                    if (!__instance.NetworkisDoorOpen)
                    {
                        ev = new OpeningGeneratorEventArgs(API.Features.Player.Get(person), __instance);

                        Player.OnOpeningGenerator(ev);
                    }
                    else
                    {
                        ev = new ClosingGeneratorEventArgs(API.Features.Player.Get(person), __instance);

                        Player.OnClosingGenerator((ClosingGeneratorEventArgs)ev);
                    }

                    if (!ev.IsAllowed)
                    {
                        __instance.RpcDenied();
                        return false;
                    }

                    __instance._doorAnimationCooldown = 1.5f;
                    __instance.NetworkisDoorOpen = !__instance.isDoorOpen;
                    __instance.RpcDoSound(__instance.isDoorOpen);
                }
            }

            return false;
        }
    }
}
