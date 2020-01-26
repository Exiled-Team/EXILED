using Harmony;

namespace EXILED.Events.Patches
{
	[HarmonyPatch(typeof(PlayerStats), "Roundrestart")]
	public class RoundRestartEvent
	{
		public static void Prefix(PlayerStats __instance)
		{
			if (EventPlugin.RoundRestartEventPatchDisable)
				return;
			Events.Events.InvokeRoundRestart();
		}
	}
}