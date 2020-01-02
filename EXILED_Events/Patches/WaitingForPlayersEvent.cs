using System;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof (ServerConsole), "AddLog")]
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
				Plugin.Error($"WaitingForPlayers event error: {e}");
			}
		}
	}
}