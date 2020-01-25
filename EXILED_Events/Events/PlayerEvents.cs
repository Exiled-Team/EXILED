using EXILED.Patches;
using Grenades;
using UnityEngine;
using static BanHandler;

namespace EXILED
{
	public partial class Events
	{
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


		public static event OnShoot OnShootEvent;
		public delegate void OnShoot(OnShootEvent ev);

		public static void InvokeOnShoot(ReferenceHub shooter, GameObject target, float damage, float distance, ref bool allow)
		{
			OnShootEvent ev = new OnShootEvent()
			{
				Shooter = shooter,
				Target = target,
				Damage = damage,
				Distance = distance,
				Allow = allow
			};
			OnShootEvent?.Invoke(ev);
			allow = ev.Allow;
		}

		public static event Scp106Teleport Scp106TeleportEvent;
		public delegate void Scp106Teleport(Scp106TeleportEvent ev);

		public static void InvokeScp106Teleport(GameObject Gplayer, Vector3 PortalPos, ref bool allow)
		{
			Scp106Teleport scp106Teleport = Scp106TeleportEvent;
			if (scp106Teleport == null)
				return;

			ReferenceHub Player = Plugin.GetPlayer(Gplayer);
			Scp106TeleportEvent ev = new Scp106TeleportEvent()
			{
				Player = Player,
				PortalPosition = PortalPos,
				Allow = allow
			};
			scp106Teleport.Invoke(ev);
			allow = ev.Allow;
		}

		public static event PocketDimDamage PocketDimDamageEvent;
		public delegate void PocketDimDamage(PocketDimDamageEvent ev);

		public static void InvokePocketDimDamage(GameObject Gplayer, ref bool allow)
		{
			PocketDimDamage pocketDimDamage = PocketDimDamageEvent;
			if (pocketDimDamage == null)
				return;

			ReferenceHub player = Plugin.GetPlayer(Gplayer);
			PocketDimDamageEvent ev = new PocketDimDamageEvent()
			{
				Player = player,
				Allow = allow
			};
			PocketDimDamageEvent?.Invoke(ev);
			allow = ev.Allow;
		}

		public static event PocketDimEnter PocketDimEnterEvent;
		public delegate void PocketDimEnter(PocketDimEnterEvent ev);

		public static void InvokePocketDimEnter(GameObject Gplayer, ref bool allow)
		{
			PocketDimEnter pocketDimEnter = PocketDimEnterEvent;
			if (pocketDimEnter == null)
				return;

			ReferenceHub player = Plugin.GetPlayer(Gplayer);
			PocketDimEnterEvent ev = new PocketDimEnterEvent()
			{
				Player = player,
				Allow = allow
			};
			PocketDimEnterEvent?.Invoke(ev);
			allow = ev.Allow;
		}

		public static event PocketDimEscaped PocketDimEscapedEvent;
		public delegate void PocketDimEscaped(PocketDimEscapedEvent ev);

		public static void InvokePocketDimEscaped(GameObject Gplayer, ref bool allow)
		{
			PocketDimEscaped pocketDimEscaped = PocketDimEscapedEvent;
			if (pocketDimEscaped == null)
				return;

			ReferenceHub player = Plugin.GetPlayer(Gplayer);
			PocketDimEscapedEvent ev = new PocketDimEscapedEvent()
			{
				Player = player,
				Allow = allow
			};
			PocketDimEscapedEvent?.Invoke(ev);
			allow = ev.Allow;
		}

		public static event PlayerBanned PlayerBannedEvent;
		public delegate void PlayerBanned(PlayerBannedEvent ev);

		public static void InvokePlayerBanned(BanDetails details, BanType type)
		{

		}

		public static event PocketDimDeath PocketDimDeathEvent;
		public delegate void PocketDimDeath(PocketDimDeathEvent ev);

		public static void InvokePocketDimDeath(GameObject Gplayer, ref bool allow)
		{
			PocketDimDeath pocketDimDeath = PocketDimDeathEvent;
			if (pocketDimDeath == null)
				return;

			ReferenceHub player = Plugin.GetPlayer(Gplayer);
			PocketDimDeathEvent ev = new PocketDimDeathEvent()
			{
				Player = player,
				Allow = allow
			};
			PocketDimDeathEvent?.Invoke(ev);
			allow = ev.Allow;
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
				returnMessage = "Command not found.";
				color = "red";
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

			if (string.IsNullOrEmpty(ev.Player.characterClassManager.UserId))
				return;
			playerHurt?.Invoke(ref ev);
			info = ev.Info;
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
			if (string.IsNullOrEmpty(ev.Player.characterClassManager.UserId))
				return;
			
			playerDeath?.Invoke(ref ev);
			info = ev.Info;
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
				Gm = gm,
				Id = id,
				Allow = allow
			};
			grenadeThrown?.Invoke(ref ev);
			allow = ev.Allow;
			id = ev.Id;
			gm = ev.Gm;
		}

		public static event DropItem DropItemEvent;
		public delegate void DropItem(ref DropItemEvent ev);

		public static void InvokeDropItem(GameObject player, ref Inventory.SyncItemInfo item, ref bool allow)
		{
			DropItem dropItem = DropItemEvent;
			if (dropItem == null)
				return;
			DropItemEvent ev = new DropItemEvent()
			{
				Player = Plugin.GetPlayer(player),
				Item = item,
				Allow = allow
			};

			dropItem.Invoke(ref ev);
			allow = ev.Allow;
			item = ev.Item;
		}

		public static event PickupItem PickupItemEvent;

		public delegate void PickupItem(ref PickupItemEvent ev);

		public static void InvokePickupItem(GameObject player, ref Pickup item, ref bool allow)
		{
			PickupItem pickupItem = PickupItemEvent;
			if (pickupItem == null)
				return;
			PickupItemEvent ev = new PickupItemEvent()
			{
				Player = Plugin.GetPlayer(player),
				Item = item,
				Allow = allow
			};
			
			pickupItem.Invoke(ref ev);
			allow = ev.Allow;
			item = ev.Item;
		}

		public static event HandcuffCuffed PlayerHandcuffedEvent;
		public delegate void HandcuffCuffed(ref HandcuffEvent ev);

		public static void InvokePlayerHandcuff(GameObject player, GameObject target, ref bool allow)
		{
			HandcuffCuffed handcuffCuffed = PlayerHandcuffedEvent;
			if (handcuffCuffed == null)
				return;
			HandcuffEvent ev = new HandcuffEvent()
			{
				Player = Plugin.GetPlayer(player),
				Target = Plugin.GetPlayer(target),
				Allow = allow
			};
			handcuffCuffed.Invoke(ref ev);
			allow = ev.Allow;
		}
		
		public static event HandcuffFreed PlayerHandcuffFreedEvent;
		public delegate void HandcuffFreed(ref HandcuffEvent ev);

		public static void InvokePlayerHandcuffFree(GameObject player, GameObject target, ref bool allow)
		{
			HandcuffFreed handcuffFreed = PlayerHandcuffFreedEvent;
			if (handcuffFreed == null)
				return;
			HandcuffEvent ev = new HandcuffEvent()
			{
				Player = Plugin.GetPlayer(player),
				Target = Plugin.GetPlayer(target),
				Allow = allow
			};
			handcuffFreed.Invoke(ref ev);
			allow = ev.Allow;
		}
	}
}