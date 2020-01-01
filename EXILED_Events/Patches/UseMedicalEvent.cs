using EXILED;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ConsumableAndWearableItems), "CallCmdUseMedicalItem")]
	public class UseMedicalEvent
	{
		public static void Postfix(ConsumableAndWearableItems __instance)
		{
			if (EXILED.plugin.UseMedicalPatchDisable)
				return;
			
			Events.InvokeUseMedicalItem(__instance.gameObject, __instance.GetComponent<Inventory>().curItem);
		}
	}
}