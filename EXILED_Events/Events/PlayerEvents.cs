using EXILED.Extensions;
using Grenades;
using Scp914;
using UnityEngine;

namespace EXILED
{
	public partial class Events
	{
		public static event UseMedicalItem UseMedicalItemEvent;
		public delegate void UseMedicalItem(MedicalItemEvent ev);

		public static void InvokeUseMedicalItem(GameObject obj, ItemType type, ref bool allow)
		{
			UseMedicalItem useMedicalItem = UseMedicalItemEvent;
			if (useMedicalItem == null)
				return;

			MedicalItemEvent ev = new MedicalItemEvent()
			{
				Player = Player.GetPlayer(obj),
				Item = type,
				Allow = allow
			};
			useMedicalItem?.Invoke(ev);
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
				Player = Player.GetPlayer(script.gameObject),
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
				Player = Player.GetPlayer(script.gameObject),
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

			ReferenceHub hub = Player.GetPlayer(obj);
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
				Attacker = Player.GetPlayer(stats.gameObject),
				Player = Player.GetPlayer(obj),
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
				Killer = Player.GetPlayer(stats.gameObject),
				Player = Player.GetPlayer(obj),
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
				Player = Player.GetPlayer(ccm.gameObject),
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
				Player = Player.GetPlayer(gm.gameObject),
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
				Player = Player.GetPlayer(player),
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
				Player = Player.GetPlayer(player),
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
				Player = Player.GetPlayer(player),
				Target = Player.GetPlayer(target),
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
				Player = Player.GetPlayer(player),
				Target = Player.GetPlayer(target),
				Allow = allow
			};
			handcuffFreed.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event Scp079TriggerTesla Scp079TriggerTeslaEvent;
		public delegate void Scp079TriggerTesla(ref Scp079TriggerTeslaEvent ev);
		public static void InvokeScp079TriggerTesla(GameObject player, ref bool allow)
		{
			Scp079TriggerTesla scp079TriggerTesla = Scp079TriggerTeslaEvent;
			if (scp079TriggerTesla == null)
				return;
			
			Scp079TriggerTeslaEvent ev = new Scp079TriggerTeslaEvent
			{
				Player = Player.GetPlayer(player),
				Allow = allow
			};
			
			scp079TriggerTesla.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event CheckEscape CheckEscapeEvent;

		public delegate void CheckEscape(ref CheckEscapeEvent ev);

		public static void InvokeCheckEscape(GameObject ply, ref bool allow)
		{
			CheckEscape checkEscape = CheckEscapeEvent;
			if (checkEscape == null)
				return;
			
			CheckEscapeEvent ev = new CheckEscapeEvent
			{
				Player = Player.GetPlayer(ply),
				Allow = allow
			};
			
			checkEscape.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event IntercomSpeak IntercomSpeakEvent;

		public delegate void IntercomSpeak(ref IntercomSpeakEvent ev);

		public static void InvokeIntercomSpeak(GameObject player, ref bool allow)
		{
			IntercomSpeak intercomSpeak = IntercomSpeakEvent;
			if (intercomSpeak == null)
				return;
			
			IntercomSpeakEvent ev = new IntercomSpeakEvent
			{
				Player = Player.GetPlayer(player),
				Allow = allow
			};
			
			intercomSpeak.Invoke(ref ev);
			allow = ev.Allow;
		}
    
		public static event OnLateShoot LateShootEvent;
		public delegate void OnLateShoot(ref LateShootEvent ev);

		public static void InvokeOnLateShoot(ReferenceHub shooter, GameObject target, float damage, float distance, ref bool allow)
		{
			OnLateShoot onLateShoot = LateShootEvent;
			if (onLateShoot == null)
				return;
			
			LateShootEvent ev = new LateShootEvent()
			{
				Shooter = shooter,
				Target = target,
				Damage = damage,
				Distance = distance,
				Allow = allow
			};
			onLateShoot.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event OnShoot ShootEvent;

		public delegate void OnShoot(ref ShootEvent ev);

		public static void InvokeOnShoot(ReferenceHub shooter, GameObject target, ref bool allow)
		{
			OnShoot onShoot = ShootEvent;
			if (onShoot == null)
				return;
			ShootEvent ev = new ShootEvent
			{
				Shooter = shooter,
				Allow = allow,
				Target = target
			};
			onShoot.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event Scp106Teleport Scp106TeleportEvent;
		public delegate void Scp106Teleport(Scp106TeleportEvent ev);

		public static void InvokeScp106Teleport(GameObject Gplayer, Vector3 PortalPos, ref bool allow)
		{
			Scp106Teleport scp106Teleport = Scp106TeleportEvent;
			if (scp106Teleport == null)
				return;
			
			Scp106TeleportEvent ev = new Scp106TeleportEvent()
			{
				Player = Player.GetPlayer(Gplayer),
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

			ReferenceHub player = Player.GetPlayer(Gplayer);
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

			ReferenceHub player = Player.GetPlayer(Gplayer);
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

			ReferenceHub player = Player.GetPlayer(Gplayer);
			PocketDimEscapedEvent ev = new PocketDimEscapedEvent()
			{
				Player = player,
				Allow = allow
			};
			PocketDimEscapedEvent?.Invoke(ev);
			allow = ev.Allow;
		}

		public static event PocketDimDeath PocketDimDeathEvent;
		public delegate void PocketDimDeath(PocketDimDeathEvent ev);

		public static void InvokePocketDimDeath(GameObject Gplayer, ref bool allow)
		{
			PocketDimDeath pocketDimDeath = PocketDimDeathEvent;
			if (pocketDimDeath == null)
				return;

			ReferenceHub player = Player.GetPlayer(Gplayer);
			PocketDimDeathEvent ev = new PocketDimDeathEvent()
			{
				Player = player,
				Allow = allow
			};
			PocketDimDeathEvent?.Invoke(ev);
			allow = ev.Allow;
		}

		public static event PlayerReload PlayerReloadEvent;

		public delegate void PlayerReload(ref PlayerReloadEvent ev);

		public static void InvokePlayerReload(GameObject player, ref bool allow)
		{
			PlayerReload playerReload = PlayerReloadEvent;
			if (playerReload == null)
				return;
			
			PlayerReloadEvent ev = new PlayerReloadEvent
			{
				Player = Player.GetPlayer(player),
				Allow = allow
			};
			
			playerReload.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event PlayerSpawn PlayerSpawnEvent;
		public delegate void PlayerSpawn(PlayerSpawnEvent ev);
		public static void InvokePlayerSpawn(ReferenceHub player, RoleType role)
		{
			PlayerSpawn playerSpawn = PlayerSpawnEvent;
			if (playerSpawn == null)
				return; 
			PlayerSpawnEvent ev = new PlayerSpawnEvent
			{
				Player = player,
				Role = role
			};
			playerSpawn.Invoke(ev);
		}

		public static event Scp106Contain Scp106ContainEvent;
		public delegate void Scp106Contain(Scp106ContainEvent ev);
		public static void InvokeScp106ContainEvent(GameObject player, ref bool allow)
		{
			Scp106Contain scp106Contain = Scp106ContainEvent;
			if (scp106Contain == null)
				return;
			Scp106ContainEvent ev = new Scp106ContainEvent
			{
				Player = Player.GetPlayer(player),
				Allow = allow
			};
			scp106Contain?.Invoke(ev);
			allow = ev.Allow;
		}
		
		public static event Scp914Activation Scp914ActivationEvent;
		public delegate void Scp914Activation(ref Scp914ActivationEvent ev);
		public static void InvokeScp914Activation(GameObject obj, ref bool allow, ref double time)
		{
			Scp914Activation scp914Activation = Scp914ActivationEvent;
			if (scp914Activation == null)
				return;
			ReferenceHub player = Player.GetPlayer(obj);
			Scp914ActivationEvent ev = new Scp914ActivationEvent
			{
				Player = player,
				Allow = allow,
				Time = time
			};
			scp914Activation.Invoke(ref ev);
			allow = ev.Allow;
			time = ev.Time;
		}

		public static event Scp914KnobChange Scp914KnobChangeEvent;

		public delegate void Scp914KnobChange(ref Scp914KnobChangeEvent ev);

		public static void InvokeScp914KnobChange(GameObject player, ref bool allow, ref Scp914Knob knobSetting)
		{
			Scp914KnobChange scp914KnobChange = Scp914KnobChangeEvent;
			if (scp914KnobChange == null)
				return;
			ReferenceHub hub = Player.GetPlayer(player);
			Scp914KnobChangeEvent ev = new Scp914KnobChangeEvent
			{
				Player = hub,
				Allow = allow,
				KnobSetting = knobSetting
			};
			scp914KnobChange.Invoke(ref ev);
			allow = ev.Allow;
			knobSetting = ev.KnobSetting;
		}
	}
}