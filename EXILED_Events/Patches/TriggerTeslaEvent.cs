using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(TeslaGate), nameof(TeslaGate.PlayerInRange))]
	public class TriggerTeslaEvent
	{
		public static bool Prefix(TeslaGate __instance) => EventPlugin.TriggerTeslaPatchDisable;

		public static void Postfix(TeslaGate __instance, ReferenceHub player, ref bool __result)
		{
			if (EventPlugin.TriggerTeslaPatchDisable)
				return;

			try
			{
				bool triggerable = true;

				if (Vector3.Distance(__instance.transform.position, player.playerMovementSync.RealModelPosition) < __instance.sizeOfTrigger)
				{
					//memo: isInHurtingRange is gone
					Events.InvokeTriggerTesla(player.gameObject, true, ref triggerable);

					__result = triggerable;
				}
			}
			catch (Exception exception)
			{
				Log.Error($"TriggerTeslaEvent error: {exception}");
			}
		}
	}
}