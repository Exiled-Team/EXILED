using System;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerStats), "Roundrestart")]
	public class RoundRestartEvent
	{
		public static void Prefix(PlayerStats __instance)
		{
			if (EventPlugin.RoundRestartEventPatchDisable)
				return;

			Log.Info("Round restarting");

			try
			{
				Events.InvokeRoundRestart();
			}
			catch (Exception e)
			{
				Log.Error($"RoundRestart Error: {e}");
			}
		}
	}
}