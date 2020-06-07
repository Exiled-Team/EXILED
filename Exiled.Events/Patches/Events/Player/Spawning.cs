// -----------------------------------------------------------------------
// <copyright file="Spawning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.ApplyProperties(bool, bool)"/>.
    /// Adds the <see cref="Player.Spawning"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.ApplyProperties))]
    public class Spawning
    {
        /// <summary>
        /// Prefix of <see cref="CharacterClassManager.ApplyProperties(bool, bool)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="CharacterClassManager"/> instance.</param>
        /// <param name="lite">Indicates a value indicating whether items are going to be peserved or not.</param>
        /// <param name="escape">Indicates a value indicating whether the item is going to escape or not.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(CharacterClassManager __instance, bool lite = false, bool escape = false)
        {
            Role role = __instance.Classes.SafeGet(__instance.CurClass);
            if (!__instance._wasAnytimeAlive && __instance.CurClass != RoleType.Spectator && __instance.CurClass != RoleType.None)
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

            __instance.GetComponent<Inventory>();
            try
            {
                __instance.GetComponent<FootstepSync>().SetLoudness(role.team, role.roleId.Is939());
            }
            catch
            {
            }

            if (NetworkServer.active)
            {
                Handcuffs component = __instance.GetComponent<Handcuffs>();
                component.ClearTarget();
                component.NetworkCufferId = -1;
            }

            if (role.team != Team.RIP)
            {
                if (NetworkServer.active && !lite)
                {
                    Vector3 constantRespawnPoint = NonFacilityCompatibility.currentSceneSettings.constantRespawnPoint;
                    if (constantRespawnPoint != Vector3.zero)
                    {
                        __instance._pms.OnPlayerClassChange(constantRespawnPoint, 0f);
                    }
                    else
                    {
                        GameObject randomPosition = CharacterClassManager.SpawnpointManager.GetRandomPosition(__instance.CurClass);
                        Vector3 spawnPoint = new Vector3(0f, 0f, 0f);
                        float rotY = 0f;
                        if (randomPosition != null)
                        {
                            spawnPoint = randomPosition.transform.position;
                            rotY = randomPosition.transform.rotation.eulerAngles.y;
                            AmmoBox component1 = __instance.GetComponent<AmmoBox>();
                            if (escape && __instance.KeepItemsAfterEscaping)
                            {
                                Inventory component2 = PlayerManager.localPlayer.GetComponent<Inventory>();
                                for (ushort index = 0; index < 3; ++index)
                                {
                                    if (component1[index] >= 15U)
                                        component2.SetPickup(component1.types[index].inventoryID, component1[index], randomPosition.transform.position, randomPosition.transform.rotation, 0, 0, 0);
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

                    if (!__instance.SpawnProtected && __instance.EnableSP && __instance.SProtectedTeam.Contains((int)role.team))
                    {
                        __instance.GodMode = true;
                        __instance.SpawnProtected = true;
                        __instance.ProtectedTime = Time.time;
                    }
                }

                if (!__instance.isLocalPlayer)
                {
                    __instance.GetComponent<PlayerStats>().maxHP = role.maxHP;
                }
            }

            __instance.Scp0492.iAm049_2 = __instance.CurClass == RoleType.Scp0492;
            __instance.Scp106.iAm106 = __instance.CurClass == RoleType.Scp106;
            __instance.Scp173.iAm173 = __instance.CurClass == RoleType.Scp173;
            __instance.Scp939.iAm939 = __instance.CurClass.Is939();
            __instance.RefreshPlyModel();

            return false;
        }
    }
}
