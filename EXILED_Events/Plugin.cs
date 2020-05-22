using EXILED.Patches;
using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace EXILED
{
	public class EventPlugin : Plugin
	{
		private HarmonyInstance instance;
		public static List<int> GhostedIds = new List<int>();
		internal static DateTime RoundTime;
		public static Random Gen = new Random();
		public static string VersionUpdateUrl = "none";
		public static ExiledVersion Version = new ExiledVersion { Major = 1, Minor = 11, Patch = 0 };

		//The below variables are used to disable the patch for any particular event, allowing devs to implement events themselves.
		#region Patch Disable
		public static bool AntiFlyPatchDisable;
		public static bool CheaterReportPatchDisable;
		public static bool GhostmodePatchDisable;
		public static bool PlayerHurtPatchDisable;
		public static bool RespawnPatchDisable;
		public static bool Scp096PatchDisable;
		public static bool Scp173PatchDisable;
		public static bool SetClassPatchDisable;
		public static bool TriggerTeslaPatchDisable;
		public static bool UseMedicalPatchDisable;
		public static bool WaitingForPlayersPatchDisable;
		public static bool PlayerSpawnEventPatchDisable;
		public static bool SetRandomRolesPatchDisable;
		public static bool WarheadLockPatchDisable;
		public static bool GrenadeThrownPatchDisable;
		public static bool Scp914UpgradeEventPatchDisable;
		public static bool PlayerConsoleCommandPatchDisable;
		public static bool Scp079TriggerTeslaPatchDisable;
		public static bool CheckEscapeEventPatchDisable;
		public static bool CheckRoundEndEventPatchDisable;
		public static bool DecontaminationEventPatchDisable;
		public static bool IntercomSpeakingEventPatchDisable;
		public static bool RoundRestartEventPatchDisable;
		public static bool DoorInteractionEventPatchDisable;
		public static bool PlayerJoinEventPatchDisable;
		public static bool PlayerLeaveEventPatchDisable;
		public static bool StartItemsEventPatchDisable;
		public static bool DropItemEventPatchDisable;
		public static bool PickupItemEventPatchDisable;
		public static bool Generator079EventPatchDisable;
		public static bool HandcuffEventPatchDisable;
		public static bool Scp106ContainEventDisable;
		public static bool SetGroupEventDisable;
		public static bool FemurEnterEventDisable;
		public static bool CmdSyncDataEventDisable;
		public static bool GrenadeExplosionEventDisabled;
		public static bool WarheadKeycardAccessEventDisable;
		public static bool Scp079ExpGainEventDisable;
		public static bool ElevatorInteractionEventDisable;
		public static bool Scp106CreatedPortalEventDisable;
		public static bool LockerInteractEventPatchDisable;
		public static bool Scp106PocketDimensionDamageEventPatchDisable;
		public static bool Scp106TeleportEventPatchDisable;
		public static bool PocketDimensionEscapedEventPatchDisabled;
		public static bool Scp914ActivationEventPatchDisabled;
		public static bool Scp914KnobChangeEventPatchDisable;
		public static bool WarheadStartEventPatchDisable;
		public static bool LateShootEventPatchDisable;
		public static bool GeneratorFinishedEventPatchDisable;
		public static bool CancelMedicalEventPatchDisable;
		public static bool PreAuthEventPatchDisable;
		public static bool RemoteAdminCommandPatchDisable;
		#endregion

		private EventHandlers handlers;

		//The below variable is used to increment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
		private static int patchFixer;

		public static bool Scp173Fix;
		public static bool Scp096Fix;
		public static bool WarheadLocked;
		public static bool NameTracking;
		public static bool DropInventory;
		public static bool RemoveBloodPlacement;
		public static int AntiFlyThreashold;
		public static Dictionary<ReferenceHub, List<int>> TargetGhost = new Dictionary<ReferenceHub, List<int>>();
		public static List<ReferenceHub> DeadPlayers = new List<ReferenceHub>();
		public static bool AimbotBreaker;
		public static bool RespawningESPBreaker;
		public static bool ESPBreaker;

		//The below method gets called when the plugin is enabled by the EXILED loader.
		public override void OnEnable()
		{
			Log.Info(Environment.CurrentDirectory);
			Log.Info("Enabled.");
			Log.Info($"Checking version status...");
			ServerConsole.AddLog($"ServerMod - Version {Version.Major}.{Version.Minor}.{Version.Patch}-EXILED LOGTYPE-8");
			if (Config.GetBool("exiled_auto_update", true))
			{
				if (IsUpdateAvailable())
				{
					Log.Info("There is an new version of EXILED available.");
					AutoUpdate();
				}
			}

			ReloadConfigs();
			Log.Debug("Adding Event Handlers...");
			handlers = new EventHandlers(this);

			Events.WaitingForPlayersEvent += handlers.OnWaitingForPlayers;
			Events.RoundStartEvent += handlers.OnRoundStart;
			Events.RemoteAdminCommandEvent += ReloadCommandHandler.CommandHandler;
			Events.PlayerLeaveEvent += handlers.OnPlayerLeave;
			Events.PlayerDeathEvent += handlers.OnPlayerDeath;
			Events.PlayerJoinEvent += handlers.OnPlayerJoin;
			Events.SetClassEvent += handlers.OnSetClass;

			Log.Debug("Patching...");
			try
			{
				//You must use an incrementer for the harmony instance name, otherwise the new instance will fail to be created if the plugin is reloaded.
				patchFixer++;
				instance = HarmonyInstance.Create($"exiled.patches{patchFixer}");
#if DEBUG
				// If debugging was enabled before, don't touch it
				var disabledStatus = HarmonyInstance.DEBUG == true;
				HarmonyInstance.DEBUG = true;
				HarmonyInstance.DEBUG = disabledStatus;
#endif
				instance.PatchAll();
			}
			catch (Exception exception)
			{
				Log.Error($"Patching failed! {exception}");
			}

			Log.Debug("Patching complete. c:");
			ServerConsole.ReloadServerName();
		}

		public static void ReloadConfigs()
		{
			Scp173Fix = Config.GetBool("exiled_tut_fix173", true);
			Scp096Fix = Config.GetBool("exiled_tut_fix096", true);
			NameTracking = Config.GetBool("exiled_name_tracking", true);
			DropInventory = Config.GetBool("exiled_drop_inventory", true);
			RemoveBloodPlacement = Config.GetBool("exiled_remove_blood_placement");
			AntiFlyThreashold = Config.GetInt("exiled_antifly_threashold", 5);
		}

		private void AutoUpdate()
		{
			try
			{
				Log.Info($"Attempting auto-update...");
				Log.Info($"URL: {VersionUpdateUrl}");
				if (VersionUpdateUrl == "none")
				{
					Log.Error("Version update was queued but not URL was set. This error should never happen.");
					return;
				}

				string tempDirectory = Path.Combine(Directory.GetCurrentDirectory(), "temp");
				Log.Info($"Creating temporary directory: {tempDirectory}...");

				if (!Directory.Exists(tempDirectory))
					Directory.CreateDirectory(tempDirectory);

				string exiledTempPath = Path.Combine(tempDirectory, "EXILED.tar.gz");

				using (WebClient client = new WebClient()) client.DownloadFile(VersionUpdateUrl, exiledTempPath);

				Log.Info("Download successful, extracting contents...");
				ExtractTarGz(exiledTempPath, tempDirectory);
				Log.Info($"Extraction complete, moving files...");

				string tempExiledMainPath = Path.Combine(Path.Combine(tempDirectory, "EXILED"), "EXILED.dll");
				string tempPluginsDirectory = Path.Combine(tempDirectory, "Plugins");
				string tempExiledEventsPath = Path.Combine(tempPluginsDirectory, "EXILED_Events.dll");
				string tempAssemblyPath = Path.Combine(tempDirectory, "Assembly-CSharp.dll");

				File.Delete(Path.Combine(PluginManager.ExiledDirectory, "EXILED.dll"));
				File.Delete(Path.Combine(PluginManager.PluginsDirectory, "EXILED_Events.dll"));
				File.Delete(Path.Combine(PluginManager.PluginsDirectory, "EXILED_Permissions.dll"));
				File.Delete(Path.Combine(PluginManager.PluginsDirectory, "EXILED_Idler.dll"));
				File.Delete(Path.Combine(PluginManager.ManagedAssembliesDirectory, "Assembly-CSharp.dll"));
				File.Move(tempExiledMainPath, Path.Combine(PluginManager.ExiledDirectory, "EXILED.dll"));
				File.Move(tempExiledEventsPath, Path.Combine(PluginManager.PluginsDirectory, "EXILED_Events.dll"));
				File.Move(Path.Combine(tempPluginsDirectory, "EXILED_Permissions.dll"), Path.Combine(PluginManager.PluginsDirectory, "EXILED_Permissions.dll"));
				File.Move(Path.Combine(tempPluginsDirectory, "EXILED_Idler.dll"), Path.Combine(PluginManager.PluginsDirectory, "EXILED_Idler.dll"));
				File.Move(tempAssemblyPath, Path.Combine(PluginManager.ManagedAssembliesDirectory, "Assembly-CSharp.dll"));
				Log.Info($"Files moved, cleaning up...");
				DeleteDirectory(tempDirectory);

				Log.Info("Auto-update complete, restarting server...");
				Application.Quit();
			}
			catch (Exception exception)
			{
				Log.Error($"Auto-update error: {exception}");
			}
		}

		public static void DeleteDirectory(string target_dir)
		{
			string[] files = Directory.GetFiles(target_dir);
			string[] dirs = Directory.GetDirectories(target_dir);

			foreach (string file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}

			foreach (string dir in dirs) DeleteDirectory(dir);

			Directory.Delete(target_dir, false);
		}

		//The below method gets called when the plugin is disabled by the EXILED loader.
		public override void OnDisable()
		{
			Log.Info("Disabled.");
			Log.Debug("Removing Event Handlers...");
			//You should unhook any events you have hooked in the plugin when it is disabled, otherwise GAC will cause your server to have a meltdown.
			Events.WaitingForPlayersEvent -= handlers.OnWaitingForPlayers;
			Events.RoundStartEvent -= handlers.OnRoundStart;
			Events.RemoteAdminCommandEvent -= ReloadCommandHandler.CommandHandler;
			Events.PlayerLeaveEvent -= handlers.OnPlayerLeave;
			Events.PlayerDeathEvent -= handlers.OnPlayerDeath;

			handlers = null;
			Log.Debug("Unpatching...");
			instance.UnpatchAll();
			Log.Debug("Unpatching complete. Goodbye. :c");
		}

		//The below is called when the EXILED loader reloads all plugins. The reloading process calls OnDisable, then OnReload, unloads the plugin and reloads the new version, then OnEnable.
		public override void OnReload() => Log.Info("Reloading.");

		public override string getName { get; }

		public static double GetRoundDuration() => Math.Abs((RoundTime - DateTime.Now).TotalSeconds);

		public bool IsUpdateAvailable()
		{
			try
			{
				string url = "https://github.com/galaxy119/EXILED/releases/" + (Config.GetBool("exiled_testing") ? "" : "latest/");
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{url}");
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Stream stream = response.GetResponseStream();

				if (stream == null)
					throw new InvalidOperationException("No response from Github. This shouldn't happen, yell at Joker.");

				StreamReader reader = new StreamReader(stream);
				string read = reader.ReadToEnd();
				string[] readArray = read.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
				string line = readArray.FirstOrDefault(s => s.Contains("EXILED.tar.gz"));
				string version = Between(line, "/galaxy119/EXILED/releases/download/", "/EXILED.tar.gz");
				string[] versionArray = version.Split('.');

				if (!int.TryParse(versionArray[0], out int major))
				{
					Log.Error($"Unable to parse EXILED major version.");
					return false;
				}

				if (!int.TryParse(versionArray[1], out int minor))
				{
					Log.Error($"Unable to parse EXILED minor version.");
					return false;
				}

				if (!int.TryParse(versionArray[2], out int patch))
				{
					Log.Error($"Unable to parse EXILED patch version.");
					return false;
				}

				VersionUpdateUrl = $"{url.Replace("latest/", "")}download/{version}/EXILED.tar.gz";
				if (major > Version.Major)
				{
					Log.Info($"Major version outdated: Current {Version.Major}. New: {major}");
					return true;
				}

				if (minor > Version.Minor && major == Version.Major)
				{
					Log.Info($"Minor version outdated: Current {Version.Minor}. New: {minor}");
					return true;
				}

				if (patch > Version.Patch && major == Version.Major && minor == Version.Minor)
				{
					Log.Info($"Patch version outdated: Current {Version.Patch}. New: {patch}");
					return true;
				}

			}
			catch (Exception exception)
			{
				Log.Error($"Updating error: {exception}");
			}

			return false;
		}

		private static string Between(string str, string firstString, string lastString)
		{
			int pos1 = str.IndexOf(firstString, StringComparison.Ordinal) + firstString.Length;
			int pos2 = str.IndexOf(lastString, StringComparison.Ordinal);
			string finalString = str.Substring(pos1, pos2 - pos1);
			return finalString;
		}

		private static void ExtractTarGz(string filename, string outputDir)
		{
			using (FileStream stream = File.OpenRead(filename))
			{
				ExtractTarGz(stream, outputDir);
			}
		}

		private static void ExtractTarGz(Stream stream, string outputDir)
		{
			using (GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress))
			{
				const int chunk = 4096;
				using (MemoryStream memStr = new MemoryStream())
				{
					int read;
					byte[] buffer = new byte[chunk];
					do
					{
						read = gzip.Read(buffer, 0, chunk);
						memStr.Write(buffer, 0, read);
					}
					while (read == chunk);

					memStr.Seek(0, SeekOrigin.Begin);
					ExtractTar(memStr, outputDir);
				}
			}
		}

		private static void ExtractTar(Stream stream, string outputDir)
		{
			byte[] buffer = new byte[100];
			while (true)
				try
				{
					stream.Read(buffer, 0, 100);
					string name = Encoding.ASCII.GetString(buffer).Trim('\0');
					if (string.IsNullOrWhiteSpace(name)) break;
					stream.Seek(24, SeekOrigin.Current);
					stream.Read(buffer, 0, 12);
					long size = Convert.ToInt64(Encoding.UTF8.GetString(buffer, 0, 12).Trim('\0').Trim(), 8);

					stream.Seek(376L, SeekOrigin.Current);

					string output = Path.Combine(outputDir, name);
					if (!Directory.Exists(Path.GetDirectoryName(output)))
						Directory.CreateDirectory(Path.GetDirectoryName(output));

					if (!name.Equals("./", StringComparison.InvariantCulture))
					{
						using (FileStream str = File.Open(output, FileMode.OpenOrCreate, FileAccess.Write))
						{
							byte[] buf = new byte[size];
							stream.Read(buf, 0, buf.Length);
							str.Write(buf, 0, buf.Length);
						}
					}

					long pos = stream.Position;

					long offset = 512 - pos % 512;
					if (offset == 512) offset = 0;

					stream.Seek(offset, SeekOrigin.Current);
				}
				catch (Exception)
				{
					// ignored
				}
		}

		//Used to Exiled2Multiadmin
		internal static void ToMultiAdmin(string message)
		{
			ServerConsole.AddLog($"[EXILED2Multiadmin] {message} LOGTYPE02");
		}
	}
}