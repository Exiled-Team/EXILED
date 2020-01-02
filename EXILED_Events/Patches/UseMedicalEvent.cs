using System;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ConsumableAndWearableItems), "CallCmdUseMedicalItem")]
	public class UseMedicalEvent
	{
		public static void Postfix(ConsumableAndWearableItems __instance)
		{
			if (EventPlugin.UseMedicalPatchDisable)
				return;

			try
			{
				Events.InvokeUseMedicalItem(__instance.gameObject, __instance.GetComponent<Inventory>().curItem);
			}
			catch (Exception e)
			{
				Plugin.Error($"UseMedicalItem event error: {e}");
			}
		}
	}
}