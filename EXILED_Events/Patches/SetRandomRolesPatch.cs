using Harmony;
using MEC;
using Mirror;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetRandomRoles))]
	public class SetRandomRolesPatch
	{
		public static bool Prefix(CharacterClassManager __instance)
		{
			if (EventPlugin.SetRandomRolesPatchDisable)
				return true;

			try
			{
				if (__instance.isLocalPlayer && __instance.isServer)
					__instance.RunSmartClassPicker();

				if (NetworkServer.active)
					Timing.RunCoroutine(__instance.MakeSureToSetHP(), Segment.FixedUpdate);

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"SetRandomRolesPatch error: {exception}");
				return true;
			}
		}
	}
}