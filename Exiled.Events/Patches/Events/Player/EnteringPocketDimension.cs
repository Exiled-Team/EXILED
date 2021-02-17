// -----------------------------------------------------------------------
// <copyright file="EnteringPocketDimension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using CustomPlayerEffects;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp106PlayerScript.CallCmdMovePlayer(GameObject, int)"/>.
    /// Adds the <see cref="Player.EnteringPocketDimension"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.CallCmdMovePlayer))]
    internal static class EnteringPocketDimension
    {
        private static bool Prefix(Scp106PlayerScript __instance, GameObject ply, int t)
        {
            try
            {
                if (!__instance._iawRateLimit.CanExecute(true) || ply == null)
                    return false;

                ReferenceHub hub = ReferenceHub.GetHub(ply);
                CharacterClassManager ccm = hub != null ? hub.characterClassManager : null;

                if (ccm == null)
                    return false;

                if (!ServerTime.CheckSynchronization(t)
                    || !__instance.iAm106
                    || Vector3.Distance(hub.playerMovementSync.RealModelPosition, ply.transform.position) >= 3f
                    || !ccm.IsHuman()
                    || ccm.GodMode
                    || ccm.CurRole.team == Team.SCP)
                {
                    return false;
                }

                var instanceHub = ReferenceHub.GetHub(__instance.gameObject);
                instanceHub.characterClassManager.RpcPlaceBlood(ply.transform.position, 1, 2f);
                __instance.TargetHitMarker(__instance.connectionToClient);

                if (Scp106PlayerScript._blastDoor.isClosed)
                {
                    instanceHub.characterClassManager.RpcPlaceBlood(ply.transform.position, 1, 2f);
                    instanceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(500f, instanceHub.LoggedNameFromRefHub(), DamageTypes.Scp106, instanceHub.playerId), ply);
                }
                else
                {
                    Scp079Interactable.ZoneAndRoom otherRoom = hub.scp079PlayerScript.GetOtherRoom();
                    Scp079Interactable.InteractableType[] filter = new Scp079Interactable.InteractableType[]
                    {
                            Scp079Interactable.InteractableType.Door, Scp079Interactable.InteractableType.Light,
                            Scp079Interactable.InteractableType.Lockdown, Scp079Interactable.InteractableType.Tesla,
                            Scp079Interactable.InteractableType.ElevatorUse,
                    };

                    foreach (Scp079PlayerScript scp079PlayerScript in Scp079PlayerScript.instances)
                    {
                        bool flag = false;

                        foreach (Scp079Interaction scp079Interaction in scp079PlayerScript.ReturnRecentHistory(12f, filter))
                        {
                            foreach (Scp079Interactable.ZoneAndRoom zoneAndRoom in scp079Interaction.interactable
                                .currentZonesAndRooms)
                            {
                                if (zoneAndRoom.currentZone == otherRoom.currentZone &&
                                    zoneAndRoom.currentRoom == otherRoom.currentRoom)
                                {
                                    flag = true;
                                }
                            }
                        }

                        if (flag)
                        {
                            scp079PlayerScript.RpcGainExp(ExpGainType.PocketAssist, ccm.CurClass);
                        }
                    }

                    var ev = new EnteringPocketDimensionEventArgs(API.Features.Player.Get(ply), Vector3.down * 1998.5f, API.Features.Player.Get(instanceHub));

                    Player.OnEnteringPocketDimension(ev);

                    if (!ev.IsAllowed)
                        return false;

                    hub.playerMovementSync.OverridePosition(ev.Position, 0f, true);

                    instanceHub.playerStats.HurtPlayer(new PlayerStats.HitInfo(40f, instanceHub.LoggedNameFromRefHub(), DamageTypes.Scp106, instanceHub.playerId), ply);
                }

                PlayerEffectsController effectsController = hub.playerEffectsController;
                effectsController.GetEffect<Corroding>().IsInPd = true;
                effectsController.EnableEffect<Corroding>(0.0f, false);

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"{typeof(EnteringPocketDimension).FullName}:\n{e}");

                return true;
            }
        }
    }
}
