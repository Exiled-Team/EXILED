using Harmony;
using System;
using UnityEngine;
using Console = GameCore.Console;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.CallCmdInteract))]
	public class Scp079TriggerTeslaEvent
	{
		public static bool Prefix(Scp079PlayerScript __instance, string command, GameObject target)
		{
			if (EventPlugin.Scp079TriggerTeslaPatchDisable)
				return true;

			try
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

				if (s != "TESLA")
					return true;
				float manaFromLabel2 = __instance.GetManaFromLabel("Tesla Gate Burst", __instance.abilities);
				if (manaFromLabel2 > (double)__instance.curMana)
				{
					__instance.RpcNotEnoughMana(manaFromLabel2, __instance.curMana);
					return false;
				}

				GameObject go1 = GameObject.Find(__instance.currentZone + "/" + __instance.currentRoom + "/Gate");
				if (!(go1 != null))
					return false;

				bool allow = true;
				Events.InvokeScp079TriggerTesla(__instance.gameObject, ref allow);
				if (!allow)
					return false;

				go1.GetComponent<TeslaGate>().RpcInstantBurst();
				__instance.AddInteractionToHistory(go1, strArray[0], true);
				__instance.Mana -= manaFromLabel2;
				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"Scp079TriggerTeslaEvent error: {exception}");
				return true;
			}
		}
	}
}