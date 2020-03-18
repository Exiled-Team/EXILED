using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(RemoteAdmin.QueryProcessor), nameof(RemoteAdmin.QueryProcessor.ProcessGameConsoleQuery), new Type[] { typeof(string), typeof(bool) })]
	public class GameConsoleQueryOverride
	{
		public static bool Prefix(RemoteAdmin.QueryProcessor __instance, ref string query, ref bool encrypted)
		{
			try
			{
				if (EventPlugin.PlayerConsoleCommandPatchDisable)
					return true;

				Events.InvokeConsoleCommand(__instance.gameObject, query, encrypted, out string returnMessage, out string color);
				if (string.IsNullOrEmpty(color))
					color = "white";
				if (!string.IsNullOrEmpty(returnMessage))
					__instance.GCT.SendToClient(__instance.connectionToClient, returnMessage, color);
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"Console Command event error: {e}");
				return true;
			}
		}
	}
}
