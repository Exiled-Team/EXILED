using System;
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

			try
			{
				bool allow = true;
				Events.InvokeSetGroup(__instance.gameObject, ref group, ref allow);
				return allow;
			}
			catch (Exception e)
			{
				Log.Error($"SetGroup Error: {e}");
				return true;
			}
		}
	}
}
