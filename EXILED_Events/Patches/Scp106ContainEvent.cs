using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdContain106))]
	public class Scp106ContainEvent
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
			catch (Exception exception)
			{
				Log.Error($"Scp106ContainEvent error: {exception}");
				return true;
			}
		}
	}
}
