using Loader;
using MEC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EXILED
{
	public class PluginManager
	{
		public static readonly List<Plugin> _plugins = new List<Plugin>();
		public static string AppDataDirectory { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public static string PluginsDirectory { get; private set; } = Path.Combine(AppDataDirectory, "Plugins");
		public static string ExiledDirectory { get; private set; } = Path.Combine(AppDataDirectory, "EXILED");
		public static string DependenciesDirectory { get; private set; } = Path.Combine(ExiledDirectory, "dependencies");
		public static string LoadedDependenciesDirectory { get; private set; } = Path.Combine(PluginsDirectory, "dependencies");
		public static string ManagedAssembliesDirectory { get; private set; } = Path.Combine(Path.Combine(Environment.CurrentDirectory, "SCPSL_Data"), "Managed");
		public static string ConfigsPath { get; private set; } = Path.Combine(ExiledDirectory, $"{ServerStatic.ServerPort}-config.yml");
		public static string LogsPath { get; private set; } = Path.Combine(ExiledDirectory, $"{ServerStatic.ServerPort}-RA_log.txt");
		private static string _typeOverrides = "";

		public static IEnumerator<float> LoadPlugins()
		{
			yield return Timing.WaitForSeconds(0.5f);

			try
			{
				if (Directory.Exists(DependenciesDirectory))
					Directory.Move(DependenciesDirectory, Path.Combine(PluginsDirectory, "dependencies"));

				LoadDependencies();
			}
			catch (Exception exception)
			{
				Log.Error(exception.ToString());
			}

			if (Environment.CurrentDirectory.ToLower().Contains("testing"))
				PluginsDirectory = Path.Combine(AppDataDirectory, "Plugins_Testing");

			if (!Directory.Exists(PluginsDirectory))
			{
				Log.Warn($"Plugin directory not found - creating: {PluginsDirectory}");
				Directory.CreateDirectory(PluginsDirectory);
			}

			List<string> mods = Directory.GetFiles(PluginsDirectory).Where(plugin => !plugin.EndsWith("overrides.txt")).ToList();

			if (File.Exists($"{PluginsDirectory}/overrides.txt"))
				_typeOverrides = File.ReadAllText($"{PluginsDirectory}/overrides.txt");

			bool eventsInstalled = true;

			if (mods.All(mod => !mod.Contains("EXILED_Events.dll")))
			{
				Log.Warn("Events plugin not installed, plugins that do not handle their own events will not function and may cause errors.");
				eventsInstalled = false;
			}

			if (eventsInstalled)
			{
				string eventsPlugin = mods.FirstOrDefault(m => m.Contains("EXILED_Events.dll"));

				LoadPlugin(eventsPlugin);
				mods.Remove(eventsPlugin);
			}

			bool permsInstalled = mods.Any(m => m.Contains("EXILED_Permissions.dll"));

			if (permsInstalled)
			{
				string permsPlugin = mods.FirstOrDefault(m => m.Contains("EXILED_Permissions.dll"));

				LoadPlugin(permsPlugin);
				mods.Remove(permsPlugin);
			}

			foreach (string mod in mods)
			{
				if (mod.EndsWith("EXILED.dll"))
					continue;

				LoadPlugin(mod);
			}

			OnEnable();
		}

		private static List<Assembly> localLoaded = new List<Assembly>();

		private static void LoadDependencies()
		{
			Log.Info("Loading dependencies...");
			Log.Debug($"Searching Directory \"{LoadedDependenciesDirectory}\"");

			if (!Directory.Exists(LoadedDependenciesDirectory))
				Directory.CreateDirectory(LoadedDependenciesDirectory);

			string[] depends = Directory.GetFiles(LoadedDependenciesDirectory);

			foreach (string dll in depends)
			{
				if (!dll.EndsWith(".dll"))
					continue;

				if (IsLoaded(dll))
					return;

				Assembly assembly = Assembly.LoadFrom(dll);
				localLoaded.Add(assembly);
				Log.Info("Loaded dependency " + assembly.FullName);
			}
			Log.Debug("Complete!");
		}

		private static bool IsLoaded(string a)
		{
			foreach (Assembly asm in localLoaded)
			{
				if (asm.Location == a)
					return true;
			}

			return false;
		}


		public static void LoadPlugin(string mod)
		{
			Log.Info($"Loading {mod}");
			try
			{
				byte[] file = ModLoader.ReadFile(mod);
				Assembly assembly = Assembly.Load(file);

				foreach (Type type in assembly.GetTypes())
				{
					if (type.IsAbstract)
					{
						Log.Debug($"{type.FullName} is abstract, skipping.");
						continue;
					}

					if (type.FullName != null && _typeOverrides.Contains(type.FullName))
					{
						Log.Debug($"Overriding type check for {type.FullName}");
					}
					else if (!typeof(Plugin).IsAssignableFrom(type))
					{
						Log.Debug($"{type.FullName} does not inherit from EXILED.Plugin, skipping.");
						continue;
					}

					Log.Info($"Loading type {type.FullName}");
					object plugin = Activator.CreateInstance(type);
					Log.Info($"Instantiated type {type.FullName}");

					if (!(plugin is Plugin p))
					{
						Log.Error($"not plugin error! {type.FullName}");
						continue;
					}

					_plugins.Add(p);
					Log.Info($"Successfully loaded {p.getName}");
				}
			}
			catch (Exception exception)
			{
				Log.Error($"Error while initalizing {mod}! {exception}");
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
				catch (Exception exception)
				{
					Log.Error($"Plugin {plugin.getName} threw an exception while enabling {exception}");
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
				catch (Exception exception)
				{
					Log.Error($"Plugin {plugin.getName} threw an exception while reloading {exception}");
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
				catch (Exception exception)
				{
					Log.Error($"Plugin {plugin.getName} threw an exception while disabling {exception}");
				}
			}
		}

		public static void ReloadPlugins()
		{
			try
			{
				Log.Info($"Reloading Plugins...");
				OnDisable();
				OnReload();
				_plugins.Clear();

				Timing.RunCoroutine(LoadPlugins());
			}
			catch (Exception exception)
			{
				Log.Error($"There was an error while reloading. {exception}");
			}
		}
	}
}