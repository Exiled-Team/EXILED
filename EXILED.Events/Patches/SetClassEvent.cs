using System;
using EXILED.Shared;
using Harmony;

namespace EXILED.Events.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), "SetClassID")]
	public class SetClassEvent
	{
		public static void Postfix(CharacterClassManager __instance, RoleType id)
		{
			if (EventPlugin.SetClassPatchDisable)
				return;

			try
			{
				Events.Events.InvokeSetClass(__instance, id);
			}
			catch (Exception e)
			{
				Plugin.Error($"SetClass event error: {e}");
			}
		}
	}
}