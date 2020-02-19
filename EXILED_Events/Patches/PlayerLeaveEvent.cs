using System;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
	public class PlayerLeaveEvent
	{
		public static void Prefix(ReferenceHub __instance)
		{
			if (EventPlugin.PlayerLeaveEventPatchDisable)
				return;

			Log.Info("Player disconnect: ");

			try
			{
				Events.InvokePlayerLeave(__instance, __instance.characterClassManager.UserId, __instance.gameObject);
			}
			catch (Exception e)
			{
				Log.Error($"Error in PlayerLeave Event: {e}");
			}
		}
	}
}
