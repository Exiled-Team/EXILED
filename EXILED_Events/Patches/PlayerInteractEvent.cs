using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.OnInteract))]
	public class PlayerInteractEvent
	{
		public static void Prefix(PlayerInteract __instance)
		{
			try
			{
				Events.InvokePlayerInteract(__instance.gameObject);
			}
			catch (Exception exception)
			{
				Log.Error($"PlayerInteractEvent error: {exception}");
			}
		}
	}
}
