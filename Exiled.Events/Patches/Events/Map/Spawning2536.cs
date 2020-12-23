// -----------------------------------------------------------------------
// <copyright file="Spawning2536.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Linq;
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="SCP_2536_Controller.SelectAndSpawnTree"/>.
    /// Adds the <see cref="Map.Spawning2536"/> and <see cref="Map.Spawned2536"/> events.
    /// </summary>
    [HarmonyPatch(typeof(SCP_2536_Controller), nameof(SCP_2536_Controller.SelectAndSpawnTree))]
    internal static class Spawning2536
    {
        private static bool Prefix(SCP_2536_Controller __instance)
        {
            __instance.PlayersToChoose.Clear();
            foreach (KeyValuePair<GameObject, ReferenceHub> keyValuePair in ReferenceHub.GetAllHubs())
            {
                if (keyValuePair.Value.characterClassManager.CurClass != RoleType.Spectator && keyValuePair.Value.characterClassManager.CurClass != RoleType.None && keyValuePair.Value.characterClassManager.CurRole.team != Team.SCP && !(keyValuePair.Value.localCurrentRoomEffects == null) && !(keyValuePair.Value.localCurrentRoomEffects.PlayersCurrentRoom == null) && !keyValuePair.Value.isDedicatedServer && keyValuePair.Value.Ready && !__instance.PlayersAlreadyChosen.Contains(keyValuePair.Value.characterClassManager.UserId) && keyValuePair.Value.transform.position.y < 900f)
                {
                    __instance.PlayersToChoose.Add(keyValuePair.Value);
                }
            }

            if (__instance.PlayersToChoose.Count == 0)
            {
                return false;
            }

            __instance.Cooldown = __instance.CooldownAmount;
            ReferenceHub referenceHub = __instance.PlayersToChoose[Random.Range(0, __instance.PlayersToChoose.Count)];
            RoomInformation.ZoneType currentZoneType = referenceHub.localCurrentRoomEffects.PlayersCurrentRoom.CurrentZoneType;
            if (currentZoneType == RoomInformation.ZoneType.LCZ || currentZoneType == RoomInformation.ZoneType.ENTRANCE || currentZoneType == RoomInformation.ZoneType.HCZ)
            {
                SCP2536_Spawn_Location scp2536_Spawn_Location = null;
                foreach (SCP2536_Spawn_Location scp2536_Spawn_Location2 in referenceHub.localCurrentRoomEffects.LastRoomIn.GetComponentsInChildren<SCP2536_Spawn_Location>())
                {
                    if (referenceHub.localCurrentRoomEffects.PlayersCurrentRoom.CurrentRoomType == RoomInformation.RoomType.HCZ_TESLA)
                    {
                        if (scp2536_Spawn_Location == null)
                        {
                            scp2536_Spawn_Location = scp2536_Spawn_Location2;
                        }
                        else if ((scp2536_Spawn_Location.transform.position - referenceHub.playerMovementSync.RealModelPosition).sqrMagnitude < (scp2536_Spawn_Location2.transform.position - referenceHub.playerMovementSync.RealModelPosition).sqrMagnitude)
                        {
                            scp2536_Spawn_Location = scp2536_Spawn_Location2;
                        }
                    }
                    else if (scp2536_Spawn_Location == null)
                    {
                        scp2536_Spawn_Location = scp2536_Spawn_Location2;
                    }
                    else if ((scp2536_Spawn_Location.transform.position - referenceHub.playerMovementSync.RealModelPosition).sqrMagnitude > (scp2536_Spawn_Location2.transform.position - referenceHub.playerMovementSync.RealModelPosition).sqrMagnitude)
                    {
                        scp2536_Spawn_Location = scp2536_Spawn_Location2;
                    }
                }

                Spawning2536EventArgs ev = new Spawning2536EventArgs(API.Features.Player.Get(referenceHub), scp2536_Spawn_Location);
                Map.OnSpawning2536(ev);

                if (!ev.IsAllowed)
                {
                    return false;
                }

                __instance.PlayersAlreadyChosen.Add(referenceHub.characterClassManager.UserId);

                List<SCP2536_Present> presentList = ListPool<SCP2536_Present>.Shared.Rent();

                scp2536_Spawn_Location.IsTreeActive = true;
                scp2536_Spawn_Location.RpcSetTreeState(true);
                SCP2536_Present[] componentsInChildren2 = scp2536_Spawn_Location.GetComponentsInChildren<SCP2536_Present>();
                List<SCP_2536_Controller.Valid2536Scenario> list = __instance.GetAllValid2536Scenarios(ev.Player.ReferenceHub);
                for (int j = 0; j < 3; j++)
                {
                    componentsInChildren2[j].RpcResetPresent();
                    componentsInChildren2[j].ThisPresentsScenario = list[UnityEngine.Random.Range(0, list.Count)];
                    presentList.Add(componentsInChildren2[j]);
                    if (list.Count > 1)
                    {
                        list.Remove(componentsInChildren2[j].ThisPresentsScenario);
                    }
                }

                Spawned2536EventArgs ev2 = new Spawned2536EventArgs(API.Features.Player.Get(referenceHub), scp2536_Spawn_Location, presentList);
                Map.OnSpawned2536(ev2);

                ListPool<SCP2536_Present>.Shared.Return(presentList);
            }

            return false;
        }
    }
}
