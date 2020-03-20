using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.AddLog))]
	public class WaitingForPlayersEvent
	{
		public static void Prefix(ref string q)
		{
			if (EventPlugin.WaitingForPlayersPatchDisable)
				return;

			try
			{
				if (q == "Waiting for players..")
				{
					q += ".";

					Events.InvokeWaitingForPlayers();
				}
			}
			catch (Exception exception)
			{
				Log.Error($"WaitingForPlayersEvent error: {exception}");
			}
		}
	}
}