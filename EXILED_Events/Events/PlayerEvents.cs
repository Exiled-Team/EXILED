using EXILED.Patches;
using Grenades;
using UnityEngine;

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
				Gm = gm
			};
			grenadeThrown?.Invoke(ref ev);
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

		public static void InvokePickupItem(GameObject player, ref Pickup.PickupInfo item, ref bool allow)
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