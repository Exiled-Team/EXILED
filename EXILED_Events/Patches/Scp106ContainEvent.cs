using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), "CallCmdContain106")]
	class Scp106ContainEvent
	{
		public static bool Prefix(CharacterClassManager __instance)
		{
			if (EventPlugin.Scp106ContainEventDisable)
				return true;

			bool allow = true;
			Plugin.Info("yes");
			Events.InvokeScp106ContainEvent(__instance.gameObject, ref allow);
			return allow;
		}
	}
}
