using System.Collections.Generic;
using Grenades;
using LiteNetLib;
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
		public delegate void GrenadeThrown(ref GrenadeManager gm, ref int id, ref bool slow, ref double fuse, ref bool allow);
		public static void InvokeGrenadeThrown(ref GrenadeManager gm, ref int id, ref bool slow, ref double fuse, ref bool allow)
		{
			GrenadeThrown grenadeThrown = GrenadeThrownEvent;
			grenadeThrown?.Invoke(ref gm, ref id, ref slow, ref fuse, ref allow);
		}
		
		public static event SetClass SetClassEvent;
		public delegate void SetClass(CharacterClassManager ccm, RoleType id);
		public static void InvokeSetClass(CharacterClassManager ccm, RoleType id)
		{
			SetClass setClass = SetClassEvent;
			setClass?.Invoke(ccm, id);
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
		public delegate void PlayerHurt(PlayerStats stats, ref PlayerStats.HitInfo info, GameObject obj);
		public static void InvokePlayerHurt(PlayerStats stats, ref PlayerStats.HitInfo info, GameObject obj)
		{
			PlayerHurt playerHurt = PlayerHurtEvent;
			playerHurt?.Invoke(stats, ref info, obj);
		}
		
		public static event TriggerTesla TriggerTeslaEvent;
		public delegate void TriggerTesla(GameObject obj, bool hurtRange, ref bool triggerable);
		public static void InvokeTriggerTesla(GameObject obj, bool hurtRange, ref bool triggerable)
		{
			TriggerTesla triggerTesla = TriggerTeslaEvent;
			triggerTesla?.Invoke(obj, hurtRange, ref triggerable);
		}

		public static event PlayerDeath PlayerDeathEvent;
		public delegate void PlayerDeath(PlayerStats stats, ref PlayerStats.HitInfo info, GameObject obj); 
		public static void InvokePlayerDeath(PlayerStats stats, ref PlayerStats.HitInfo info, GameObject obj)
		{
			PlayerDeath playerDeath = PlayerDeathEvent;
			playerDeath?.Invoke(stats, ref info, obj);
		}

		public static event TeamRespawn TeamRespawnEvent;
		public delegate void TeamRespawn(ref bool isChaos, ref int maxRespawn, ref List<GameObject> toRespawn);

		public static void InvokeTeamRespawn(ref bool isChaos, ref int maxRespawn, ref List<GameObject> toRespawn)
		{
			TeamRespawn teamRespawn = TeamRespawnEvent;
			teamRespawn?.Invoke(ref isChaos, ref maxRespawn, ref toRespawn);
		}

		public static event UseMedicalItem UseMedicalItemEvent;
		public delegate void UseMedicalItem(GameObject obj, ItemType type);

		public static void InvokeUseMedicalItem(GameObject obj, ItemType type)
		{
			UseMedicalItem useMedicalItem = UseMedicalItemEvent;
			useMedicalItem?.Invoke(obj, type);
		}

		public static event Scp096Enrage Scp096EnrageEvent;
		public delegate void Scp096Enrage(Scp096PlayerScript script, ref bool allow);

		public static void InvokeScp096Enrage(Scp096PlayerScript script, ref bool allow)
		{
			Scp096Enrage scp096Enrage = Scp096EnrageEvent;
			scp096Enrage?.Invoke(script, ref allow);
		}

		public static event Scp096Calm Scp096CalmEvent;
		public delegate void Scp096Calm(Scp096PlayerScript script, ref bool allow);

		public static void InvokeScp096Calm(Scp096PlayerScript script, ref bool allow)
		{
			Scp096Calm scp096Calm = Scp096CalmEvent;
			scp096Calm?.Invoke(script, ref allow);
		}
		
		public static event OnRoundStart RoundStartEvent;
		public delegate void OnRoundStart();
		public static void InvokeRoundStart()
		{
			OnRoundStart roundStartEvent = RoundStartEvent;
			roundStartEvent?.Invoke();
		}

		public static event OnPreAuth PreAuthEvent;
		public delegate void OnPreAuth(ref string userid, ConnectionRequest request, ref bool allow);
		public static void InvokePreAuth(
			ref string userid,
			ConnectionRequest request,
			ref bool allow)
		{
			OnPreAuth preAuthEvent = PreAuthEvent;
			preAuthEvent?.Invoke(ref userid, request, ref allow);
		}

		public static event OnRoundEnd RoundEndEvent;
		public delegate void OnRoundEnd();
		public static void InvokeRoundEnd()
		{
			OnRoundEnd roundEndEvent = RoundEndEvent;
			roundEndEvent?.Invoke();
		}
		
		
		public static event OnCommand RemoteAdminCommandEvent;
		public delegate void OnCommand(ref string query, ref CommandSender sender, ref bool allow);
		public static void InvokeCommand(ref string query, ref CommandSender sender, ref bool allow)
		{
			OnCommand adminCommandEvent = RemoteAdminCommandEvent;
			adminCommandEvent?.Invoke(ref query, ref sender, ref allow);
		}
		
		public static event OnCheaterReport CheaterReportEvent;
		public delegate void OnCheaterReport(string reporterId, string reportedId, string reportedIp, string reason, int serverId, ref bool allow);

		public static void InvokeCheaterReport(string reporterId, string reportedId, string reportedIp, string reason, int serverId,
			ref bool allow)
		{
			OnCheaterReport onCheaterReport = CheaterReportEvent;
			onCheaterReport?.Invoke(reporterId, reportedId, reportedIp, reason, serverId, ref allow);
		}

		public static event OnWarheadCommand WarheadCommandEvent;
		public delegate void OnWarheadCommand(PlayerInteract interaction, ref string n, ref bool allow);

		public static void InvokeWarheadEvent(PlayerInteract interaction, ref string n, ref bool allow)
		{
			OnWarheadCommand onWarheadCommand = WarheadCommandEvent;
			onWarheadCommand?.Invoke(interaction, ref n, ref allow);
		}

		public static event OnDoorInteract DoorInteractEvent;
		public delegate void OnDoorInteract(GameObject player, Door door, ref bool allow);

		public static void InvokeDoorInteract(GameObject player, Door door, ref bool allow)
		{
			OnDoorInteract onDoorInteract = DoorInteractEvent;
			onDoorInteract?.Invoke(player, door, ref allow);
		}

		public static event OnPlayerJoin PlayerJoinEvent;
		public delegate void OnPlayerJoin(ReferenceHub hub);
		public static void InvokePlayerJoin(ReferenceHub hub)
		{
			OnPlayerJoin onPlayerJoin = PlayerJoinEvent;
			onPlayerJoin?.Invoke(hub);
		}

		public static event OnPlayerLeave PlayerLeaveEvent;
		public delegate void OnPlayerLeave(ReferenceHub hub, string userId, GameObject obj);
		public static void InvokePlayerLeave(ReferenceHub hub, string userId, GameObject obj)
		{
			OnPlayerLeave onPlayerLeave = PlayerLeaveEvent;
			onPlayerLeave?.Invoke(hub, userId, obj);
		}
	}
}