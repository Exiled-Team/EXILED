using System;
using Harmony;
using RemoteAdmin;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof (CommandProcessor), "ProcessQuery", typeof (string), typeof (CommandSender))]
	public class OnCommandPatch
	{
		public static bool Prefix(ref string q, ref CommandSender sender)
		{
			try
			{
				QueryProcessor queryProcessor = sender is PlayerCommandSender playerCommandSender ? playerCommandSender.Processor : null;
				bool allow = true;
				
				if (q.ToLower() == "gban-kick")
				{
					if (queryProcessor == null || !queryProcessor._sender.SR.RaEverywhere)
					{
						sender.RaReply(
							$"GBAN-KICK# Permission to run command denied by the server. If this is an unexpected error, contact EXILED developers.",
							false, true, string.Empty);
						Plugin.Error(
							$"A user {sender.Nickname} attempted to run GBAN-KICK and was denied permission. If this is an unexpected error, contact EXILED developers.");
						allow = false;
					}
				}
				
				Events.InvokeCommand(ref q, ref sender, ref allow);
				return allow;
			}
			catch (Exception e)
			{
				Plugin.Error($"RA Command event error: {e}");
				return true;
			}
		}
	}
}