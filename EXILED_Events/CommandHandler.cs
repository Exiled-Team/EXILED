using EXILED.Extensions;
using MEC;
using UnityEngine;

namespace EXILED
{
	public class CommandHandler
	{
		//Do not use this as plugin command handler, this is only meant to handle the EXILED reload command, handle commands similarly from within your plugin.
		private readonly Plugin plugin;
		public CommandHandler(Plugin plugin) => this.plugin = plugin;
		public void OnRaCommand(RaCommandEvent ev)
		{
			string[] args = ev.Command.Split(' ');
			switch (args[0].ToLower())
			{
				case "reconnectrs":
					ev.Allow = false;
					PlayerManager.localPlayer.GetComponent<PlayerStats>()?.Roundrestart();
					Timing.CallDelayed(1.5f, Application.Quit);
					break;
				case "reload":
					ev.Allow = false;
					if (args.Length < 2)
					{
						ev.Sender.RAMessage("You must specify a config type to reload: exiled, plugin, gameplay, ra or all");
						return;
					}

					switch (args[1].ToLower())
					{
						case "plugins":
							PluginManager.ReloadPlugins();
							ev.Sender.RAMessage("Reloading ploogins..");
							break;
						case "pluginconfig":
							ReloadPluginConfig();
							ev.Sender.RAMessage("All plugin configs reloaded.");
							break;
						case "exiled":
							ReloadPluginConfig(true);
							ev.Sender.RAMessage("EXILED Configs reloaded.");
							break;
						case "gameplay":
							ReloadGameplayConfig();
							ev.Sender.RAMessage("Gameplay configs reloaded.");
							break;
						case "remoteadmin":
						case "ra":
							ReloadRaConfig();
							ev.Sender.RAMessage("Remote admin configs reloaded.");
							break;
					}

					break;
			}
		}

		public static void ReloadPluginConfig(bool onlyExiled = false)
		{
			Plugin.Config = new YamlConfig(Plugin.Config.Path);
			foreach (Plugin plugin in PluginManager._plugins)
			{
				if (onlyExiled && plugin.GetName != "EXILED")
					continue;
				plugin.ReloadConfig();
			}
		}

		public static void ReloadGameplayConfig()
		{
			GameCore.ConfigFile.ReloadGameConfigs();
		}

		public static void ReloadRaConfig()
		{
			ServerStatic.RolesConfig = new YamlConfig(ServerStatic.RolesConfigPath);
			ServerStatic.SharedGroupsConfig = ((GameCore.ConfigSharing.Paths[4] == null) ? null : new YamlConfig(GameCore.ConfigSharing.Paths[4] + "shared_groups.txt"));
			ServerStatic.SharedGroupsMembersConfig = ((GameCore.ConfigSharing.Paths[5] == null) ? null : new YamlConfig(GameCore.ConfigSharing.Paths[5] + "shared_groups_members.txt"));
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			ServerStatic.GetPermissionsHandler().RefreshPermissions();
		}
		
	}
}