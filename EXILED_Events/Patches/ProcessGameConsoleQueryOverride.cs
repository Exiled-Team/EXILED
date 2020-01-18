using System;
using Harmony;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(RemoteAdmin.QueryProcessor), "ProcessGameConsoleQuery", new Type[] { typeof(string), typeof(bool) })]
    public class ProcessGameConsoleQueryOverride
    {
		public static bool Prefix(RemoteAdmin.QueryProcessor __instance, ref string query, ref bool encrypted)
		{
			try
			{
				ReferenceHub hub = ReferenceHub.GetHub(__instance.gameObject);
				Events.InvokeConsoleCommand(hub, query, encrypted, out string returnMessage, out string color);
				__instance.GCT.SendToClient(__instance.connectionToClient, returnMessage, color);
				return false;
			}
			catch (Exception e)
			{
				Plugin.Error($"Console Command event error: {e}");
				return true;
			}
		}
	}
}
