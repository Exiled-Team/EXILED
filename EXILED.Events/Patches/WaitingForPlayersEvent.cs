using System;
using EXILED.Shared.Helpers;
using Harmony;

namespace EXILED.Events.Patches
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
					Events.Events.InvokeWaitingForPlayers();
			}
			catch (Exception e)
			{
				LogHelper.Error($"WaitingForPlayers event error: {e}");
			}
		}
	}
}