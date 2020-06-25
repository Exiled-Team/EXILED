// -----------------------------------------------------------------------
// <copyright file="EnteringPocketDimension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using CustomPlayerEffects;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using RemoteAdmin;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp106PlayerScript.CallCmdMovePlayer(GameObject, int)"/>.
    /// Adds the <see cref="Player.EnteringPocketDimension"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.CallCmdMovePlayer))]
    public class EnteringPocketDimension
    {
        /// <summary>
        /// Prefix of <see cref="Scp106PlayerScript.CallCmdMovePlayer(GameObject, int)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Scp106PlayerScript"/> instance.</param>
        /// <param name="ply">The player's game object.</param>
        /// <param name="t">The time.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Scp106PlayerScript __instance, GameObject ply, int t)
        {
            if (!__instance._iawRateLimit.CanExecute(true))
                return false;
            if (ply == null)
                return false;

            ReferenceHub hub = ReferenceHub.GetHub(ply);
            CharacterClassManager component = hub.characterClassManager;
            if (component == null)
                return false;
            if (!ServerTime.CheckSynchronization(t) || !__instance.iAm106 ||
                Vector3.Distance(
                    __instance.GetComponent<PlayerMovementSync>().RealModelPosition,
                    ply.transform.position) >= 3f || !component.IsHuman())
                return false;

            if (component.GodMode)
                return false;
            if (component.Classes.SafeGet(component.CurClass).team == Team.SCP)
                return false;

            __instance.GetComponent<CharacterClassManager>().RpcPlaceBlood(ply.transform.position, 1, 2f);
            if (Scp106PlayerScript._blastDoor.isClosed)
            {
                __instance.GetComponent<CharacterClassManager>().RpcPlaceBlood(ply.transform.position, 1, 2f);
                __instance.GetComponent<PlayerStats>().HurtPlayer(
                    new PlayerStats.HitInfo(
                        500f,
                        __instance.GetComponent<NicknameSync>().MyNick + " (" + __instance.GetComponent<CharacterClassManager>().UserId + ")",
                        DamageTypes.Scp106,
                        __instance.GetComponent<QueryProcessor>().PlayerId), ply);
            }
            else
            {
                CharacterClassManager component3 = ply.GetComponent<CharacterClassManager>();

                foreach (Scp079PlayerScript scp079PlayerScript in Scp079PlayerScript.instances)
                {
                    Scp079Interactable.ZoneAndRoom
                        otherRoom = ply.GetComponent<Scp079PlayerScript>().GetOtherRoom();
                    Scp079Interactable.InteractableType[] filter = new Scp079Interactable.InteractableType[]
                    {
                            Scp079Interactable.InteractableType.Door, Scp079Interactable.InteractableType.Light,
                            Scp079Interactable.InteractableType.Lockdown, Scp079Interactable.InteractableType.Tesla,
                            Scp079Interactable.InteractableType.ElevatorUse,
                    };
                    bool flag = false;
                    foreach (Scp079Interaction scp079Interaction in scp079PlayerScript.ReturnRecentHistory(
                        12f,
                        filter))
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
                        scp079PlayerScript.RpcGainExp(ExpGainType.PocketAssist, component3.CurClass);
                    }
                }

                var ev = new EnteringPocketDimensionEventArgs(API.Features.Player.Get(ply));

                Player.OnEnteringPocketDimension(ev);

                if (!ev.IsAllowed)
                    return false;

                ply.GetComponent<PlayerMovementSync>().OverridePosition(Vector3.down * 1998.5f, 0f, true);

                __instance.GetComponent<PlayerStats>().HurtPlayer(
                    new PlayerStats.HitInfo(
                        40f,
                        __instance.GetComponent<NicknameSync>().MyNick + " (" + __instance.GetComponent<CharacterClassManager>().UserId + ")",
                        DamageTypes.Scp106,
                        __instance.GetComponent<QueryProcessor>().PlayerId), ply);
            }

            PlayerEffectsController effectsController = hub.playerEffectsController;
            effectsController.GetEffect<Corroding>().IsInPd = true;
            effectsController.EnableEffect<Corroding>(0.0f, false);

            return false;
        }
    }
}
