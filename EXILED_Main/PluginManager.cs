using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Loader;
using MEC;

namespace EXILED
{
	public class PluginManager
	{
		private static readonly List<Plugin> _plugins = new List<Plugin>();
		private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		private static string _typeOverrides = "";
		
		public static IEnumerator<float> LoadPlugins()
		{
			yield return Timing.WaitForSeconds(0.5f);
			string path = Path.Combine(AppData, "Plugins");

			if (Environment.CurrentDirectory.ToLower().Contains("testing"))
				path = Path.Combine(AppData, "Plugins_Testing");

			if (!Directory.Exists(path))
			{
				Plugin.Info($"Plugin directory not found - creating: {path}");
				Directory.CreateDirectory(path);
			}

			List<string> mods = Directory.GetFiles(path).Where(p => !p.EndsWith("overrides.txt")).ToList();
			if (File.Exists($"{path}/overrides.txt"))
				_typeOverrides = File.ReadAllText($"{path}/overrides.txt");

			bool eventsInstalled = true;
			if (mods.All(m => !m.Contains("EXILED_Events.dll")))
			{
				ServerConsole.AddLog(
					"WARN: Events plugin not installed, plugins that do not handle their own events will not function and may cause errors.");
				eventsInstalled = false;
			}

			if (eventsInstalled)
			{
				string eventsPlugin = mods.FirstOrDefault(m => m.Contains("EXILED_Events.dll"));
				LoadPlugin(eventsPlugin);
				mods.Remove(eventsPlugin);
			}

			foreach (string mod in mods)
			{
				if (mod.Contains("EXILED.dll"))
					continue;
				LoadPlugin(mod);
			}
			
			OnEnable();
		}

		public static void LoadPlugin(string mod)
		{
			ServerConsole.AddLog($"Loading {mod}");
			try
			{
				byte[] file = ModLoader.ReadFile(mod);
				Assembly assembly = Assembly.Load(file);
				
				foreach (Type type in assembly.GetTypes())
				{
					if (type.IsAbstract)
					{
						continue;
					}

					if (type.FullName != null && _typeOverrides.Contains(type.FullName))
					{
						ServerConsole.AddLog($"Overriding type check for {type.FullName}");
					}
					else if (!typeof(Plugin).IsAssignableFrom(type))
					{
						continue;
					}
					ServerConsole.AddLog($"Loading type {type.FullName}");
					object plugin = Activator.CreateInstance(type);
					ServerConsole.AddLog($"Instantiated type {type.FullName}");
					if (!(plugin is Plugin p))
					{
						ServerConsole.AddLog($"not plugin error! {type.FullName}");
						continue;
					}

					_plugins.Add(p);
					ServerConsole.AddLog($"Successfully loaded {p.getName}");
				}
			}
			catch (Exception e)
			{
				ServerConsole.AddLog($"Error while initalizing {mod}! {e}");
			}
		}

		public static void OnEnable()
		{
			foreach (Plugin plugin in _plugins)
			{
				try
				{
					plugin.OnEnable();
				}
				catch (Exception e)
				{
					ServerConsole.AddLog($"Plugin {plugin.getName} threw an exception while enabling {e}");
				}
			}
		}

		public static void OnReload()
		{
			foreach (Plugin plugin in _plugins)
			{
				try
				{
					plugin.OnReload();
				}
				catch (Exception e)
				{
					ServerConsole.AddLog($"Plugin {plugin.getName} threw an exception while reloading {e}");
				}
			}
		}

		public static void OnDisable()
		{
			foreach (Plugin plugin in _plugins)
			{
				try
				{
					plugin.OnDisable();
				}
				catch (Exception e)
				{
					ServerConsole.AddLog($"Plugin {plugin.getName} threw an exception while disabling {e}");
				}
			}
		}

		public static void ReloadPlugins()
		{
			try
			{
				OnDisable();
				OnReload();

				_plugins.Clear();

				Timing.RunCoroutine(LoadPlugins());
				OnEnable();
			}
			catch (Exception e)
			{
				ServerConsole.AddLog($"There was an error while reloading. {e}");
			}
		}
	}
}