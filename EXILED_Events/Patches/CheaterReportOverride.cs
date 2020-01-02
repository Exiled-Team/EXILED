using System;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CheaterReport), "IssueReport")]
	public class CheaterReportOverride
	{
		public static bool Prefix(CheaterReport __instance, GameConsoleTransmission reporter, string reporterSteamId,
			string reportedSteamId, string reportedAuth, string reportedIp, string reporterAuth, string reporterIp,
			ref string reason, ref byte[] signature, string reporterPublicKey, int reportedId)
		{
			if (EventPlugin.CheaterReportPatchDisable)
				return true;

			try
			{
				if (reportedSteamId == reporterSteamId)
					reporter.SendToClient(__instance.connectionToClient,
						"You can't report yourself!" + Environment.NewLine, "yellow");

				int serverId = ServerConsole.Port;
				bool allow = true;
				Events.InvokeCheaterReport(reporterSteamId, reportedSteamId, reportedIp, reason, serverId, ref allow);

				return allow;
			}
			catch (Exception e)
			{
				Plugin.Error($"Cheater Report Patch error: {e}");
				return true;
			}
		}
	}
}