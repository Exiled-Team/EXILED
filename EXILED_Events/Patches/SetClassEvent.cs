using System;
using Harmony;

namespace EXILED.Patches
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
				Events.InvokeSetClass(__instance, id);
			}
			catch (Exception e)
			{
				Plugin.Error($"SetClass event error: {e}");
			}
		}
	}
}