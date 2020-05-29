using Harmony;
using System;
using LightContainmentZoneDecontamination;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.FinishDecontamination))]
	public class DecontaminationEvent
	{
		public static bool Prefix(DecontaminationController __instance)
		{
			if (EventPlugin.DecontaminationEventPatchDisable)
				return true;

			try
			{
				bool allow = true;

				Events.InvokeDecontamination(ref allow);

				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"DecontaminationEvent error: {exception}");
				return true;
			}
		}
	}
}