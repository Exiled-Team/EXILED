using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), "CallCmdUsePanel")]
	public class WarheadLockPatch
	{
		public static bool Prefix(PlayerInteract __instance, string n)
		{
			if (plugin.WarheadLockPatchDisable)
				return true;

			bool allow = true;
			Events.InvokeWarheadEvent(__instance, ref n, ref allow);
			return allow;
		}
	}
}