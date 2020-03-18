using MEC;
using System;
using System.IO;

namespace EXILED
{
	public class MainLoader
	{
		//This method is called by the assembly's Loader class when the server starts.
		private static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		private static string exiledPath = Path.Combine(appData, "EXILED");
		private static string path = Path.Combine(exiledPath, $"{ServerStatic.ServerPort}-config.yml");
		public static void EntryPointForLoader()
		{
			Log.Info($"Initalizing Mod Loader");

			if (!File.Exists(path))
				File.Create(path).Close();
			Plugin.Config = new YamlConfig(path);
			Log.debug = Plugin.Config.GetBool("exiled_debug", false);

			CustomNetworkManager.Modded = true;
			Timing.RunCoroutine(PluginManager.LoadPlugins());
		}
	}
}