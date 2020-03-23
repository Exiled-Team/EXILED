using Harmony;
using System;
using Console = GameCore.Console;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Console), nameof(Console.TypeCommand), new Type[] { typeof(string), typeof(CommandSender) })]
	public class ServerRemoteAdminCommandEvent
	{
		public static bool Prefix(ref string cmd)
		{
			try
			{
				bool allow = true;

				CommandSender sender = Console._ccs;

				Events.InvokeCommand(ref cmd, ref sender, ref allow);

				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"RemoteAdminCommandEvent error: {exception}");
				return true;
			}

		}
	}
}
