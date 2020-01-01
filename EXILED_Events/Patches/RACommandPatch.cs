using System;
using Harmony;
using RemoteAdmin;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof (CommandProcessor), "ProcessQuery", new Type[] {typeof (string), typeof (CommandSender)})]
	public class OnCommandPatch
	{
		public static bool Prefix(ref string q, ref CommandSender sender)
		{
			bool allow = true;
			Events.InvokeCommand(ref q, ref sender, ref allow);
			return allow;
		}
	}
}