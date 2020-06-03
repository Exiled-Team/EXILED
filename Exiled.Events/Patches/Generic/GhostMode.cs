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
    public class GhostMode
    {
        /// <summary>
        /// Prefix of <see cref="PlayerPositionManager.TransmitData"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerPositionManager"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(PlayerPositionManager __instance)
        {
            try
            {
                ++__instance.frame;
                if (__instance.frame != __instance.syncFrequency)
                    return false;
                __instance.frame = 0;
                List<GameObject> players = PlayerManager.players;
                __instance.usedData = players.Count;
                if (__instance.receivedData == null || __instance.receivedData.Length < __instance.usedData)
                    __instance.receivedData = new PlayerPositionData[__instance.usedData * 2];
                for (int index = 0; index < __instance.usedData; ++index)
                    __instance.receivedData[index] = new PlayerPositionData(ReferenceHub.GetHub(players[index]));
                if (__instance.transmitBuffer == null || __instance.transmitBuffer.Length < __instance.usedData)
                    __instance.transmitBuffer = new PlayerPositionData[__instance.usedData * 2];
                foreach (GameObject gameObject in players)
                {
                    Player player = Player.Get(gameObject);
                    Array.Copy(__instance.receivedData, __instance.transmitBuffer, __instance.usedData);
                    if (player.Role.Is939())
                    {
                        for (int index = 0; index < __instance.usedData; ++index)
                        {
                            if (__instance.transmitBuffer[index].position.y < 800.0)
                            {
                                ReferenceHub hub2 = ReferenceHub.GetHub(players[index]);
                                if (player.IsInvisible || (hub2.characterClassManager.CurRole.team != Team.SCP && hub2.characterClassManager.CurRole.team != Team.RIP && !players[index].GetComponent<Scp939_VisionController>().CanSee(player.ReferenceHub.characterClassManager.Scp939)))
                                    __instance.transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance.transmitBuffer[index].playerID);
                            }
                        }
                    }
                    else if (player.Role != RoleType.Scp079 && player.Role != RoleType.Spectator)
                    {
                        for (int index = 0; index < __instance.usedData; ++index)
                        {
                            ReferenceHub hub2;
                            if (ReferenceHub.TryGetHub(__instance.transmitBuffer[index].playerID, out hub2))
                            {
                                if (player.IsInvisible || (player.ReferenceHub.scpsController.CurrentScp is Scp096 currentScp && currentScp.Enraged && (!currentScp.HasTarget(hub2) && hub2.characterClassManager.CurRole.team != Team.SCP)))
                                    __instance.transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance.transmitBuffer[index].playerID);

                                if (hub2.playerEffectsController.GetEffect<Scp268>().Enabled)
                                {
                                    bool flag = false;
                                    if (player.ReferenceHub.scpsController.CurrentScp is Scp096 curScp && curScp != null)
                                        flag = curScp.HasTarget(hub2);
                                    if (player.Role != RoleType.Scp079 && player.Role != RoleType.Spectator && !flag)
                                        __instance.transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance.transmitBuffer[index].playerID);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < __instance.usedData; i++)
                    {
                        if (player.TargetGhosts.Contains(__instance.transmitBuffer[i].playerID))
                            __instance.transmitBuffer[i] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance.transmitBuffer[i].playerID);
                    }

                    NetworkConnection networkConnection = player.ReferenceHub.characterClassManager.netIdentity.isLocalPlayer ? NetworkServer.localConnection : player.ReferenceHub.characterClassManager.netIdentity.connectionToClient;
                    if (__instance.usedData <= 20)
                    {
                        networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance.transmitBuffer, (byte)__instance.usedData, 0), 1);
                    }
                    else
                    {
                        byte part;
                        for (part = (byte)0; (int)part < __instance.usedData / 20; ++part)
                            networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance.transmitBuffer, 20, part), 1);
                        byte count = (byte)(__instance.usedData % (part * 20));
                        if (count > 0)
                            networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance.transmitBuffer, count, part), 1);
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