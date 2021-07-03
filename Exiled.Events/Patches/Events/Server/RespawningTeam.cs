// -----------------------------------------------------------------------
// <copyright file="RespawningTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using Respawning;
    using Respawning.NamingRules;

    using UnityEngine;

    /// <summary>
    /// Patch the <see cref="RespawnManager.Spawn"/>.
    /// Adds the <see cref="Handlers.Server.RespawningTeam"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
    internal static class RespawningTeam
    {
        private static bool Prefix(RespawnManager __instance)
        {
            try
            {
                if (!RespawnWaveGenerator.SpawnableTeams.TryGetValue(__instance.NextKnownTeam, out SpawnableTeam spawnableTeam) ||
                    __instance.NextKnownTeam == SpawnableTeamType.None)
                {
                    ServerConsole.AddLog("Fatal error. Team '" + __instance.NextKnownTeam + "' is undefined.", ConsoleColor.Red);
                }
                else
                {
                    List<API.Features.Player> list = ListPool<API.Features.Player>.Shared.Rent(API.Features.Player.List.Where(player => player.IsDead && !player.IsOverwatchEnabled));

                    if (__instance._prioritySpawn)
                    {
                        var tempList = ListPool<API.Features.Player>.Shared.Rent();

                        tempList.AddRange(list.OrderBy(item => item.ReferenceHub.characterClassManager.DeathTime));

                        ListPool<API.Features.Player>.Shared.Return(list);

                        list = tempList;
                    }
                    else
                    {
                        list.ShuffleList();
                    }

                    // Code that should be here is in RespawningTeamEventArgs::ReissueNextKnownTeam
                    var ev = new RespawningTeamEventArgs(list, __instance.NextKnownTeam);

                    Handlers.Server.OnRespawningTeam(ev);

                    if (ev.IsAllowed && ev.SpawnableTeam != null)
                    {
                        while (list.Count > ev.MaximumRespawnAmount)
                            list.RemoveAt(list.Count - 1);

                        list.ShuffleList();

                        List<ReferenceHub> referenceHubList = ListPool<ReferenceHub>.Shared.Rent();

                        foreach (API.Features.Player me in list)
                        {
                            try
                            {
                                RoleType classid = ev.SpawnableTeam.Value.ClassQueue[Mathf.Min(referenceHubList.Count, spawnableTeam.ClassQueue.Length - 1)];

                                me.ReferenceHub.characterClassManager.SetPlayersClass(classid, me.ReferenceHub.gameObject);

                                referenceHubList.Add(me.ReferenceHub);

                                ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + me.ReferenceHub.LoggedNameFromRefHub() + " respawned as " + classid + ".", ServerLogs.ServerLogType.GameEvent);
                            }
                            catch (Exception ex)
                            {
                                if (me != null)
                                    ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + me.ReferenceHub.LoggedNameFromRefHub() + " couldn't be spawned. Err msg: " + ex.Message, ServerLogs.ServerLogType.GameEvent);
                                else
                                    ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Couldn't spawn a player - target's ReferenceHub is null.", ServerLogs.ServerLogType.GameEvent);
                            }
                        }

                        if (referenceHubList.Count > 0)
                        {
                            ServerLogs.AddLog(ServerLogs.Modules.ClassChange, $"RespawnManager has successfully spawned {referenceHubList.Count} players as {__instance.NextKnownTeam}!", ServerLogs.ServerLogType.GameEvent);
                            RespawnTickets.Singleton.GrantTickets(__instance.NextKnownTeam, -referenceHubList.Count * spawnableTeam.TicketRespawnCost);

                            if (UnitNamingRules.TryGetNamingRule(__instance.NextKnownTeam, out UnitNamingRule rule))
                            {
                                rule.GenerateNew(__instance.NextKnownTeam, out string regular);
                                foreach (ReferenceHub referenceHub in referenceHubList)
                                {
                                    referenceHub.characterClassManager.NetworkCurSpawnableTeamType =
                                        (byte)__instance.NextKnownTeam;
                                    referenceHub.characterClassManager.NetworkCurUnitName = regular;
                                }

                                rule.PlayEntranceAnnouncement(regular);
                            }

                            RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, __instance.NextKnownTeam);
                        }

                        ListPool<ReferenceHub>.Shared.Return(referenceHubList);
                    }

                    ListPool<API.Features.Player>.Shared.Return(list);
                    __instance.NextKnownTeam = SpawnableTeamType.None;
                }

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Server.RespawningTeam: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
