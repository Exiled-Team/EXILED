using System;
using System.IO;
using MEC;

namespace EXILED
{
	public class MainLoader
	{
		//This method is called by the assembly's Loader class when the server starts.
		private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		private static readonly string ExiledPath = System.IO.Path.Combine(AppData, "EXILED");
		private static readonly string Path = System.IO.Path.Combine(ExiledPath, $"{ServerStatic.ServerPort}-config.yml");
		public static void EntryPointForLoader()
		{
			Log.Info($"Initializing Mod Loader");

			if (!File.Exists(Path))
				File.Create(Path).Close();
			Plugin.Config = new YamlConfig(Path);
			Log.debug = Plugin.Config.GetBool("exiled_debug", false);

			CustomNetworkManager.Modded = true;
			Timing.RunCoroutine(PluginManager.LoadPlugins());
		}
	}
}