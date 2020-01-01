using EXILED;
using Harmony;

namespace JokersPlugin
{
	[HarmonyPatch(typeof(PlayerInteract), "CallCmdUsePanel")]
	public class WarheadLockPatch
	{
		public static bool Prefix(PlayerInteract __instance, string n)
		{
			if (EXILED.plugin.WarheadLockPatchDisable)
				return true;

			bool allow = true;
			Events.InvokeWarheadEvent(__instance, ref n, ref allow);
			return allow;
		}
	}
}