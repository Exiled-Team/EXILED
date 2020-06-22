// -----------------------------------------------------------------------
// <copyright file="InteractingTesla.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Exiled.Events.Patches.Events.Scp079
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using UnityEngine;
    using Console = GameCore.Console;

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
            if (!__instance._interactRateLimit.CanExecute() || !__instance.iAm079)
					return false;
				Console.AddDebugLog("SCP079", "Command received from a client: " + command,
					MessageImportance.LessImportant);
				if (!command.Contains(":"))
					return false;
				string[] strArray = command.Split(':');
				__instance.RefreshCurrentRoom();
				if (!__instance.CheckInteractableLegitness(__instance.currentRoom, __instance.currentZone, target,
					true))
					return false;

				string s = strArray[0];

				if (s == "TESLA")
				{
					float manaFromLabel2 = __instance.GetManaFromLabel("Tesla Gate Burst", __instance.abilities);
					if (manaFromLabel2 > (double)__instance.curMana)
					{
						__instance.RpcNotEnoughMana(manaFromLabel2, __instance.curMana);
						return false;
					}

					GameObject go1 = GameObject.Find(__instance.currentZone + "/" + __instance.currentRoom + "/Gate");
					if (!(go1 != null))
						return false;

                    var ev = new InteractingTeslaEventArgs(API.Features.Player.Get(__instance.gameObject), go1.GetComponent<TeslaGate>());

                    Scp079.OnInteractingTesla(ev);

                    if (!ev.IsAllowed)
                        return false;

					go1.GetComponent<TeslaGate>().RpcInstantBurst();
					__instance.AddInteractionToHistory(go1, strArray[0], true);
					__instance.Mana -= manaFromLabel2;
					return false;
				}

				if (s == "DOOR")
				{
					List<string> list = GameCore.ConfigFile.ServerConfig.GetStringList("scp079_door_blacklist") ?? new List<string>();
					if (AlphaWarheadController.Host.inProgress)
					{
						return false;
					}
					if (target == null)
					{
						GameCore.Console.AddDebugLog("SCP079", "The door command requires a target.", MessageImportance.LessImportant, false);
						return false;
					}
					Door component = target.GetComponent<Door>();
					if (component == null)
					{
						return false;
					}
					if (list != null && list.Count > 0 && list != null && list.Contains(component.DoorName))
					{
						GameCore.Console.AddDebugLog("SCP079", "Door access denied by the server.", MessageImportance.LeastImportant, false);
						return false;
					}
					float manaFromLabel = __instance.GetManaFromLabel("Door Interaction " + (string.IsNullOrEmpty(component.permissionLevel) ? "DEFAULT" : component.permissionLevel), __instance.abilities);
					if (manaFromLabel > __instance.curMana)
					{
						GameCore.Console.AddDebugLog("SCP079", "Not enough mana.", MessageImportance.LeastImportant, false);
						__instance.RpcNotEnoughMana(manaFromLabel, __instance.curMana);
						return false;
					}
					if (component == null)
						return false;
                    var ev = new InteractingDoorEventArgs(API.Features.Player.Get(__instance.gameObject), component);

                    Scp079.OnInteractingDoor(ev);

                    if (!ev.IsAllowed)
                        return false;

					if (component.ChangeState079())
					{
						__instance.Mana -= manaFromLabel;
						__instance.AddInteractionToHistory(target, s, true);
						GameCore.Console.AddDebugLog("SCP079", "Door state changed.", MessageImportance.LeastImportant, false);
						return false;
					}
					GameCore.Console.AddDebugLog("SCP079", "Door state failed to change.", MessageImportance.LeastImportant, false);
					return false;
				}
				return false;
        }
    }
}
