using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(MuteHandler), nameof(MuteHandler.Reload))]
	public class MuteHandlerFix
	{
		public static void Prefix() => MuteHandler.Mutes?.Clear();
	}
}