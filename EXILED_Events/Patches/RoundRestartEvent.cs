using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.Roundrestart))]
	public class RoundRestartEvent
	{
		public static void Prefix(PlayerStats __instance)
		{
			if (EventPlugin.RoundRestartEventPatchDisable)
				return;

			EventPlugin.ToMultiAdmin("Round restarting");

			try
			{
				Events.InvokeRoundRestart();
				Events.InvokeRoundEnd();
			}
			catch (Exception exception)
			{
				Log.Error($"RoundRestartEvent/RoundEndEvent error: {exception}");
			}
		}
	}
}