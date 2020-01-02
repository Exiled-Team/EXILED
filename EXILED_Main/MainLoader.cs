using Harmony;

namespace EXILED
{
	public class MainLoader
	{
		//This method is called by the assembly's Loader class when the server starts.
		public static void EntryPointForLoader()
		{
			ServerConsole.AddLog($"Initalizing Mod Loader");

			PluginManager.LoadPlugins();
			PluginManager.OnEnable();
		}
	}
}