﻿// -----------------------------------------------------------------------
// <copyright file="EscapingPocketDimension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
#pragma warning disable SA1313
    using System.Collections.Generic;

    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;

    using GameCore;

    using HarmonyLib;

    using LightContainmentZoneDecontamination;

    using Mirror;

    using UnityEngine;

    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Patches <see cref="PocketDimensionTeleport.OnTriggerEnter(Collider)"/>.
    /// Adds the <see cref="Player.EscapingPocketDimension"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    public class EscapingPocketDimension
    {
        /// <summary>
        /// Prefix of <see cref="PocketDimensionTeleport.OnTriggerEnter(Collider)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PocketDimensionTeleport"/> instance.</param>
        /// <param name="other">The <see cref="Collider"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(PocketDimensionTeleport __instance, Collider other)
        {
            NetworkIdentity component1 = other.GetComponent<NetworkIdentity>();
            if (!((Object)component1 != (Object)null))
                return false;
            if (__instance.type == PocketDimensionTeleport.PDTeleportType.Killer || BlastDoor.OneDoor.isClosed)
            {
                component1.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(999990f, "WORLD", DamageTypes.Pocket, 0), other.gameObject);
            }
            else if (__instance.type == PocketDimensionTeleport.PDTeleportType.Exit)
            {
                __instance.tpPositions.Clear();
                bool flag = false;
                DecontaminationController.DecontaminationPhase[] decontaminationPhases = DecontaminationController.Singleton.DecontaminationPhases;
                if (DecontaminationController.GetServerTime > (double)decontaminationPhases[decontaminationPhases.Length - 2].TimeTrigger)
                    flag = true;
                List<string> stringList = ConfigFile.ServerConfig.GetStringList(flag ? "pd_random_exit_rids_after_decontamination" : "pd_random_exit_rids");
                if (stringList.Count > 0)
                {
                    foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("RoomID"))
                    {
                        if (gameObject.GetComponent<Rid>() != (Object)null && stringList.Contains(gameObject.GetComponent<Rid>().id))
                            __instance.tpPositions.Add(gameObject.transform.position);
                    }

                    if (stringList.Contains("PORTAL"))
                    {
                        foreach (Scp106PlayerScript scp106PlayerScript in Object.FindObjectsOfType<Scp106PlayerScript>())
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

                component2.OverridePosition(tpPosition, 0.0f, false);
                __instance.RemoveCorrosionEffect(other.gameObject);
                PlayerManager.localPlayer.GetComponent<PlayerStats>().TargetAchieve(component1.connectionToClient, "larryisyourfriend");
            }

            if (!__instance.RefreshExit)
                return false;
            ImageGenerator.pocketDimensionGenerator.GenerateRandom();
            return false;
        }
    }
}
