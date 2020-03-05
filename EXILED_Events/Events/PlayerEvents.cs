using EXILED.Extensions;
using Grenades;
using Scp914;
using System.Collections.Generic;
using UnityEngine;

namespace EXILED
{
	public partial class Events
	{
		public static event UseMedicalItem UseMedicalItemEvent;
		public delegate void UseMedicalItem(MedicalItemEvent ev);

		public static void InvokeUseMedicalItem(GameObject playerGameObject, ItemType type, ref bool allow)
		{
            if (UseMedicalItemEvent == null)
                return;

			MedicalItemEvent ev = new MedicalItemEvent()
			{
				Player = Player.GetPlayer(playerGameObject),
				Item = type,
				Allow = allow
			};

            UseMedicalItemEvent.Invoke(ev);

			allow = ev.Allow;
		}

		public static event Scp096Enrage Scp096EnrageEvent;
		public delegate void Scp096Enrage(ref Scp096EnrageEvent ev);

		public static void InvokeScp096Enrage(Scp096PlayerScript script, ref bool allow)
		{
            if (Scp096EnrageEvent == null)
                return;

			Scp096EnrageEvent ev = new Scp096EnrageEvent()
			{
				Player = Player.GetPlayer(script.gameObject),
				Script = script,
				Allow = allow
			};

            Scp096EnrageEvent.Invoke(ref ev);

			allow = ev.Allow;
		}

		public static event Scp096Calm Scp096CalmEvent;
		public delegate void Scp096Calm(ref Scp096CalmEvent ev);

		public static void InvokeScp096Calm(Scp096PlayerScript script, ref bool allow)
		{
            if (Scp096CalmEvent == null)
                return;

			Scp096CalmEvent ev = new Scp096CalmEvent()
			{
				Player = Player.GetPlayer(script.gameObject),
				Script = script,
				Allow = allow
			};

            Scp096CalmEvent.Invoke(ref ev);

			allow = ev.Allow;
		}
		
		public static event OnPlayerJoin PlayerJoinEvent;
		public delegate void OnPlayerJoin(PlayerJoinEvent ev);

		public static void InvokePlayerJoin(ReferenceHub playerReferenceHub)
		{
            if (PlayerJoinEvent == null)
                return;

			PlayerJoinEvent ev = new PlayerJoinEvent()
			{
				Player = playerReferenceHub
            };

            PlayerJoinEvent.Invoke(ev);
		}

		public static event OnPlayerLeave PlayerLeaveEvent;
		public delegate void OnPlayerLeave(PlayerLeaveEvent ev);

		public static void InvokePlayerLeave(ReferenceHub playerReferenceHub, string userId, GameObject obj)
		{
            if (PlayerLeaveEvent == null)
                return;

			PlayerLeaveEvent ev = new PlayerLeaveEvent()
			{
				Player = playerReferenceHub
			};

            PlayerLeaveEvent.Invoke(ev);
		}

		public static event OnConsoleCommand ConsoleCommandEvent;
		public delegate void OnConsoleCommand(ConsoleCommandEvent ev);

		public static void InvokeConsoleCommand(GameObject playerGameObject, string command, bool encrypted, ref string returnMessage, ref string color)
		{
            if (ConsoleCommandEvent == null)
                return;

			ConsoleCommandEvent ev = new ConsoleCommandEvent(encrypted)
			{
				Command = command,
				Player = Player.GetPlayer(playerGameObject),
				ReturnMessage = returnMessage,
				Color = color
            };

            ConsoleCommandEvent.Invoke(ev);

			returnMessage = ev.ReturnMessage;
			color = ev.Color;
		}
		
		public static event PlayerHurt PlayerHurtEvent;
		public delegate void PlayerHurt(ref PlayerHurtEvent ev);

		public static void InvokePlayerHurt(PlayerStats stats, ref PlayerStats.HitInfo info, GameObject obj, int pid = 0)
		{
            if (PlayerHurtEvent == null)
                return;

			PlayerHurtEvent ev = new PlayerHurtEvent
			{
				Attacker = pid == 0 ? stats.gameObject.GetPlayer() : Player.GetPlayer(pid),
				Player = obj.GetPlayer(),
				Info = info
			};

			if (string.IsNullOrEmpty(ev.Player.characterClassManager.UserId))
				return;

            PlayerHurtEvent.Invoke(ref ev);

			info = ev.Info;
		}
		
		public static event PlayerDeath PlayerDeathEvent;
		public delegate void PlayerDeath(ref PlayerDeathEvent ev); 

