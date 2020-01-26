using System;
using EXILED.Shared.Helpers;
using Harmony;

namespace EXILED.Events.Patches
{
	public class PlayerLeaveEvent
	{
		[HarmonyPatch(typeof(ReferenceHub), "OnDestroy")]
		public static void Prefix(ReferenceHub __instance)
		{
			if (EventPlugin.PlayerLeaveEventPatchDisable)
				return;
			
			try
			{
				Events.Events.InvokePlayerLeave(__instance, __instance.characterClassManager.UserId, __instance.gameObject);
			}
			catch (Exception e)
			{
				LogHelper.Error($"Error in PlayerLeave Event: {e}");
			}
		}
	}
}