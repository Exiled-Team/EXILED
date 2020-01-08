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
				bool allow = true;
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