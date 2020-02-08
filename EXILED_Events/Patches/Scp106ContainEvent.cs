using System;
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

			try
			{
				bool allow = true;
				Events.InvokeScp106ContainEvent(__instance.gameObject, ref allow);
				return allow;
			}
			catch (Exception e)
			{
				Log.Error($"SCP106Contain Error: {e}");
				return true;
			}
		}
	}
}
