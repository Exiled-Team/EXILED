using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(DecontaminationLCZ), nameof(DecontaminationLCZ._KillPlayersInLCZ))]
	public class DecontaminationEvent
	{
		public static bool Prefix(DecontaminationLCZ __instance)
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