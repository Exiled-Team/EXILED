using System;
using System.Collections.Generic;
using System.Linq;
using EXILED.Patches;
using Harmony;
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
		
		private EventHandlers handlers;
		//The below variable is used to incriment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
		private static int patchFixer;

		//The below method gets called when the plugin is enabled by the EXILED loader.
		public override void OnEnable()
		{
			Info("Enabled.");
			Debug("Adding Event Handlers..");
			handlers = new EventHandlers(this);
			Events.WaitingForPlayersEvent += handlers.OnWaitingForPlayers;
			Events.RoundStartEvent += handlers.OnRoundStart;
			Events.RemoteAdminCommandEvent += ReloadCommandHandler.CommandHandler;
			Events.SetClassEvent += handlers.OnSetClass;
			
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
	}
}