using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetClassIDAdv))]
	public class SetClassEvent
	{
		public static void Prefix(CharacterClassManager __instance, RoleType id)
		{
			if (EventPlugin.SetClassPatchDisable)
				return;

			try
			{
				Events.InvokeSetClass(__instance, id);
			}
			catch (Exception exception)
			{
				Log.Error($"SetClassEvent error: {exception}");
			}
		}
	}
}