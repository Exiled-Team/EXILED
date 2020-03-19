using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(TeslaGate), nameof(TeslaGate.PlayersInRange))]
	public class TriggerTeslaEvent
	{
		public static bool Prefix(TeslaGate __instance, bool hurtRange) => EventPlugin.TriggerTeslaPatchDisable;

		public static void Postfix(TeslaGate __instance, bool hurtRange, ref List<PlayerStats> __result)
		{
			if (EventPlugin.TriggerTeslaPatchDisable)
				return;

			try
			{
				__result = new List<PlayerStats>();

				foreach (GameObject player in PlayerManager.players)
				{
					bool triggerable = true;

					if (Vector3.Distance(__instance.transform.position, player.transform.position) < __instance.sizeOfTrigger &&
						player.GetComponent<CharacterClassManager>().CurClass != RoleType.Spectator)
					{
						Events.InvokeTriggerTesla(player, hurtRange, ref triggerable);

						if (triggerable)
							__result.Add(player.GetComponent<PlayerStats>());
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error($"TriggerTeslaEvent error: {exception}");
			}
		}
	}
}