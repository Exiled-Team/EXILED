using Harmony;

namespace EXILED
{
	[HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.ReloadServerName))]
	public class ServerNamePatch
	{
		public static void Postfix()
		{
			if (!EventPlugin.NameTracking)
				return;

			ExiledVersion version = EventPlugin.Version;
			ServerConsole._serverName = ServerConsole._serverName.Replace("<size=1>SM119.0.0</size>", "");
			ServerConsole._serverName += $" <color=#00000000><size=1>SM119.{version.Major}.{version.Minor}.{version.Patch} (EXILED)</size></color>";
		}
	}
}