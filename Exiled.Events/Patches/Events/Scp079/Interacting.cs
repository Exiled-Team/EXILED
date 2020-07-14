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
    using Exiled.Events.EventArgs;
    using GameCore;
    using HarmonyLib;
    using UnityEngine;
    using Console = GameCore.Console;
    using Log = Exiled.API.Features.Log;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.CallCmdInteract(string, GameObject)"/>.
    /// Adds the <see cref="InteractingTeslaEventArgs"/> and <see cref="InteractingDoorEventArgs"/> event for SCP-079.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.CallCmdInteract))]
    public class Interacting
    {
        /// <summary>
        /// Prefix of <see cref="Scp079PlayerScript.CallCmdInteract(string, GameObject)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Scp079PlayerScript"/> instance.</param>
        /// <param name="command">The command to be executed.</param>
        /// <param name="target">The target game object.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Scp079PlayerScript __instance, string command, GameObject target)
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

                List<string> list = ConfigFile.ServerConfig.GetStringList("scp079_door_blacklist") ??
                                    new List<string>();
                string s = array[0];

                switch (s)
                {
                    case "TESLA":
                    {
                        float manaFromLabel = __instance.GetManaFromLabel("Tesla Gate Burst", __instance.abilities);
                        if (manaFromLabel > __instance.curMana)
                        {
                            __instance.RpcNotEnoughMana(manaFromLabel, __instance.curMana);
                            return false;
                        }

                        GameObject gameObject =
                            GameObject.Find(__instance.currentZone + "/" + __instance.currentRoom + "/Gate");
                        if (gameObject != null)
                        {
                            gameObject.GetComponent<TeslaGate>().RpcInstantBurst();
                            __instance.AddInteractionToHistory(gameObject, array[0], true);
                            __instance.Mana -= manaFromLabel;
                            return false;
                        }

                        return false;
                    }

                    case "DOOR":
                    {
                        if (AlphaWarheadController.Host.inProgress)
                        {
                            return false;
                        }

                        if (target == null)
                        {
                            Console.AddDebugLog("SCP079", "The door command requires a target.", MessageImportance.LessImportant);
                            return false;
                        }

                        Door component = target.GetComponent<Door>();
                        if (component == null)
                        {
                            return false;
                        }

                        if (list != null && list.Count > 0 && list != null && list.Contains(component.DoorName))
                        {
                            Console.AddDebugLog("SCP079", "Door access denied by the server.", MessageImportance.LeastImportant);
                            return false;
                        }

                        float manaFromLabel = __instance.GetManaFromLabel(
                            "Door Interaction " + (string.IsNullOrEmpty(component.permissionLevel)
                                ? "DEFAULT"
                                : component.permissionLevel), __instance.abilities);
                        if (manaFromLabel > __instance.curMana)
                        {
                            Console.AddDebugLog("SCP079", "Not enough mana.", MessageImportance.LeastImportant);
                            __instance.RpcNotEnoughMana(manaFromLabel, __instance.curMana);
                            return false;
                        }

                        if (component != null && component.ChangeState079())
                        {
                            __instance.Mana -= manaFromLabel;
                            __instance.AddInteractionToHistory(target, array[0], true);
                            Console.AddDebugLog("SCP079", "Door state changed.", MessageImportance.LeastImportant);
                            return true;
                        }

                        Console.AddDebugLog("SCP079", "Door state failed to change.", MessageImportance.LeastImportant);
                        return false;
                    }

                    default:
                        return true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Exiled.Events.Patches.Events.Scp079.Interacting: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
