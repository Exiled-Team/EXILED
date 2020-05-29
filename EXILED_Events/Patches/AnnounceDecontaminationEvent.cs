using Harmony;
using System;
using LightContainmentZoneDecontamination;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.UpdateSpeaker))]
	public class AnnounceDecontaminationEvent
	{
		public static bool Prefix(DecontaminationController __instance, ref bool hard)
		{
			try
			{
				bool allow = true;
				int id = __instance.nextPhase;

				Events.InvokeAnnounceDecontamination(ref id, ref hard, ref allow);

				__instance.nextPhase = id;
				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"AnnounceDecontaminationEvent error: {exception}");
				return true;
			}
		}
	}
}
