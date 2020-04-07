using MEC;
using System.IO;

namespace EXILED
{
	public class MainLoader
	{
		//This method is called by the assembly's Loader class when the server starts.
		public static void EntryPointForLoader()
		{
			Log.Info($"Initalizing Mod Loader");

			if (!File.Exists(PluginManager.ConfigsPath))
				File.Create(PluginManager.ConfigsPath).Close();

			Plugin.Config = new YamlConfig(PluginManager.ConfigsPath);
			Log.debug = Plugin.Config.GetBool("exiled_debug", false);

			CustomNetworkManager.Modded = true;
			Timing.RunCoroutine(PluginManager.LoadPlugins());
		}
	}
}