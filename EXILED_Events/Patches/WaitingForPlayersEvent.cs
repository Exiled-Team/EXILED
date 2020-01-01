using EXILED;
using Harmony;

namespace JokersPlugin.Patches
{
	[HarmonyPatch(typeof (ServerConsole), "AddLog")]
	public class WaitingForPlayersEvent
	{
		public static void Prefix(string q)
		{
			if (EXILED.plugin.WaitingForPlayersPatchDisable)
				return;
			
			if (q == "Waiting for players..")
				Events.InvokeWaitingForPlayers();
		}
	}
}