		public static void InvokePlayerDeath(PlayerStats stats, ref PlayerStats.HitInfo info, GameObject obj, int pid = 0)
		{
            if (PlayerDeathEvent == null)
                return;

			PlayerDeathEvent ev = new PlayerDeathEvent()
			{
				Killer = pid == 0 ? stats.gameObject.GetPlayer() : Player.GetPlayer(pid),
				Player = Player.GetPlayer(obj),
				Info = info
			};

			if (string.IsNullOrEmpty(ev.Player.characterClassManager.UserId))
				return;

            PlayerDeathEvent.Invoke(ref ev);

			info = ev.Info;
		}
		
		public static event SetClass SetClassEvent;
		public delegate void SetClass(SetClassEvent ev);

		public static void InvokeSetClass(CharacterClassManager ccm, RoleType id)
		{
            if (SetClassEvent == null)
                return;

			SetClassEvent ev = new SetClassEvent()
			{
				Player = Player.GetPlayer(ccm.gameObject),
				Role = id
			};

            SetClassEvent.Invoke(ev);
		}

        public static event StartItems StartItemsEvent;
        public delegate void StartItems(StartItemsEvent ev);

        public static void InvokeStartItems(GameObject playerGameObject, RoleType id, ref List<ItemType> startItems)
        {
            if (StartItemsEvent == null)
                return;

            StartItemsEvent ev = new StartItemsEvent()
            {
                Player = Player.GetPlayer(playerGameObject),
                Role = id,
                StartItems = startItems
            };

            StartItemsEvent.Invoke(ev);

            startItems = ev.StartItems;
        }

        public static event GrenadeThrown GrenadeThrownEvent;
		public delegate void GrenadeThrown(ref GrenadeThrownEvent ev);

		public static void InvokeGrenadeThrown(ref GrenadeManager gm, ref int id, ref bool slow, ref double fuse, ref bool allow)
		{
            if (GrenadeThrownEvent == null)
                return;

			GrenadeThrownEvent ev = new GrenadeThrownEvent()
			{   
				Player = gm.gameObject.GetPlayer(),
				Gm = gm,
				Id = id,
				Allow = allow,
				Slow = slow,
				Fuse = fuse
			};

            GrenadeThrownEvent.Invoke(ref ev);

			allow = ev.Allow;
			id = ev.Id;
			gm = ev.Gm;
			slow = ev.Slow;
			fuse = ev.Fuse;
		}

		public static event DropItem DropItemEvent;
		public delegate void DropItem(ref DropItemEvent ev);

		public static void InvokeDropItem(GameObject playerGameObject, ref Inventory.SyncItemInfo item, ref bool allow)
		{
            if (DropItemEvent == null)
                return;

			DropItemEvent ev = new DropItemEvent()
			{
				Player = Player.GetPlayer(playerGameObject),
				Item = item,
				Allow = allow
			};

            DropItemEvent.Invoke(ref ev);

			allow = ev.Allow;
			item = ev.Item;
		}

		public static event PickupItem PickupItemEvent;
		public delegate void PickupItem(ref PickupItemEvent ev);

		public static void InvokePickupItem(GameObject playerGameObject, ref Pickup item, ref bool allow)
		{
            if (PickupItemEvent == null)
                return;

			PickupItemEvent ev = new PickupItemEvent()
			{
				Player = Player.GetPlayer(playerGameObject),
				Item = item,
				Allow = allow
			};

            PickupItemEvent.Invoke(ref ev);

			allow = ev.Allow;
			item = ev.Item;
		}

		public static event HandcuffCuffed PlayerHandcuffedEvent;
		public delegate void HandcuffCuffed(ref HandcuffEvent ev);

		public static void InvokePlayerHandcuff(GameObject playerGameObject, GameObject targetGameObject, ref bool allow)
		{
            if (PlayerHandcuffedEvent == null)
                return;

			HandcuffEvent ev = new HandcuffEvent()
			{
				Player = Player.GetPlayer(playerGameObject),
				Target = Player.GetPlayer(targetGameObject),
				Allow = allow
			};

            PlayerHandcuffedEvent.Invoke(ref ev);

			allow = ev.Allow;
		}
		
		public static event HandcuffFreed PlayerHandcuffFreedEvent;
		public delegate void HandcuffFreed(ref HandcuffEvent ev);

		public static void InvokePlayerHandcuffFree(GameObject playerGameObject, GameObject targetGameObject, ref bool allow)
		{
            if (PlayerHandcuffFreedEvent == null)
                return;

			HandcuffEvent ev = new HandcuffEvent()
			{
				Player = Player.GetPlayer(playerGameObject),
				Target = Player.GetPlayer(targetGameObject),
				Allow = allow
			};

            PlayerHandcuffFreedEvent.Invoke(ref ev);

			allow = ev.Allow;
		}

		public static event Scp079TriggerTesla Scp079TriggerTeslaEvent;
		public delegate void Scp079TriggerTesla(ref Scp079TriggerTeslaEvent ev);

