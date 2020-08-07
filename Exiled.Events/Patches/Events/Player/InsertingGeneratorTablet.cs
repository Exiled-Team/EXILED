// -----------------------------------------------------------------------
// <copyright file="InsertingGeneratorTablet.cs" company="Exiled Team">
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

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Generator079.Interact(GameObject, PlayerInteract.Generator079Operations)"/>.
    /// Adds the <see cref="Player.InsertingGeneratorTablet"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Generator079), nameof(Generator079.Interact))]
    internal static class InsertingGeneratorTablet
    {
        private static bool Prefix(Generator079 __instance, GameObject person, PlayerInteract.Generator079Operations command)
        {
            try
            {
                switch (command)
                {
                    case PlayerInteract.Generator079Operations.Door:
                        bool isAllowed = true;
                        switch (__instance.isDoorOpen)
                        {
                            case false:
                                var openingEventArgs = new OpeningGeneratorEventArgs(API.Features.Player.Get(person), __instance, isAllowed);

                                Player.OnOpeningGenerator(openingEventArgs);

                                isAllowed = openingEventArgs.IsAllowed;
                                break;
                            case true:
                                var closingEventArgs = new ClosingGeneratorEventArgs(API.Features.Player.Get(person), __instance, isAllowed);

                                Player.OnClosingGenerator(closingEventArgs);

                                isAllowed = closingEventArgs.IsAllowed;
                                break;
                        }

                        if (isAllowed)
                            __instance.OpenClose(person);
                        else
                            __instance.RpcDenied();
                        break;

                    case PlayerInteract.Generator079Operations.Tablet:
                        if (__instance.isTabletConnected || !__instance.isDoorOpen || (__instance._localTime <= 0.0 || Generator079.mainGenerator.forcedOvercharge))
                            break;
                        Inventory component = person.GetComponent<Inventory>();
                        using (SyncList<Inventory.SyncItemInfo>.SyncListEnumerator enumerator = component.items.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                Inventory.SyncItemInfo current = enumerator.Current;
                                if (current.id == ItemType.WeaponManagerTablet)
                                {
                                    var insertingEventAgrs = new InsertingGeneratorTabletEventArgs(API.Features.Player.Get(person), __instance);

                                    Player.OnInsertingGeneratorTablet(insertingEventAgrs);

                                    if (insertingEventAgrs.IsAllowed)
                                    {
                                        component.items.Remove(current);
                                        __instance.NetworkisTabletConnected = true;
                                    }

                                    break;
                                }
                            }

                            break;
                        }

                    case PlayerInteract.Generator079Operations.Cancel:
                        var ejectingEventArgs = new EjectingGeneratorTabletEventArgs(API.Features.Player.Get(person), __instance);

                        Player.OnEjectingGeneratorTablet(ejectingEventArgs);

                        if (ejectingEventArgs.IsAllowed)
                            __instance.EjectTablet();

                        break;
                }

                return false;
            }
            catch (Exception exception)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.InsertingGeneratorTablet: {exception}\n{exception.StackTrace}");

                return true;
            }
        }
    }
}
