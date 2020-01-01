using System;
using System.Collections.Generic;
using EXILED.Patches;
using Harmony;
using JokersPlugin;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace EXILED
{
	public class plugin : EXILED.Plugin
	{
		private HarmonyInstance instance;
		public static List<int> GhostedIds = new List<int>();
		internal static DateTime RoundTime;
		internal static Random Gen = new Random();
		public static bool WarheadLocked;
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
		private EventHandlers handlers;
		private int patchFixer = 0;
		
		public override void OnEnable()
		{
			Info("Enabled.");
			Debug("Adding Event Handlers..");
			handlers = new EventHandlers(this);
			Events.WaitingForPlayersEvent += handlers.OnWaitingForPlayers;
			Events.RoundStartEvent += handlers.OnRoundStart;
			
			Debug("Patching..");
			try
			{
				patchFixer++;
				instance = HarmonyInstance.Create($"exiled.patches{patchFixer}");
				instance.PatchAll();
			}
			catch (Exception e)
			{
				Error($"Patching failed! {e}");
			}

			Debug("Patching complete. c:");
		}

		public override void OnDisable()
		{
			Info("Disabled.");
			Debug("Removing Event Handlers..");
			Events.WaitingForPlayersEvent -= handlers.OnWaitingForPlayers;
			Events.RoundStartEvent -= handlers.OnRoundStart;
			handlers = null;
			Debug("Unpatching..");
			instance.UnpatchAll();
			Debug("Unpatching complete. Goodbye. :c");
		}

		public override void OnReload()
		{
			
		}

		public override string getName { get; }

		public static double GetRoundDuration() => Math.Abs((RoundTime - DateTime.Now).TotalSeconds);
	}
}