using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(AnimationController), nameof(AnimationController.CallCmdSyncData))]
	public class SyncDataEvent
	{
		public static bool Prefix(AnimationController __instance, int state, Vector2 v2)
		{
			if (EventPlugin.CmdSyncDataEventDisable)
				return true;

			try
			{
				if (!__instance._mSyncRateLimit.CanExecute(false))
					return false;

				bool allow = true;

				Events.InvokeSyncData(__instance.gameObject, ref state, ref v2, ref allow);

				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"SyncDataEvent error: {exception}");
				return true;
			}
		}

	}
}
