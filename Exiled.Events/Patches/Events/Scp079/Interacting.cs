// -----------------------------------------------------------------------
// <copyright file="Interacting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
#pragma warning disable SA1313
#pragma warning disable CS0618
#pragma warning disable CS0436
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using GameCore;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using Console = GameCore.Console;
    using Log = Exiled.API.Features.Log;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.CallCmdInteract(string, GameObject)"/>.
    /// Adds the <see cref="InteractingTeslaEventArgs"/>, <see cref="InteractingDoorEventArgs"/>, <see cref="Handlers.Scp079.StartingSpeaker"/> and <see cref="Handlers.Scp079.StoppingSpeaker"/> event for SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.CallCmdInteract))]
    internal static class Interacting
    {
        /// <summary>
        /// Prefix of <see cref="Scp079PlayerScript.CallCmdInteract(string, GameObject)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Scp079PlayerScript"/> instance.</param>
        /// <param name="command">The command to be executed.</param>
        /// <param name="target">The target game object.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        private static bool Prefix(Scp079PlayerScript __instance, string command, GameObject target)
        {
            try
            {
                if (!__instance._interactRateLimit.CanExecute())
                {
                    return false;
                }

                if (!__instance.iAm079)
                {
                    return false;
                }

                Console.AddDebugLog("SCP079", "Command received from a client: " + command, MessageImportance.LessImportant);
                if (!command.Contains(":"))
                {
                    return false;
                }

                string[] array = command.Split(':');
                __instance.RefreshCurrentRoom();
                if (!__instance.CheckInteractableLegitness(__instance.currentRoom, __instance.currentZone, target, true))
                {
                    return false;
                }

                List<string> list = ListPool<string>.Shared.Rent();
                ConfigFile.ServerConfig.GetStringCollection("scp079_door_blacklist", list);

                bool result = true;
                switch (array[0])
                {
                    case "TESLA":
                        {
                            GameObject gameObject3 = GameObject.Find(__instance.currentZone + "/" + __instance.currentRoom + "/Gate");
                            if (gameObject3 == null)
                            {
                                result = false;
                                break;
                            }

                            Player player = Player.Get(__instance.gameObject);
                            TeslaGate teslaGate = gameObject3.GetComponent<TeslaGate>();
                            float apDrain = __instance.GetManaFromLabel("Tesla Gate Burst", __instance.abilities);
                            bool isAllowed = apDrain <= __instance.curMana;

                            InteractingTeslaEventArgs ev = new InteractingTeslaEventArgs(player, teslaGate, isAllowed);
                            Handlers.Scp079.OnInteractingTesla(ev);

                            if (!ev.IsAllowed)
                            {
                                if (apDrain > __instance.curMana)
                                {
                                    __instance.RpcNotEnoughMana(apDrain, __instance.curMana);
                                    result = false;
                                    break;
                                }
                            }
                            else
                            {
                                teslaGate.RpcInstantBurst();
                                __instance.AddInteractionToHistory(gameObject3, array[0], addMana: true);
                                __instance.Mana -= apDrain;
                                result = false;
                                break;
                            }

                            result = false;
                            break;
                        }

                    case "DOOR":
                        {
                            if (AlphaWarheadController.Host.inProgress)
                            {
                                result = false;
                                break;
                            }

                            if (target == null)
                            {
                                Console.AddDebugLog("SCP079", "The door command requires a target.", MessageImportance.LessImportant);
                                result = false;
                                break;
                            }

                            Door door = target.GetComponent<Door>();
                            if (door == null)
                            {
                                result = false;
                                break;
                            }

                            if (list != null && list.Count > 0 && list != null && list.Contains(door.DoorName))
                            {
                                Console.AddDebugLog("SCP079", "Door access denied by the server.", MessageImportance.LeastImportant);
                                result = false;
                                break;
                            }

                            Player player = Player.Get(__instance.gameObject);
                            float apDrain = __instance.GetManaFromLabel("Door Interaction " + (string.IsNullOrEmpty(door.permissionLevel) ? "DEFAULT" : door.permissionLevel), __instance.abilities);
                            bool isAllowed = apDrain <= __instance.curMana;

                            InteractingDoorEventArgs ev = new InteractingDoorEventArgs(player, door, isAllowed);
                            Handlers.Scp079.OnInteractingDoor(ev);

                            if (!ev.IsAllowed)
                            {
                                if (apDrain > __instance.curMana)
                                {
                                    Console.AddDebugLog("SCP079", "Not enough mana.", MessageImportance.LeastImportant);
                                    __instance.RpcNotEnoughMana(apDrain, __instance.curMana);
                                    result = false;
                                    break;
                                }
                            }
                            else if (door != null && door.ChangeState079())
                            {
                                __instance.Mana -= apDrain;
                                __instance.AddInteractionToHistory(target, array[0], addMana: true);
                                Console.AddDebugLog("SCP079", "Door state changed.", MessageImportance.LeastImportant);
                                result = false;
                                break;
                            }
                            else
                            {
                                Console.AddDebugLog("SCP079", "Door state failed to change.", MessageImportance.LeastImportant);
                            }

                            result = false;
                            break;
                        }

                    case "SPEAKER":
                        {
                            GameObject scp079SpeakerObject = GameObject.Find(__instance.currentZone + "/" + __instance.currentRoom + "/Scp079Speaker");
                            if (scp079SpeakerObject == null)
                            {
                                result = false;
                                break;
                            }

                            Player player = Player.Get(__instance.gameObject);
                            Room room = Map.FindParentRoom(__instance.currentCamera.gameObject);

                            float apDrain = __instance.GetManaFromLabel("Speaker Start", __instance.abilities);
                            bool isAllowed = apDrain * 1.5f <= __instance.curMana;

                            StartingSpeakerEventArgs ev = new StartingSpeakerEventArgs(player, room, apDrain, isAllowed);
                            Handlers.Scp079.OnStartingSpeaker(ev);

                            if (!ev.IsAllowed)
                            {
                                if (ev.APDrain * 1.5f > __instance.curMana)
                                {
                                    __instance.RpcNotEnoughMana(ev.APDrain, __instance.curMana);
                                    result = false;
                                    break;
                                }
                            }
                            else if (scp079SpeakerObject != null)
                            {
                                __instance.Mana -= ev.APDrain;
                                __instance.Speaker = __instance.currentZone + "/" + __instance.currentRoom + "/Scp079Speaker";
                                __instance.AddInteractionToHistory(scp079SpeakerObject, array[0], addMana: true);
                                result = false;
                                break;
                            }

                            result = false;
                            break;
                        }

                    case "STOPSPEAKER":
                        {
                            void ResetSpeaker() => __instance.Speaker = string.Empty;

                            // Somehow it can be empty
                            if (string.IsNullOrEmpty(__instance.Speaker))
                            {
                                ResetSpeaker();
                                result = false;
                                break;
                            }

                            string[] array7 = __instance.Speaker.Substring(0, __instance.Speaker.Length - 14).Split('/');

                            StoppingSpeakerEventArgs ev = new StoppingSpeakerEventArgs(
                                Player.Get(__instance.gameObject),
                                Map.FindParentRoom(__instance.currentCamera.gameObject));

                            Handlers.Scp079.OnStoppingSpeaker(ev);

                            if (ev.IsAllowed)
                            {
                                ResetSpeaker();
                                result = false;
                                break;
                            }

                            result = false;
                            break;
                        }

                    case "ELEVATORTELEPORT":
                        float manaFromLabel = __instance.GetManaFromLabel("Elevator Teleport", __instance.abilities);
                        global::Camera079 camera = null;
                        foreach (global::Scp079Interactable scp079Interactable in __instance.nearbyInteractables)
                        {
                            if (scp079Interactable.type == global::Scp079Interactable.InteractableType.ElevatorTeleport)
                            {
                                camera = scp079Interactable.optionalObject.GetComponent<global::Camera079>();
                            }
                        }

                        if (camera != null)
                        {
                            ElevatorTeleportEventArgs ev = new ElevatorTeleportEventArgs(Player.Get(__instance.gameObject), camera, manaFromLabel, manaFromLabel <= __instance.curMana);

                            Handlers.Scp079.OnElevatorTeleport(ev);

                            if (ev.IsAllowed)
                            {
                                __instance.RpcSwitchCamera(ev.Camera.cameraId, false);
                                __instance.Mana -= ev.APCost;
                                __instance.AddInteractionToHistory(target, array[0], true);
                            }
                            else
                            {
                                if (ev.APCost > __instance.curMana)
                                {
                                    __instance.RpcNotEnoughMana(manaFromLabel, __instance.curMana);
                                }
                            }
                        }

                        result = false;
                        break;

                    default:
                        result = true;
                        break;
                }

                ListPool<string>.Shared.Return(list);
                return result;
            }
            catch (Exception e)
            {
                Log.Error($"{typeof(Interacting).FullName}.{nameof(Prefix)}:\n{e}");

                return true;
            }
        }
    }
}
