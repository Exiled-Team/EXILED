using EXILED.Extensions;
using MEC;
using UnityEngine;

namespace EXILED
{
	public static class ReloadCommandHandler
	{
		//Do not use this as plugin command handler, this is only meant to handle the EXILED reload command, handle commands similarly from within your plugin.
		public static void CommandHandler(ref RACommandEvent ev)
		{
			string[] args = ev.Command.Split(' ');

			switch (args[0].ToLower())
			{
				case "reloadplugins":
					ev.Allow = false;
					PluginManager.ReloadPlugins();
					ev.Sender.RAMessage("Reloading ploogins...");
					break;
				case "reloadconfigs":
					ev.Allow = false;
					Plugin.Config.Reload();
					ev.Sender.RAMessage("Configs have been reloaded!");
					break;
				case "reconnectrs":
					ev.Allow = false;
					PlayerManager.localPlayer.GetComponent<PlayerStats>()?.Roundrestart();
					Timing.CallDelayed(1.5f, Application.Quit);
					break;
			}
		}
	}
}