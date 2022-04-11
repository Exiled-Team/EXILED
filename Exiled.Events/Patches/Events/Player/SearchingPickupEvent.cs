// -----------------------------------------------------------------------
// <copyright file="SearchingPickupEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Searching;

    using Mirror;
    using Mirror.LiteNetLib4Mirror;

    /// <summary>
    /// Patches <see cref="SearchCoordinator.ReceiveRequestUnsafe"/>.
    /// Adds the <see cref="Handlers.Player.SearchingPickup"/> event.
    /// </summary>
    [HarmonyPatch(typeof(SearchCoordinator), nameof(SearchCoordinator.ReceiveRequestUnsafe))]
    internal static class SearchingPickupEvent
    {
        private static bool Prefix(SearchCoordinator __instance, ref bool __result, out SearchSession? session, out SearchCompletor completor)
        {
            try
            {
                SearchRequest request = __instance.SessionPipe.Request;

                SearchingPickupEventArgs ev = new SearchingPickupEventArgs(Player.Get(__instance.Hub), request.Target, request.Body, SearchCompletor.FromPickup(__instance, request.Target, __instance.ServerMaxRayDistanceSqr), request.Target.SearchTime);
                Handlers.Player.OnSearchPickupRequest(ev);

                completor = ev.SearchCompletor;
                if (!ev.IsAllowed)
                {
                    session = null;
                    completor = null;
                    __result = true;
                    return false;
                }

                SearchSession body = ev.SearchSession;
                if (!__instance.isLocalPlayer)
                {
                    double num = NetworkTime.time - request.InitialTime;
                    double num2 = LiteNetLib4MirrorServer.Peers[__instance.connectionToClient.connectionId].Ping * 0.001 * __instance.serverDelayThreshold;
                    float searchTime = ev.SearchTime;
                    if (num < 0.0 || num2 < num)
                    {
                        body.InitialTime = NetworkTime.time - num2;
                        body.FinishTime = body.InitialTime + searchTime;
                    }
                    else if (Math.Abs(body.FinishTime - body.InitialTime - searchTime) > 0.001)
                    {
                        body.FinishTime = body.InitialTime + searchTime;
                    }
                }

                session = new SearchSession?(body);
                __result = true;
                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(SearchingPickupEvent).FullName}.{nameof(Prefix)}:\n{exception}");
                session = null;
                completor = null;
                return true;
            }
        }
    }
}
