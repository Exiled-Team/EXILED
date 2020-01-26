using Harmony;

namespace EXILED.Events.Patches
{
	[HarmonyPatch(typeof(DecontaminationLCZ), nameof(DecontaminationLCZ._KillPlayersInLCZ))]
	public class DecontaminationEvent
	{
		public static bool Prefix(DecontaminationLCZ __instance)
		{
			if (EventPlugin.DecontaminationEventPatchDisable)
				return true;
			
			bool allow = true;
			Events.Events.InvokeDecontamination(ref allow);
			return allow;
		}
	}
}