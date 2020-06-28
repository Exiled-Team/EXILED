using Harmony;
using System;
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
				bool isInHurtingRange = true;

				if (Vector3.Distance(__instance.transform.position, player.playerMovementSync.RealModelPosition) < __instance.sizeOfTrigger)
				{
					//memo: isInHurtingRange is not used after 10.0.0
					Events.InvokeTriggerTesla(player.gameObject, isInHurtingRange, ref triggerable);

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