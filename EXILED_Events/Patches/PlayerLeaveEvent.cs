using System;
using System.Linq;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ReferenceHub), "OnDestroy")]
	public class PlayerLeaveEvent
	{
		public static void Prefix(ReferenceHub __instance)
		{
			if (EventPlugin.PlayerLeaveEventPatchDisable)
				return;
			
			try
			{
				Events.InvokePlayerLeave(__instance, __instance.characterClassManager.UserId, __instance.gameObject);
			}
			catch (Exception e)
			{
				Plugin.Error($"Error in PlayerLeave Event: {e}");
			}
		}
	}
}
