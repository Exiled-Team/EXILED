using System.Collections.Generic;
using Grenades;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace EXILED
{
	public class Events
	{
		public static event WaitingForPlayers WaitingForPlayersEvent;
		public delegate void WaitingForPlayers();
		public static void InvokeWaitingForPlayers()
		{
			WaitingForPlayers waitingForPlayers = WaitingForPlayersEvent;
			waitingForPlayers?.Invoke();
		}
		
		public static event GrenadeThrown GrenadeThrownEvent;
		public delegate void GrenadeThrown(ref GrenadeThrownEvent ev);
		public static void InvokeGrenadeThrown(ref GrenadeManager gm, ref int id, ref bool slow, ref double fuse, ref bool allow)
		{
			GrenadeThrown grenadeThrown = GrenadeThrownEvent;
			if (grenadeThrown == null)
				return;
			GrenadeThrownEvent ev = new GrenadeThrownEvent()
			{   
				Player = Plugin.GetPlayer(gm.gameObject),
				Gm = gm
			};
			grenadeThrown?.Invoke(ref ev);
		}

        public static event SCP914Upgrade SCP914UpgradeEvent;
        public delegate void SCP914Upgrade(SCP914UpgradeEvent ev);
        public static void InvokeSCP914Upgrade(Scp914.Scp914Machine machine, List<CharacterClassManager> ccms, List<Pickup> pickups, Scp914.Scp914Knob knobSetting)
        {
	        SCP914Upgrade activated = SCP914UpgradeEvent;
	        if (activated == null)
		        return;
            List<ReferenceHub> players = new List<ReferenceHub>();
            foreach (CharacterClassManager ccm in ccms)
            {
                players.Add(Plugin.GetPlayer(ccm.gameObject));
            }

            List<ItemType> items = new List<ItemType>();
            foreach (Pickup pick in pickups)
            {
                items.Add(pick.info.itemId);
            }
            
            SCP914UpgradeEvent ev = new SCP914UpgradeEvent()
            {
                Machine = machine,
                Players = players,
                Items = items,
                KnobSetting = knobSetting
            };
            activated?.Invoke(ev);
        }

		public static event SetClass SetClassEvent;
		public delegate void SetClass(SetClassEvent ev);
		public static void InvokeSetClass(CharacterClassManager ccm, RoleType id)
		{
			SetClass setClass = SetClassEvent;
			if (setClass == null)
				return;
			
			SetClassEvent ev = new SetClassEvent()
			{
				Player = Plugin.GetPlayer(ccm.gameObject),
				Role = id
			};
			setClass?.Invoke(ev);
		}
		
		// public static event OnPlayerSpawn PlayerSpawnEvent;
		// public delegate void OnPlayerSpawn(ReferenceHub rh, RoleType id);
		// public static void InvokePlayerSpawn(CharacterClassManager ccm, RoleType id)
		// {
		// 	ReferenceHub hub = Plugin.GetPlayer(ccm.gameObject);
		// 	OnPlayerSpawn onPlayerSpawn = PlayerSpawnEvent;
		// 	onPlayerSpawn?.Invoke(hub, id);
		// }

		public static event PlayerHurt PlayerHurtEvent;
		public delegate void PlayerHurt(ref PlayerHurtEvent ev);
		public static void InvokePlayerHurt(PlayerStats stats, ref PlayerStats.HitInfo info, GameObject obj)
		{
			PlayerHurt playerHurt = PlayerHurtEvent;
			if (playerHurt == null)
				return;
			
			PlayerHurtEvent ev = new PlayerHurtEvent()
			{
				Attacker = Plugin.GetPlayer(stats.gameObject),
				Player = Plugin.GetPlayer(obj),
				Info = info
			};
			playerHurt?.Invoke(ref ev);
			info = ev.Info;
		}
		
		public static event TriggerTesla TriggerTeslaEvent;
		public delegate void TriggerTesla(ref TriggerTeslaEvent ev);
		public static void InvokeTriggerTesla(GameObject obj, bool hurtRange, ref bool triggerable)
		{
			TriggerTesla triggerTesla = TriggerTeslaEvent;
			if (triggerTesla == null)
				return;
			
			TriggerTeslaEvent ev = new TriggerTeslaEvent()
			{
				Player = Plugin.GetPlayer(obj),
				Triggerable = triggerable
			};
			triggerTesla?.Invoke(ref ev);
			triggerable = ev.Triggerable;
		}

		public static event PlayerDeath PlayerDeathEvent;
		public delegate void PlayerDeath(ref PlayerDeathEvent ev); 
		public static void InvokePlayerDeath(PlayerStats stats, ref PlayerStats.HitInfo info, GameObject obj)
		{
			PlayerDeath playerDeath = PlayerDeathEvent;
			if (playerDeath == null)
				return;
			
			PlayerDeathEvent ev = new PlayerDeathEvent()
			{
				Killer = Plugin.GetPlayer(stats.gameObject),
				Player = Plugin.GetPlayer(obj),
				Info = info
			};
			playerDeath?.Invoke(ref ev);
			info = ev.Info;
		}

		public static event TeamRespawn TeamRespawnEvent;
		public delegate void TeamRespawn(ref TeamRespawnEvent ev);

		public static void InvokeTeamRespawn(ref bool isChaos, ref int maxRespawn, ref List<GameObject> toRespawn)
		{
			TeamRespawn teamRespawn = TeamRespawnEvent;
			if (teamRespawn == null)
				return;
			
			List<ReferenceHub> respawn = new List<ReferenceHub>();
			foreach (GameObject obj in toRespawn)
				respawn.Add(Plugin.GetPlayer(obj));
			TeamRespawnEvent ev = new TeamRespawnEvent()
			{
				IsChaos = isChaos,
				MaxRespawnAmt = maxRespawn,
				ToRespawn = respawn
			};
			teamRespawn?.Invoke(ref ev);
			maxRespawn = ev.MaxRespawnAmt;
			toRespawn = new List<GameObject>();
			foreach (ReferenceHub hub in ev.ToRespawn)
				toRespawn.Add(hub.gameObject);
		}

		public static event UseMedicalItem UseMedicalItemEvent;
		public delegate void UseMedicalItem(MedicalItemEvent ev);

		public static void InvokeUseMedicalItem(GameObject obj, ItemType type)
		{
			UseMedicalItem useMedicalItem = UseMedicalItemEvent;
			if (useMedicalItem == null)
				return;
			
			MedicalItemEvent ev = new MedicalItemEvent()
			{
				Player = Plugin.GetPlayer(obj),
				Item = type
			};
			useMedicalItem?.Invoke(ev);
		}

		public static event Scp096Enrage Scp096EnrageEvent;
		public delegate void Scp096Enrage(ref Scp096EnrageEvent ev);

		public static void InvokeScp096Enrage(Scp096PlayerScript script, ref bool allow)
		{
			Scp096Enrage scp096Enrage = Scp096EnrageEvent;
			if (scp096Enrage == null)
				return;
			
			Scp096EnrageEvent ev = new Scp096EnrageEvent()
			{
				Player = Plugin.GetPlayer(script.gameObject),
				Script = script,
				Allow = allow
			};
			scp096Enrage?.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event Scp096Calm Scp096CalmEvent;
		public delegate void Scp096Calm(ref Scp096CalmEvent ev);

		public static void InvokeScp096Calm(Scp096PlayerScript script, ref bool allow)
		{
			Scp096Calm scp096Calm = Scp096CalmEvent;
			if (scp096Calm == null)
				return;
			
			Scp096CalmEvent ev = new Scp096CalmEvent()
			{
				Player = Plugin.GetPlayer(script.gameObject),
				Script = script,
				Allow = allow
			};
			scp096Calm?.Invoke(ref ev);
			allow = ev.Allow;
		}
		
		public static event OnRoundStart RoundStartEvent;
		public delegate void OnRoundStart();
		public static void InvokeRoundStart()
		{
			OnRoundStart roundStartEvent = RoundStartEvent;
			roundStartEvent?.Invoke();
		}

		public static event OnPreAuth PreAuthEvent;
		public delegate void OnPreAuth(ref PreauthEvent ev);
		public static void InvokePreAuth(ref string userid, ConnectionRequest request, ref bool allow)
		{
			OnPreAuth preAuthEvent = PreAuthEvent;
			if (preAuthEvent == null)
				return;
			
			PreauthEvent ev = new PreauthEvent()
			{
				Allow = allow,
				Request = request,
				UserId = userid
			};
			preAuthEvent?.Invoke(ref ev);
			allow = ev.Allow;
			userid = ev.UserId;
		}

		public static event OnRoundEnd RoundEndEvent;
		public delegate void OnRoundEnd();
		public static void InvokeRoundEnd()
		{
			OnRoundEnd roundEndEvent = RoundEndEvent;
			roundEndEvent?.Invoke();
		}
		
		
		public static event OnCommand RemoteAdminCommandEvent;
		public delegate void OnCommand(ref RACommandEvent ev);
		public static void InvokeCommand(ref string query, ref CommandSender sender, ref bool allow)
		{
			OnCommand adminCommandEvent = RemoteAdminCommandEvent;
			if (adminCommandEvent == null)
				return;
			
			RACommandEvent ev = new RACommandEvent()
			{
				Allow = allow,
				Command = query,
				Sender = sender
			};
			adminCommandEvent?.Invoke(ref ev);
			query = ev.Command;
			sender = ev.Sender;
			allow = ev.Allow;
		}
		
		public static event OnCheaterReport CheaterReportEvent;
		public delegate void OnCheaterReport(ref CheaterReportEvent ev);

		public static void InvokeCheaterReport(string reporterId, string reportedId, string reportedIp, string reason, int serverId, ref bool allow)
		{
			OnCheaterReport onCheaterReport = CheaterReportEvent;
			if (onCheaterReport == null)
				return;
			
			CheaterReportEvent ev = new CheaterReportEvent()
			{
				Allow = allow,
				Report = reason,
				ReportedId = reportedId,
				ReportedIp = reportedIp,
				ReporterId = reporterId
			};
			onCheaterReport?.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event OnWarheadCommand WarheadCommandEvent;
		public delegate void OnWarheadCommand(ref WarheadLeverEvent ev);

		public static void InvokeWarheadEvent(PlayerInteract interaction, ref string n, ref bool allow)
		{
			OnWarheadCommand onWarheadCommand = WarheadCommandEvent;
			if (onWarheadCommand == null)
				return;
			
			WarheadLeverEvent ev = new WarheadLeverEvent()
			{
				Player = Plugin.GetPlayer(interaction.gameObject),
				Allow = allow
			};
			onWarheadCommand?.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event OnDoorInteract DoorInteractEvent;
		public delegate void OnDoorInteract(ref DoorInteractionEvent ev);

		public static void InvokeDoorInteract(GameObject player, Door door, ref bool allow)
		{
			OnDoorInteract onDoorInteract = DoorInteractEvent;
			if (onDoorInteract == null)
				return;
			
			DoorInteractionEvent ev = new DoorInteractionEvent()
			{
				Player = Plugin.GetPlayer(player),
				Allow = allow,
				Door = door
			};
			onDoorInteract?.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event OnPlayerJoin PlayerJoinEvent;
		public delegate void OnPlayerJoin(PlayerJoinEvent ev);
		public static void InvokePlayerJoin(ReferenceHub hub)
		{
			OnPlayerJoin onPlayerJoin = PlayerJoinEvent;
			if (onPlayerJoin == null)
				return;
			
			PlayerJoinEvent ev = new PlayerJoinEvent()
			{
				Player = hub
			};
			onPlayerJoin?.Invoke(ev);
		}

		public static event OnPlayerLeave PlayerLeaveEvent;
		public delegate void OnPlayerLeave(PlayerLeaveEvent ev);
		public static void InvokePlayerLeave(ReferenceHub hub, string userId, GameObject obj)
		{
			OnPlayerLeave onPlayerLeave = PlayerLeaveEvent;
			if (onPlayerLeave == null)
				return;
			
			PlayerLeaveEvent ev = new PlayerLeaveEvent()
			{
				Player = hub
			};
			onPlayerLeave?.Invoke(ev);
		}
		public static event OnConsoleCommand ConsoleCommandEvent;
		public delegate void OnConsoleCommand(ConsoleCommandEvent ev);
		public static void InvokeConsoleCommand(GameObject obj, string command, bool encrypted, out string returnMessage, out string color)
		{
			OnConsoleCommand onConsoleCommand = ConsoleCommandEvent;
			if (onConsoleCommand == null)
			{
				returnMessage = string.Empty;
				color = null;
				return;
			}

			ReferenceHub hub = Plugin.GetPlayer(obj);
			ConsoleCommandEvent ev = new ConsoleCommandEvent(encrypted)
			{
				Command = command,
				Player = hub,
				ReturnMessage = "Command not found.",
				Color = "red"
			};
			onConsoleCommand?.Invoke(ev);
			returnMessage = ev.ReturnMessage;
			color = ev.Color;
		}
	}
}