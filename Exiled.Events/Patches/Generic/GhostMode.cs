// -----------------------------------------------------------------------
// <copyright file="GhostMode.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;

    using CustomPlayerEffects;

    using Exiled.API.Features;

    using HarmonyLib;

    using Mirror;

    using PlayableScps;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerPositionManager.TransmitData"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayerPositionManager), nameof(PlayerPositionManager.TransmitData))]
    internal static class GhostMode
    {
        private static bool Prefix(PlayerPositionManager __instance)
        {
            try
            {
                ++__instance._frame;
                if (__instance._frame != __instance._syncFrequency)
                    return false;
                __instance._frame = 0;
                List<GameObject> players = PlayerManager.players;
                __instance._usedData = players.Count;
                if (__instance._receivedData == null || __instance._receivedData.Length < __instance._usedData)
                    __instance._receivedData = new PlayerPositionData[__instance._usedData * 2];
                for (int index = 0; index < __instance._usedData; ++index)
                    __instance._receivedData[index] = new PlayerPositionData(ReferenceHub.GetHub(players[index]));
                if (__instance._transmitBuffer == null || __instance._transmitBuffer.Length < __instance._usedData)
                    __instance._transmitBuffer = new PlayerPositionData[__instance._usedData * 2];

                foreach (GameObject gameObject in players)
                {
                    Player player = Player.Get(gameObject);

                    if (player == null)
                        continue;

                    Array.Copy(__instance._receivedData, __instance._transmitBuffer, __instance._usedData);
                    for (int index = 0; index < __instance._usedData; ++index)
                    {
                        PlayerPositionData ppd = __instance._transmitBuffer[index];
                        Player currentTarget = Player.Get(players[index]);
                        Scp096 scp096 = player.ReferenceHub.scpsController.CurrentScp as Scp096;
                        bool canSee = true;
                        if (currentTarget == null)
                            continue;

                        if (currentTarget.IsInvisible || player.TargetGhosts.Contains(ppd.playerID))
                        {
                            canSee = false;
                        }
                        else if (player.Role.Is939() && ppd.position.y < 800.0)
                        {
                            if (currentTarget.Team != Team.SCP && currentTarget.Team != Team.RIP && !currentTarget.GameObject.GetComponent<Scp939_VisionController>().CanSee(player.ReferenceHub.characterClassManager.Scp939))
                                canSee = false;
                        }
                        else if (player.Role != RoleType.Scp079 && player.Role != RoleType.Spectator)
                        {
                            if (Math.Abs(ppd.position.y - player.Position.y) > 35)
                            {
                                canSee = false;
                            }
                            else
                            {
                                if (ReferenceHub.TryGetHub(ppd.playerID, out ReferenceHub hub))
                                {
                                    if (scp096 != null && scp096.Enraged && !scp096.HasTarget(hub) && hub.characterClassManager.CurRole.team != Team.SCP)
                                        canSee = false;
                                    else if (hub.playerEffectsController.GetEffect<Scp268>().Enabled && (scp096 == null || !scp096.HasTarget(hub)))
                                        canSee = false;
                                }
                            }
                        }
                        if (!canSee)
                            ppd = new PlayerPositionData(Vector3.up * 6000f, 0.0f, ppd.playerID);

                        __instance._transmitBuffer[index] = ppd;
                    }

                    NetworkConnection networkConnection = player.ReferenceHub.characterClassManager.netIdentity.isLocalPlayer ? NetworkServer.localConnection : player.ReferenceHub.characterClassManager.netIdentity.connectionToClient;
                    if (__instance._usedData <= 20)
                    {
                        networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance._transmitBuffer, (byte)__instance._usedData, 0), 1);
                    }
                    else
                    {
                        byte part;
                        for (part = 0; part < __instance._usedData / 20; ++part)
                            networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance._transmitBuffer, 20, part), 1);
                        byte count = (byte)(__instance._usedData % (part * 20));
                        if (count > 0)
                            networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance._transmitBuffer, count, part), 1);
                    }
                }

                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"GhostMode error: {exception}");
                return true;
            }
        }
    }
}
