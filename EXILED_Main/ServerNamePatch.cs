using Harmony;
using Telepathy;

namespace EXILED
{
	[HarmonyPatch(typeof(ServerConsole), "ReloadServerName")]
	public class ServerNamePatch
	{
		public static void Postfix()
		{
			ServerConsole._serverName = ServerConsole._serverName.Replace("SM119.0.0", "");
			ServerConsole._serverName += " <size=1>SM119.1.0.0 (EXILED)</size>";
		}
	}
}