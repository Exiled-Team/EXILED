using EXILED;
using Harmony;

namespace JokersPlugin.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), "SetClassID")]
	public class SetClassEvent
	{
		public static void Postfix(CharacterClassManager __instance, RoleType id)
		{
			if (EXILED.plugin.SetClassPatchDisable)
				return;
			
			Events.InvokeSetClass(__instance, id);
		}
	}
}