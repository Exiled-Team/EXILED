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
			string exiled = Path.Combine(AppData, "EXILED");
			string deps = Path.Combine(exiled, "dependencies");
			try
			{
				if (Directory.Exists(deps))
					Directory.Move(deps, Path.Combine(path, "dependencies"));
				LoadDeps();
			}
			catch (Exception e)
			{
				Plugin.Error(e.ToString());
			}

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
				Plugin.Error(
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

		private static List<Assembly> localLoaded = new List<Assembly>();

		private static void LoadDeps()
		{
			Plugin.Info("Loading dependencies...");
			string pl = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Plugins");
			string folder = Path.Combine(pl, "dependencies");
			Plugin.Info("Searching Directory '" + folder + "'");
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);
			string[] depends = Directory.GetFiles(folder);
			foreach (string dll in depends)
			{
				if (!dll.EndsWith(".dll"))
					continue;
				if (IsLoaded(dll))
					return;
				Assembly a = Assembly.LoadFrom(dll);
				localLoaded.Add(a);
				Plugin.Info("Loaded dependency " + a.FullName);
			}
			Plugin.Info("Complete!");
		}
		
		private static bool IsLoaded(string a)
		{
			foreach(Assembly asm in localLoaded)
			{
				if (asm.Location == a)
					return true;
			}
			return false;
		}


		public static void LoadPlugin(string mod)
		{
			Plugin.Info($"Loading {mod}");
			try
			{
				byte[] file = ModLoader.ReadFile(mod);
				Assembly assembly = Assembly.Load(file);
				
				foreach (Type type in assembly.GetTypes())
				{
					if (type.IsAbstract)
					{
						Plugin.Debug($"{type.FullName} is abstract, skipping.");
						continue;
					}

					if (type.FullName != null && _typeOverrides.Contains(type.FullName))
					{
						Plugin.Debug($"Overriding type check for {type.FullName}");
					}
					else if (!typeof(Plugin).IsAssignableFrom(type))
					{
						Plugin.Debug($"{type.FullName} does not inherit from EXILED.Plugin, skipping.");
						continue;
					}
					Plugin.Info($"Loading type {type.FullName}");
					object plugin = Activator.CreateInstance(type);
					Plugin.Info($"Instantiated type {type.FullName}");
					if (!(plugin is Plugin p))
					{
						Plugin.Info($"not plugin error! {type.FullName}");
						continue;
					}

					_plugins.Add(p);
					Plugin.Info($"Successfully loaded {p.getName}");
				}
			}
			catch (Exception e)
			{
				Plugin.Error($"Error while initalizing {mod}! {e}");
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
					Plugin.Error($"Plugin {plugin.getName} threw an exception while enabling {e}");
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
					Plugin.Error($"Plugin {plugin.getName} threw an exception while reloading {e}");
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
					Plugin.Error($"Plugin {plugin.getName} threw an exception while disabling {e}");
				}
			}
		}

		public static void ReloadPlugins()
		{
			try
			{
				Plugin.Info($"Reloading Plugins..");
				OnDisable();
				OnReload();
				_plugins.Clear();

				Timing.RunCoroutine(LoadPlugins());
			}
			catch (Exception e)
			{
				Plugin.Error($"There was an error while reloading. {e}");
			}
		}
	}
}