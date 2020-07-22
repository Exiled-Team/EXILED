// -----------------------------------------------------------------------
// <copyright file="EscapingAndFailingEscapePocketDimension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using GameCore;

    using HarmonyLib;

    using LightContainmentZoneDecontamination;

    using Mirror;

    using UnityEngine;

    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Patches <see cref="PocketDimensionTeleport.OnTriggerEnter(Collider)"/>.
    /// Adds the <see cref="Player.EscapingPocketDimension"/> and <see cref="Player.FailingEscapePocketDimension"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class EscapingAndFailingEscapePocketDimension
    {
        private static bool Prefix(PocketDimensionTeleport __instance, Collider other)
        {
            try
            {
                NetworkIdentity component1 = other.GetComponent<NetworkIdentity>();
                if (!(component1 != null))
                    return false;
                if (__instance.type == PocketDimensionTeleport.PDTeleportType.Killer || BlastDoor.OneDoor.isClosed)
                {
                    if (__instance.type == PocketDimensionTeleport.PDTeleportType.Killer)
                    {
                        var ev = new FailingEscapePocketDimensionEventArgs(API.Features.Player.Get(other.gameObject), __instance);

                        Player.OnFailingEscapePocketDimension(ev);

                        if (!ev.IsAllowed)
                            return false;
                    }
                    else
                    {
                        // warhead larry event goes here
                    }

                    component1.GetComponent<PlayerStats>()
                        .HurtPlayer(new PlayerStats.HitInfo(999990f, "WORLD", DamageTypes.Pocket, 0), other.gameObject);
                }
                else if (__instance.type == PocketDimensionTeleport.PDTeleportType.Exit)
                {
                    __instance.tpPositions.Clear();
                    bool flag = false;
                    DecontaminationController.DecontaminationPhase[] decontaminationPhases =
                        DecontaminationController.Singleton.DecontaminationPhases;
                    if (DecontaminationController.GetServerTime >
                        decontaminationPhases[decontaminationPhases.Length - 2].TimeTrigger)
                        flag = true;
                    List<string> stringList =
                        ConfigFile.ServerConfig.GetStringList(flag
                            ? "pd_random_exit_rids_after_decontamination"
                            : "pd_random_exit_rids");
                    if (stringList.Count > 0)
                    {
                        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("RoomID"))
                        {
                            if (gameObject.GetComponent<Rid>() != null &&
                                stringList.Contains(gameObject.GetComponent<Rid>().id))
                                __instance.tpPositions.Add(gameObject.transform.position);
                        }

                        if (stringList.Contains("PORTAL"))
                        {
                            foreach (Scp106PlayerScript scp106PlayerScript in Object
                                .FindObjectsOfType<Scp106PlayerScript>())
                            {
                                if (scp106PlayerScript.portalPosition != Vector3.zero)
                                    __instance.tpPositions.Add(scp106PlayerScript.portalPosition);
                            }
                        }
                    }

                    if (__instance.tpPositions == null || __instance.tpPositions.Count == 0)
                    {
                        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("PD_EXIT"))
                            __instance.tpPositions.Add(gameObject.transform.position);
                    }

                    Vector3 tpPosition = __instance.tpPositions[Random.Range(0, __instance.tpPositions.Count)];
                    tpPosition.y += 2f;
                    PlayerMovementSync component2 = other.GetComponent<PlayerMovementSync>();
                    component2.SetSafeTime(2f);

                    var ev = new EscapingPocketDimensionEventArgs(API.Features.Player.Get(component2.gameObject), tpPosition);

                    Player.OnEscapingPocketDimension(ev);

                    if (ev.IsAllowed)
                    {
                        component2.OverridePosition(tpPosition, 0.0f, false);
                        __instance.RemoveCorrosionEffect(other.gameObject);
                        PlayerManager.localPlayer.GetComponent<PlayerStats>()
                            .TargetAchieve(component1.connectionToClient, "larryisyourfriend");
                    }
                }
                if (!__instance.RefreshExit)
                    return false;
                ImageGenerator.pocketDimensionGenerator.GenerateRandom();
                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.EscapingAndFailingEscapePocketDimension: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
