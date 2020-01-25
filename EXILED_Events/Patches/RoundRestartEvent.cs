using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerStats), "RoundRestart")]
	public class RoundRestartEvent
	{
		public static void Prefix(PlayerStats __instance) => Events.InvokeRoundRestart();
	}
}