using Harmony;

namespace EXILED
{
	[HarmonyPatch(typeof(ServerConsole), "ReloadServerName")]
	public class ServerNamePatch
	{
		public static void Postfix()
		{
			ServerConsole._serverName = ServerConsole._serverName.Replace("<size=1>SM119.0.0</size>", "");
			ServerConsole._serverName += " <size=1>SM119.1.0.2 (EXILED)</size>";
		}
	}
}