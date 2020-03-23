using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(RemoteAdmin.QueryProcessor), nameof(RemoteAdmin.QueryProcessor.ProcessGameConsoleQuery), new Type[] { typeof(string), typeof(bool) })]
	public class ConsoleCommandEvent
	{
		public static bool Prefix(RemoteAdmin.QueryProcessor __instance, ref string query, ref bool encrypted)
		{
			if (EventPlugin.PlayerConsoleCommandPatchDisable)
				return true;

			try
			{
				Events.InvokeConsoleCommand(__instance.gameObject, query, encrypted, out string returnMessage, out string color);

				if (string.IsNullOrEmpty(color))
					color = "white";

				if (!string.IsNullOrEmpty(returnMessage))
					__instance.GCT.SendToClient(__instance.connectionToClient, returnMessage, color);

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"ConsoleCommandEvent error: {exception}");
				return true;
			}
		}
	}
}
