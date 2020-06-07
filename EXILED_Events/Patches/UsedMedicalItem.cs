using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.SendRpc))]
	public class UsedMedicalItem
	{
		public static void Prefix(ConsumableAndWearableItems __instance, ConsumableAndWearableItems.HealAnimation healAnimation, int mid)
		{
			try
			{
				if (healAnimation == ConsumableAndWearableItems.HealAnimation.DequipMedicalItem)
					Events.InvokeUsedMedicalItem(__instance.gameObject, __instance.usableItems[mid].inventoryID);
			}
			catch (Exception exception)
			{
				Log.Error($"UsedMedicalItem error: {exception}");
				return;
			}
		}
	}
}
