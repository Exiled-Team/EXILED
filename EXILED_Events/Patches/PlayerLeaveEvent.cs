using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
	public class PlayerLeaveEvent
	{
		public static void Prefix(ReferenceHub __instance)
		{
			if (EventPlugin.PlayerLeaveEventPatchDisable)
				return;

			EventPlugin.ToMultiAdmin("Player disconnect: ");

			try
			{
				Events.InvokePlayerLeave(__instance);
			}
			catch (Exception exception)
			{
				Log.Error($"PlayerLeaveEvent error: {exception}");
			}
		}
	}
}
