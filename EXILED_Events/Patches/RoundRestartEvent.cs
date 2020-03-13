using System;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.Roundrestart))]
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
				Events.InvokeRoundEnd();
			}
			catch (Exception e)
			{
				Log.Error($"RoundRestart Error: {e}");
			}
		}
	}
}