
using System.Linq;
using System.Reflection;
using Harmony;

namespace EXILED
{
	public class MainLoader
	{

		internal static HarmonyInstance instance;
		
		public static void EntryPointForLoader()
		{
			instance = HarmonyInstance.Create("EventHooks");
			instance.PatchAll();
			
			ServerConsole.AddLog($"Initalizing Mod Loader");

			PluginManager.LoadPlugins();
			PluginManager.OnEnable();
		}
		

	}
}