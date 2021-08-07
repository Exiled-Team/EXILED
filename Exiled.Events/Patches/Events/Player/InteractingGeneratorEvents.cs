// -----------------------------------------------------------------------
// <copyright file="InteractingGeneratorEvents.cs" company="Exiled Team">
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
    /// Patches <see cref="Generator079.Interact(GameObject, PlayerInteract.Generator079Operations)"/>.
    /// Adds the <see cref="Player.InsertingGeneratorTablet"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Generator079), nameof(Generator079.Interact))]
    internal static class InteractingGeneratorEvents
    {
        private static bool Prefix(Generator079 __instance, GameObject person, PlayerInteract.Generator079Operations command)
        {
            try
            {
                API.Features.Player player = API.Features.Player.Get(person);
                switch (command)
                {
                    case PlayerInteract.Generator079Operations.Door:
                        bool isAllowed = true;
                        switch (__instance.isDoorOpen)
                        {
                            case false:
                                var openingEventArgs = new OpeningGeneratorEventArgs(player, __instance);

                                Player.OnOpeningGenerator(openingEventArgs);

                                isAllowed = openingEventArgs.IsAllowed;

                                break;
                            case true:
                                var closingEventArgs = new ClosingGeneratorEventArgs(player, __instance);

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

                        var insertingEventArgs = new InsertingGeneratorTabletEventArgs(player, __instance);
                        Player.OnInsertingGeneratorTablet(insertingEventArgs);

                        if (insertingEventArgs.IsAllowed)
                            __instance.NetworkisTabletConnected = true;
                        else
                            __instance.RpcDenied();

                        break;
                    case PlayerInteract.Generator079Operations.Cancel:
                        var ejectingEventArgs = new EjectingGeneratorTabletEventArgs(player, __instance);
                        Player.OnEjectingGeneratorTablet(ejectingEventArgs);

                        if (ejectingEventArgs.IsAllowed)
                            __instance.EjectTablet();
                        else
                            __instance.RpcDenied();

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
