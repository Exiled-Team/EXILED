using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerConnect))]
	public class ConnectPatch
	{
		public static void Prefix()
		{
			Log.Info($"Player connect: ");
			if(PlayerManager.players.Count >= CustomNetworkManager.slots) 
				Log.Info($"Server full");
		}
	}

	[HarmonyPatch(typeof(CustomNetworkManager),nameof(CustomNetworkManager.OnServerDisconnect))]
	public class DisconnectPatch
	{
		public static void Prefix()
		{
			Log.Info("Player disconnect: ");
		}
	}
}