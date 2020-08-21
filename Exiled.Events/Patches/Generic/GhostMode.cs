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
                    Array.Copy(__instance._receivedData, __instance._transmitBuffer, __instance._usedData);

                    if (player.Role.Is939())
                    {
                        for (int index = 0; index < __instance._usedData; ++index)
                        {
                            if (__instance._transmitBuffer[index].position.y < 800.0)
                            {
                                ReferenceHub hub2 = ReferenceHub.GetHub(players[index]);
                                if (hub2.characterClassManager.CurRole.team != Team.SCP &&
                                    hub2.characterClassManager.CurRole.team != Team.RIP && !players[index]
                                        .GetComponent<Scp939_VisionController>()
                                        .CanSee(player.ReferenceHub.characterClassManager.Scp939))
                                    __instance._transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance._transmitBuffer[index].playerID);
                            }
                        }
                    }
                    else if (player.Role != RoleType.Spectator && player.Role != RoleType.Scp079)
                    {
                        for (int index = 0; index < __instance._usedData; ++index)
                        {
                            PlayerPositionData ppd = __instance._transmitBuffer[index];
                            Player currentTarget = Player.Get(players[index]);
                            Scp096 scp096 = player.ReferenceHub.scpsController.CurrentScp as Scp096;
                            bool canSee = true;
                            bool shouldRotate = false;

                            if (currentTarget?.ReferenceHub == null)
                                continue;

                            if (currentTarget.IsInvisible || player.TargetGhosts.Contains(ppd.playerID))
                                canSee = false;

                            Vector3 vector3 = __instance._transmitBuffer[index].position - player.ReferenceHub.playerMovementSync.RealModelPosition;
                            if (Math.Abs(vector3.y) > 35.0)
                            {
                                canSee = false;
                            }
                            else
                            {
                                float sqrMagnitude = vector3.sqrMagnitude;
                                if (player.ReferenceHub.playerMovementSync.RealModelPosition.y < 800.0)
                                {
                                    if (sqrMagnitude >= 1764.0)
                                    {
                                        canSee = false;
                                        continue;
                                    }
                                }
                                else if (sqrMagnitude >= 7225.0)
                                {
                                    canSee = false;
                                    continue;
                                }

                                if (ReferenceHub.TryGetHub(__instance._transmitBuffer[index].playerID, out ReferenceHub hub2))
                                {
                                    if (player.ReferenceHub.scpsController.CurrentScp is Scp096 currentScp && currentScp.Enraged && (!currentScp.HasTarget(hub2) && hub2.characterClassManager.CurRole.team != Team.SCP))
                                    {
                                        canSee = false;
                                    }
                                    else if (hub2.playerEffectsController.GetEffect<Scp268>().Enabled)
                                    {
                                        bool flag = false;
                                        if (scp096 != null)
                                            flag = scp096._targets.Contains(hub2);

                                        canSee = flag;
                                    }
                                }

                                switch (player.Role)
                                {
                                    case RoleType.Scp173 when !Exiled.Events.Events.Instance.Config.CanTutorialBlockScp173:
                                        shouldRotate = true;
                                        break;
                                    case RoleType.Scp096 when !Exiled.Events.Events.Instance.Config.CanTutorialTriggerScp096:
                                        shouldRotate = true;
                                        break;
                                }

                                if (!canSee)
                                {
                                    ppd = new PlayerPositionData(Vector3.up * 6000f, 0.0f, ppd.playerID);
                                }
                                else if (shouldRotate)
                                {
                                    ppd = new PlayerPositionData(ppd.position, Quaternion.Inverse(Quaternion.LookRotation(FindLookRotation(player.Position, currentTarget.Position))).eulerAngles.y, ppd.playerID);
                                }

                                __instance._transmitBuffer[index] = ppd;
                            }
                        }
                    }

                    NetworkConnection networkConnection = player.ReferenceHub.characterClassManager.netIdentity.isLocalPlayer
                        ? NetworkServer.localConnection
                        : player.ReferenceHub.characterClassManager.netIdentity.connectionToClient;
                    if (__instance._usedData <= 20)
                    {
                        networkConnection.Send(
                            new PlayerPositionManager.PositionMessage(__instance._transmitBuffer, (byte)__instance._usedData, 0), 1);
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

        private static Vector3 FindLookRotation(Vector3 player, Vector3 target) => (player - target).normalized;
    }
}