		public static void InvokeScp079TriggerTesla(GameObject playerGameObject, ref bool allow)
		{
            if (Scp079TriggerTeslaEvent == null)
                return;

			Scp079TriggerTeslaEvent ev = new Scp079TriggerTeslaEvent
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow
			};

            Scp079TriggerTeslaEvent.Invoke(ref ev);

			allow = ev.Allow;
		}

		public static event CheckEscape CheckEscapeEvent;
		public delegate void CheckEscape(ref CheckEscapeEvent ev);

		public static void InvokeCheckEscape(GameObject playerGameObject, ref bool allow)
		{
            if (CheckEscapeEvent == null)
                return;

            CheckEscapeEvent ev = new CheckEscapeEvent
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow
			};

            CheckEscapeEvent.Invoke(ref ev);

			allow = ev.Allow;
		}

		public static event IntercomSpeak IntercomSpeakEvent;
		public delegate void IntercomSpeak(ref IntercomSpeakEvent ev);

		public static void InvokeIntercomSpeak(GameObject player, ref bool allow)
		{
            if (IntercomSpeakEvent == null)
                return;

			IntercomSpeakEvent ev = new IntercomSpeakEvent
			{
				Player = Player.GetPlayer(player),
				Allow = allow
			};

            IntercomSpeakEvent.Invoke(ref ev);

			allow = ev.Allow;
		}
    
		public static event OnLateShoot LateShootEvent;
		public delegate void OnLateShoot(ref LateShootEvent ev);

		public static void InvokeOnLateShoot(ReferenceHub shooter, GameObject target, float damage, float distance, ref bool allow)
		{
            if (LateShootEvent == null)
                return;

			LateShootEvent ev = new LateShootEvent()
			{
				Shooter = shooter,
				Target = target,
				Damage = damage,
				Distance = distance,
				Allow = allow
			};

            LateShootEvent.Invoke(ref ev);

			allow = ev.Allow;
		}

		public static event OnShoot ShootEvent;
		public delegate void OnShoot(ref ShootEvent ev);

		public static void InvokeOnShoot(ReferenceHub shooter, GameObject target, ref bool allow)
		{
            if (ShootEvent == null)
                return;

			ShootEvent ev = new ShootEvent
			{
				Shooter = shooter,
				Allow = allow,
				Target = target
			};

            ShootEvent.Invoke(ref ev);

			allow = ev.Allow;
		}

		public static event Scp106Teleport Scp106TeleportEvent;
		public delegate void Scp106Teleport(Scp106TeleportEvent ev);

		public static void InvokeScp106Teleport(GameObject playerGameObject, Vector3 PortalPos, ref bool allow)
		{
            if (Scp106TeleportEvent == null)
                return;

			Scp106TeleportEvent ev = new Scp106TeleportEvent()
			{
				Player = Player.GetPlayer(playerGameObject),
				PortalPosition = PortalPos,
				Allow = allow
			};

            Scp106TeleportEvent.Invoke(ev);

			allow = ev.Allow;
		}

		public static event PocketDimDamage PocketDimDamageEvent;
		public delegate void PocketDimDamage(PocketDimDamageEvent ev);

		public static void InvokePocketDimDamage(GameObject playerGameObject, ref bool allow)
		{
            if (PocketDimDamageEvent == null)
                return;

			PocketDimDamageEvent ev = new PocketDimDamageEvent()
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow
			};

			PocketDimDamageEvent.Invoke(ev);

			allow = ev.Allow;
		}

		public static event PocketDimEnter PocketDimEnterEvent;
		public delegate void PocketDimEnter(PocketDimEnterEvent ev);

		public static void InvokePocketDimEnter(GameObject playerGameObject, ref bool allow)
		{
            if (PocketDimEnterEvent == null)
                return;

			PocketDimEnterEvent ev = new PocketDimEnterEvent()
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow
			};

			PocketDimEnterEvent.Invoke(ev);

			allow = ev.Allow;
		}

		public static event PocketDimEscaped PocketDimEscapedEvent;
		public delegate void PocketDimEscaped(PocketDimEscapedEvent ev);

		public static void InvokePocketDimEscaped(GameObject playerGameObject, ref bool allow)
		{
            if (PocketDimEscapedEvent == null)
                return;

			PocketDimEscapedEvent ev = new PocketDimEscapedEvent()
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow
			};

            PocketDimEscapedEvent.Invoke(ev);

			allow = ev.Allow;
		}

		public static event PocketDimDeath PocketDimDeathEvent;
		public delegate void PocketDimDeath(PocketDimDeathEvent ev);

		public static void InvokePocketDimDeath(GameObject playerGameObject, ref bool allow)
		{
            if (PocketDimDeathEvent == null)
                return;

			PocketDimDeathEvent ev = new PocketDimDeathEvent()
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow
			};

			PocketDimDeathEvent.Invoke(ev);

			allow = ev.Allow;
		}

		public static event PlayerReload PlayerReloadEvent;
		public delegate void PlayerReload(ref PlayerReloadEvent ev);

		public static void InvokePlayerReload(GameObject playerGameObject, ref bool allow)
		{
            if (PlayerReloadEvent == null)
                return;

			PlayerReloadEvent ev = new PlayerReloadEvent
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow
			};

            PlayerReloadEvent.Invoke(ref ev);

			allow = ev.Allow;
		}

		public static event PlayerSpawn PlayerSpawnEvent;
		public delegate void PlayerSpawn(PlayerSpawnEvent ev);

		public static void InvokePlayerSpawn(CharacterClassManager ccm, RoleType role, ref Vector3 spawnPoint, ref float rotY)
		{
            if (PlayerSpawnEvent == null)
                return;

			PlayerSpawnEvent ev = new PlayerSpawnEvent
			{
				Player = Player.GetPlayer(ccm.gameObject),
				Role = role,
                Spawnpoint = spawnPoint,
                RotationY = rotY
			};

            PlayerSpawnEvent.Invoke(ev);

            spawnPoint = ev.Spawnpoint;
            rotY = ev.RotationY;
		}

		public static event Scp106Contain Scp106ContainEvent;
		public delegate void Scp106Contain(Scp106ContainEvent ev);

		public static void InvokeScp106ContainEvent(GameObject playerGameObject, ref bool allow)
		{
            if (Scp106ContainEvent == null)
                return;

			Scp106ContainEvent ev = new Scp106ContainEvent
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow
			};

            Scp106ContainEvent.Invoke(ev);

			allow = ev.Allow;
		}
		
		public static event Scp914Activation Scp914ActivationEvent;
		public delegate void Scp914Activation(ref Scp914ActivationEvent ev);

		public static void InvokeScp914Activation(GameObject playerGameObject, ref bool allow, ref double time)
		{
            if (Scp914ActivationEvent == null)
                return;

			Scp914ActivationEvent ev = new Scp914ActivationEvent
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow,
				Time = time
			};

            Scp914ActivationEvent.Invoke(ref ev);

			allow = ev.Allow;
			time = ev.Time;
		}

		public static event Scp914KnobChange Scp914KnobChangeEvent;
		public delegate void Scp914KnobChange(ref Scp914KnobChangeEvent ev);

		public static void InvokeScp914KnobChange(GameObject playerGameObject, ref bool allow, ref Scp914Knob knobSetting)
		{
            if (Scp914KnobChangeEvent == null)
                return;

			Scp914KnobChangeEvent ev = new Scp914KnobChangeEvent
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow,
				KnobSetting = knobSetting
			};

            Scp914KnobChangeEvent.Invoke(ref ev);

			allow = ev.Allow;
			knobSetting = ev.KnobSetting;
		}

		public static event FemurEnter FemurEnterEvent;
		public delegate void FemurEnter(FemurEnterEvent ev);

		public static void InvokeFemurEnterEvent(GameObject playerGameObject, ref bool allow)
		{
            if (FemurEnterEvent == null)
                return;

			FemurEnterEvent ev = new FemurEnterEvent
			{
				Player = Player.GetPlayer(playerGameObject),
				Allow = allow
			};

            FemurEnterEvent.Invoke(ev);

			allow = ev.Allow;
		}

        public static event SyncData SyncDataEvent;
        public delegate void SyncData (ref SyncDataEvent ev);

        public static void InvokeSyncData(GameObject playerGameObject, ref int state, ref Vector2 v2, ref bool allow)
        {
            if (SyncDataEvent == null)
                return;

            SyncDataEvent ev = new SyncDataEvent
            {
                Player = Player.GetPlayer(playerGameObject),
                State = state,
                v2 = v2,
                Allow = allow
            };

            SyncDataEvent.Invoke(ref ev);

            allow = ev.Allow;
        }

        public static event OnScp079GainExperienceEvent Scp079GainExperienceEvent;
        public delegate void OnScp079GainExperienceEvent(Scp079GainExperienceEvent ev);

        public static void InvokeScp079GainExperienceEvent(GameObject playerGameObject, ExpGainType gainType, ref bool allow, ref float amount)
        {
            if (Scp079GainExperienceEvent == null)
                return;

            Scp079GainExperienceEvent ev = new Scp079GainExperienceEvent
            {
                Player = Player.GetPlayer(playerGameObject),
                Allow = allow,
                GainType = gainType,
                Amount = amount
            };

            Scp079GainExperienceEvent.Invoke(ev);

            allow = ev.Allow;
            amount = ev.Amount;
        }
    }
}