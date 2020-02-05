using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using EXILED.Patches;
using Harmony;
using UnityEngine;
using Random = System.Random;

namespace EXILED
{
	public class EventPlugin : Plugin
	{
		private HarmonyInstance instance;
		public static List<int> GhostedIds = new List<int>();
		internal static DateTime RoundTime;
		internal static Random Gen = new Random();
		public static bool WarheadLocked;
		public static string VersionUpdateUrl = "none";
		public static ExiledVersion Version = new ExiledVersion
		{
			Major = 1,
			Minor = 7,
			Patch = 0
		};
		
		//The below variables are used to disable the patch for any particular event, allowing devs to implement events themselves.
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
		public static bool WarheadLockPatchDisable;
		public static bool GrenadeThrownPatchDisable;
		public static bool NineFourteenMachinePatchDisable;
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
		public static bool DropItemEventPatchDisable;
		public static bool PickupItemEventPatchDisable;
		public static bool Generator079EventPatchDisable;
		public static bool HandcuffEventPatchDisable;
		public static bool Scp106ContainEventDisable;
		public static bool SetGroupEventDisable;

		private EventHandlers handlers;
		//The below variable is used to incriment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
		private static int patchFixer;

		//The below method gets called when the plugin is enabled by the EXILED loader.
		public override void OnEnable()
		{
			Info("Enabled.");
			Info("Checking version status..");
			 if (IsUpdateAvailible())
			 {
			 	Info("There is an new version of EXILED available.");
			 	if (Config.GetBool("exiled_auto_update", true))
			 	{
			 		AutoUpdate();
			    }
			 }

			Debug("Adding Event Handlers..");
			handlers = new EventHandlers(this);
			Events.WaitingForPlayersEvent += handlers.OnWaitingForPlayers;
			Events.RoundStartEvent += handlers.OnRoundStart;
			Events.RemoteAdminCommandEvent += ReloadCommandHandler.CommandHandler;
			Events.SetClassEvent += handlers.OnSetClass;
			Events.PlayerLeaveEvent += handlers.OnPlayerLeave;
			
			Debug("Patching..");
			try
			{
				//You must use an incrementer for the harmony instance name, otherwise the new instance will fail to be created if the plugin is reloaded.
				patchFixer++;
				instance = HarmonyInstance.Create($"exiled.patches{patchFixer}");
				instance.PatchAll();
			}
			catch (Exception e)
			{
				Error($"Patching failed! {e}");
			}

			Debug("Patching complete. c:");
			ServerConsole.ReloadServerName();
		}

		private void AutoUpdate()
		{
			Info($"Attempting auto-update..");
			Debug($"URL: {VersionUpdateUrl}");
			if (VersionUpdateUrl == "none")
			{
				Error("Version update was queued but not URL was set. This error should never happen.");
				return;
			}
			
			string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "temp");
			Debug($"Creating temporary directory: {tempPath}..");
			
			if (!Directory.Exists(tempPath))
				Directory.CreateDirectory(tempPath);
			string exiledTemp = Path.Combine(tempPath, "EXILED.tar.gz");
			using (WebClient client = new WebClient())
				client.DownloadFile(VersionUpdateUrl, exiledTemp);
			
			Debug("Download successful, extracting contents..");
			ExtractTarGz(exiledTemp, tempPath);
			Debug($"Extraction complete, moving files..");
			string tempExiledMain = Path.Combine(Path.Combine(tempPath, "EXILED"), "EXILED.dll");
			string tempExiledEvents = Path.Combine(Path.Combine(tempPath, "Plugins"), "EXILED_Events.dll");
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			File.Delete(Path.Combine(Path.Combine(appData, "EXILED"), "EXILED.dll"));
			File.Delete(Path.Combine(Path.Combine(appData, "Plugins"), "EXILED_Events.dll"));
			File.Move(tempExiledMain, Path.Combine(Path.Combine(appData, "EXILED"), "EXILED.dll"));
			File.Move(tempExiledEvents, Path.Combine(Path.Combine(appData, "Plugins"), "EXILED_Events.dll"));
			Debug($"Files moved, cleaning up..");
			DeleteDirectory(tempPath);
			
			Info("Auto-update complete, restarting server..");
			Application.Quit();
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

			foreach (string dir in dirs)
			{
				DeleteDirectory(dir);
			}

			Directory.Delete(target_dir, false);
		}

		//The below method gets called when the plugin is disabled by the EXILED loader.
		public override void OnDisable()
		{
			Info("Disabled.");
			//You should unhook any events you have hooked in the plugin when it is disabled, otherwise GAC will cause your server to have a meltdown.
			Debug("Removing Event Handlers..");
			Events.WaitingForPlayersEvent -= handlers.OnWaitingForPlayers;
			Events.RoundStartEvent -= handlers.OnRoundStart;
			handlers = null;
			Debug("Unpatching..");
			instance.UnpatchAll();
			Debug("Unpatching complete. Goodbye. :c");
		}

		//The below is called when the EXILED loader reloads all plugins. The reloading process calls OnDisable, then OnReload, unloads the plugin and reloads the new version, then OnEnable.
		public override void OnReload() {}

		public override string getName { get; }

		public static double GetRoundDuration() => Math.Abs((RoundTime - DateTime.Now).TotalSeconds);

		public bool IsUpdateAvailible()
		{
			string url = "https://github.com/galaxy119/EXILED/releases/";
			HttpWebRequest request = (HttpWebRequest) WebRequest.Create($"{url}latest/");
			HttpWebResponse response = (HttpWebResponse) request.GetResponse();
			Stream stream = response.GetResponseStream();
			StreamReader reader = new StreamReader(stream);
			string read = reader.ReadToEnd();
			string[] readArray = read.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			string line = readArray.FirstOrDefault(s => s.Contains("EXILED.tar.gz"));
			string version = Between(line, "/galaxy119/EXILED/releases/download/", "/EXILED.tar.gz");
			string[] versionArray = version.Split('.');

			if (!int.TryParse(versionArray[0], out int major))
			{
				Error($"Unable to parse EXILED major version.");
				return false;
			}

			if (!int.TryParse(versionArray[1], out int minor))
			{
				Error($"Unable to parse EXILED minor version.");
				return false;
			}

			if (!int.TryParse(versionArray[2], out int patch))
			{
				Error($"Unable to parse EXILED patch version.");
				return false;
			}


			VersionUpdateUrl = $"{url}download/{version}/EXILED.tar.gz";
			return major != Version.Major || minor != Version.Minor || patch != Version.Patch;
		}
		
		private static string Between(string str , string firstString, string lastString)
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
					} while (read == chunk);

					memStr.Seek(0, SeekOrigin.Begin);
					ExtractTar(memStr, outputDir);
				}
			}
		}

		private static void ExtractTar(Stream stream, string outputDir)
		{
			byte[] buffer = new byte[100];
			while (true)
			{
				try
				{
					stream.Read(buffer, 0, 100);
					string name = Encoding.ASCII.GetString(buffer).Trim('\0');
					if (string.IsNullOrWhiteSpace(name))
						break;
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

					long offset = 512 - (pos % 512);
					if (offset == 512)
						offset = 0;

					stream.Seek(offset, SeekOrigin.Current);
				}
				catch (Exception)
				{
					// ignored
				}
			}
		}
	}
}