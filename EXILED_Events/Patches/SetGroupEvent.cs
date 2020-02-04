using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetGroup))]
	class SetGroupEvent
	{
		public static bool Prefix(ServerRoles __instance, UserGroup group)
		{
			if (EventPlugin.SetGroupEventDisable)
				return true;

			bool allow = true;
			Events.InvokeSetGroup(__instance.gameObject, ref group, ref allow);
			return allow;
		}
	}
}
