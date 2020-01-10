using System;
using System.IO;
using GameCore;
using Mirror.LiteNetLib4Mirror;

namespace EXILED
{
	public class ConfigHandler
	{
		public static YamlConfig PluginConfig;

		public static void LoadConfig()
		{
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string exiledPath = Path.Combine(appData, "EXILED");
			string path = Path.Combine(exiledPath, $"{ServerStatic.ServerPort}-config.yml");
			Plugin.Debug($"Config path: {path}");
			
			if (!File.Exists(path))
				File.Create(path).Close();
			PluginConfig = new YamlConfig(path);
		}
	}
}