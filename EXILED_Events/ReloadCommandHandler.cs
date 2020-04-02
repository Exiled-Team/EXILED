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
				case "listplugins":
					ev.Allow = false;
					var result = "";
					var count = PluginManager._plugins.Count;
					var active = PluginManager._active.Count;
					for (var i = 0; i < count; i++) {
						var plugin = PluginManager._plugins[i];
						result += $"<color={(PluginManager._active.Contains(plugin) ? "green" : "red")}>{plugin.getName}</color>";
					}
					ev.Sender.RAMessage($"Ploogins list ({active}/{count}):\n{string.Join("  ", result)}");
					break;
				
				case "enableplugin":
					ev.Allow = false;
					OnEnableCommand(args, ev.Sender);
					break;
				
				case "disableplugin":
					ev.Allow = false;
					OnDisableCommand(args, ev.Sender);
					break;
				
				case "reloadplugin":
					ev.Allow = false;
					OnReloadCommand(args, ev.Sender);
					break;
				
				case "reloadallplugins":
					ev.Allow = false;
					PluginManager.ReloadAllPlugins();
					ev.Sender.RAMessage("Reloading ploogins...");
					break;
				
				case "reconnectrs":
					ev.Allow = false;
					PlayerManager.localPlayer.GetComponent<PlayerStats>()?.Roundrestart();
					Timing.CallDelayed(1.5f, Application.Quit);
					break;
			}
		}

		private static void OnEnableCommand(string[] args, CommandSender sender)
		{
			if (args.Length < 2)
			{
				sender.RAMessage("Enter ploogin name");
				return;
			}

			var plugin = FindPloogin(args[1]);
			if (plugin == null)
			{
				sender.RAMessage("Ploogin not found");
				return;
			}

			if (PluginManager._active.Contains(plugin))
			{
				sender.RAMessage("Ploogin already enabled");
				return;
			}
					
			PluginManager.EnablePlugin(plugin);
			sender.RAMessage("Enabling ploogin...");
		}

		private static void OnDisableCommand(string[] args, CommandSender sender)
		{
			if (args.Length < 2)
			{
				sender.RAMessage("Enter ploogin name");
				return;
			}

			var plugin = FindPloogin(args[1]);
			if (plugin == null)
			{
				sender.RAMessage("Ploogin not found");
				return;
			}

			if (!PluginManager._active.Contains(plugin))
			{
				sender.RAMessage("Ploogin already disabled");
				return;
			}
					
			PluginManager.DisablePlugin(plugin);
			sender.RAMessage("Disabling ploogin...");
		}

		private static void OnReloadCommand(string[] args, CommandSender sender)
		{
			if (args.Length < 2)
			{
				sender.RAMessage("Enter ploogin name");
				return;
			}

			var plugin = FindPloogin(args[1]);
			if (plugin == null)
			{
				sender.RAMessage("Ploogin not found");
				return;
			}

			if (!PluginManager._active.Contains(plugin))
			{
				sender.RAMessage("Ploogin disabled");
				return;
			}
					
			PluginManager.DisablePlugin(plugin);
			PluginManager.ReloadPlugin(plugin);
			sender.RAMessage("Reloading ploogin...");
		}

		private static Plugin FindPloogin(string name)
		{
			return PluginManager._plugins.Find(p => p.getName == name);
		}
	}
}