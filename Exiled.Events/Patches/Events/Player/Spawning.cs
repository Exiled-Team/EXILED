// -----------------------------------------------------------------------
// <copyright file="Spawning.cs" company="Exiled Team">
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

    using Respawning;
    using Respawning.NamingRules;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.ApplyProperties(bool, bool)"/>.
    /// Adds the <see cref="Player.Spawning"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.ApplyProperties))]
    internal static class Spawning
    {
        private static bool Prefix(CharacterClassManager __instance, bool lite = false, bool escape = false)
        {
            try
            {
                Role role = __instance.CurRole;
                if (!__instance._wasAnytimeAlive
                    && __instance.CurClass != RoleType.Spectator
                    && __instance.CurClass != RoleType.None)
                {
                    __instance._wasAnytimeAlive = true;
                }

                __instance.InitSCPs();
                __instance.AliveTime = 0f;
                switch (role.team)
                {
                    case Team.MTF:
                        AchievementManager.Achieve("arescue");
                        break;
                    case Team.CHI:
                        AchievementManager.Achieve("chaos");
                        break;
                    case Team.RSC:
                    case Team.CDP:
                        __instance.EscapeStartTime = (int)Time.realtimeSinceStartup;
                        break;
                }

                try
                {
                    __instance._hub.footstepSync.SetLoudness(role.team, role.roleId.Is939());
                }
                catch
                {
                }

                if (NetworkServer.active)
                {
                    Handcuffs component = __instance._hub.handcuffs;
                    component.ClearTarget();
                    component.NetworkCufferId = -1;
                    component.NetworkForceCuff = false;

                    if (role.roleId != RoleType.Spectator
                        && RespawnManager.CurrentSequence() != RespawnManager.RespawnSequencePhase.SpawningSelectedTeam
                        && UnitNamingManager.RolesWithEnforcedDefaultName.TryGetValue(role.roleId, out SpawnableTeamType value)
                        && RespawnManager.Singleton.NamingManager.TryGetAllNamesFromGroup((byte)value, out string[] names)
                        && names.Length != 0)
                    {
                        __instance.NetworkCurSpawnableTeamType = (byte)value;
                        __instance.NetworkCurUnitName = names[0];
                    }
                    else if (__instance.CurSpawnableTeamType != 0)
                    {
                        __instance.NetworkCurSpawnableTeamType = 0;
                        __instance.NetworkCurUnitName = string.Empty;
                    }
                }

                if (role.team != Team.RIP)
                {
                    if (NetworkServer.active && !lite)
                    {
                        Vector3 constantRespawnPoint =
                            NonFacilityCompatibility.currentSceneSettings.constantRespawnPoint;
                        if (constantRespawnPoint != Vector3.zero)
                        {
                            __instance._pms.OnPlayerClassChange(constantRespawnPoint, 0f);
                            __instance._pms.IsAFK = true;
                        }
                        else
                        {
                            GameObject randomPosition =
                                CharacterClassManager._spawnpointManager.GetRandomPosition(__instance.CurClass);

                            Vector3 spawnPoint;
                            float rotY;
                            if (randomPosition != null)
                            {
                                spawnPoint = randomPosition.transform.position;
                                rotY = randomPosition.transform.rotation.eulerAngles.y;

                                AmmoBox component1 = __instance._hub.ammoBox;
                                if (escape && CharacterClassManager.KeepItemsAfterEscaping)
                                {
                                    Inventory component2 = ReferenceHub.HostHub.inventory;
                                    for (ushort index = 0; index < 3; ++index)
                                    {
                                        if (component1[index] >= 15U)
                                        {
                                            component2.SetPickup(component1.types[index].inventoryID, component1[index], randomPosition.transform.position, randomPosition.transform.rotation, 0, 0, 0);
                                        }
                                    }
                                }

                                component1.ResetAmmo();
                            }
                            else
                            {
                                spawnPoint = __instance.DeathPosition;
                                rotY = 0f;
                            }

                            var ev = new SpawningEventArgs(API.Features.Player.Get(__instance.gameObject), __instance.CurClass, spawnPoint, rotY);

                            Player.OnSpawning(ev);

                            __instance._pms.OnPlayerClassChange(ev.Position, ev.RotationY);
                        }

                        if (!__instance.SpawnProtected
                            && CharacterClassManager.EnableSP
                            && CharacterClassManager.SProtectedTeam.Contains((int)role.team))
                        {
                            __instance.GodMode = true;
                            __instance.SpawnProtected = true;
                            __instance.ProtectedTime = Time.time;
                        }
                    }

                    if (!__instance.isLocalPlayer)
                    {
                        __instance._hub.playerStats.maxHP = role.maxHP;
                    }
                }

                __instance.Scp0492.iAm049_2 = __instance.CurClass == RoleType.Scp0492;
                __instance.Scp106.iAm106 = __instance.CurClass == RoleType.Scp106;
                __instance.Scp173.iAm173 = __instance.CurClass == RoleType.Scp173;
                __instance.Scp939.iAm939 = __instance.CurClass.Is939();
                __instance.RefreshPlyModel();

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"({typeof(Spawning).FullName}.{nameof(Prefix)}):\n{e}");

                return true;
            }
        }
    }
}
