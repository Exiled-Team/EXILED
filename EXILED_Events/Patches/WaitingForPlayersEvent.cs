using System;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof (ServerConsole), nameof(ServerConsole.AddLog))]
	public class WaitingForPlayersEvent
	{
		public static void Prefix(string q)
		{
			if (EventPlugin.WaitingForPlayersPatchDisable)
				return;

			try
			{
				if (q == "Waiting for players..")
					Events.InvokeWaitingForPlayers();
			}
			catch (Exception e)
			{
				Log.Error($"WaitingForPlayers event error: {e}");
			}
		}
	}
}