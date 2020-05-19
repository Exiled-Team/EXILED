using EXILED.Extensions;
using Grenades;
using Scp914;
using System.Collections.Generic;
using UnityEngine;

namespace EXILED
{
	public partial class Events
	{
		public static event UsedMedicalItem UsedMedicalItemEvent;
		public delegate void UsedMedicalItem(UsedMedicalItemEvent ev);

		public static void InvokeUsedMedicalItem(GameObject player, ItemType itemType)
		{
			if (UsedMedicalItemEvent == null)
				return;

			UsedMedicalItemEvent ev = new UsedMedicalItemEvent()
			{
				Player = player.GetPlayer(),
				ItemType = itemType
			};

			UsedMedicalItemEvent.InvokeSafely(ev);
		}

		public static event CancelMedicalItem CancelMedicalItemEvent;
		public delegate void CancelMedicalItem(MedicalItemEvent ev);

		public static void InvokeCancelMedicalItem(GameObject player, ItemType itemType, ref float cooldown, ref bool allow)
		{
			if (CancelMedicalItemEvent == null)
				return;

			MedicalItemEvent ev = new MedicalItemEvent()
			{
				Player = player.GetPlayer(),
				Item = itemType,
				Cooldown = cooldown,
				Allow = allow
			};

			CancelMedicalItemEvent.InvokeSafely(ev);

			cooldown = ev.Cooldown;
			allow = ev.Allow;
		}

		public static event PlayerInteract PlayerInteractEvent;
		public delegate void PlayerInteract(PlayerInteractEvent ev);

		public static void InvokePlayerInteract(GameObject player)
		{
			if (PlayerInteractEvent == null)
				return;

			PlayerInteractEvent ev = new PlayerInteractEvent()
			{
				Player = player.GetPlayer()
			};

			PlayerInteractEvent.InvokeSafely(ev);
		}

		public static event SpawnRagdoll SpawnRagdollEvent;
		public delegate void SpawnRagdoll(SpawnRagdollEvent ev);

		public static void InvokeSpawnRagdoll(GameObject player, ref Vector3 position, ref Quaternion rotation, ref int roleType, ref PlayerStats.HitInfo hitInfo, ref bool allowRecall, ref string ragdollUserId, ref string ragdollPlayerName, ref int ragdollPlayerId, ref bool allow)
		{
			if (SpawnRagdollEvent == null)
				return;

			SpawnRagdollEvent ev = new SpawnRagdollEvent()
			{
				Killer = hitInfo.PlyId == 0 ? null : Player.GetPlayer(hitInfo.PlyId),
				Player = player.GetPlayer(),
				Position = position,
				Rotation = rotation,
				RoleType = (RoleType)roleType,
				HitInfo = hitInfo,
				AllowRecall = allowRecall,
				RagdollDissonanceId = ragdollUserId,
				RagdollPlayerName = ragdollPlayerName,
				RagdollPlayerId = ragdollPlayerId,
				Allow = allow
			};

			SpawnRagdollEvent.InvokeSafely(ev);

			position = ev.Position;
			rotation = ev.Rotation;
			roleType = (int)ev.RoleType;
			hitInfo = ev.HitInfo;
			allowRecall = ev.AllowRecall;
			ragdollUserId = ev.RagdollDissonanceId;
			ragdollPlayerName = ev.RagdollPlayerName;
			ragdollPlayerId = ev.RagdollPlayerId;
			allow = ev.Allow;
		}

		public static event Scp106CreatedPortal Scp106CreatedPortalEvent;
		public delegate void Scp106CreatedPortal(Scp106CreatedPortalEvent ev);

		public static void InvokeScp106CreatedPortal(GameObject player, ref bool allow, ref Vector3 portalPosition)
		{
			if (Scp106CreatedPortalEvent == null)
				return;

			Scp106CreatedPortalEvent ev = new Scp106CreatedPortalEvent()
			{
				Player = player.GetPlayer(),
				Allow = allow,
				PortalPosition = portalPosition
			};

			Scp106CreatedPortalEvent.InvokeSafely(ev);

			allow = ev.Allow;
			portalPosition = ev.PortalPosition;
		}

		public static event ItemChanged ItemChangedEvent;
		public delegate void ItemChanged(ItemChangedEvent ev);

		public static void InvokeItemChanged(GameObject player, ref Inventory.SyncItemInfo oldItem, Inventory.SyncItemInfo newItem)
		{
			if (ItemChangedEvent == null)
				return;

			ItemChangedEvent ev = new ItemChangedEvent()
			{
				Player = player.GetPlayer(),
				OldItem = oldItem,
				NewItem = newItem
			};

			ItemChangedEvent.InvokeSafely(ev);

			oldItem = ev.OldItem;
		}

		public static event Scp079ExpGain Scp079ExpGainEvent;
		public delegate void Scp079ExpGain(Scp079ExpGainEvent ev);

		public static void InvokeScp079ExpGain(GameObject player, ExpGainType expGainType, ref bool allow, ref float amount)
		{
			if (Scp079ExpGainEvent == null)
				return;

			Scp079ExpGainEvent ev = new Scp079ExpGainEvent()
			{
				Player = player.GetPlayer(),
				GainType = expGainType,
				Allow = allow,
				Amount = amount
			};

			Scp079ExpGainEvent.InvokeSafely(ev);

			allow = ev.Allow;
			amount = ev.Amount;
		}

		public static event Scp079LvlGain Scp079LvlGainEvent;
		public delegate void Scp079LvlGain(Scp079LvlGainEvent ev);

		public static void InvokeScp079LvlGain(GameObject player, int oldLevel, ref int newLevel, ref bool allow)
		{
			if (Scp079LvlGainEvent == null)
				return;

			Scp079LvlGainEvent ev = new Scp079LvlGainEvent()
			{
				Player = player.GetPlayer(),
				OldLvl = oldLevel,
				NewLvl = newLevel,
				Allow = allow
			};

			Scp079LvlGainEvent.InvokeSafely(ev);

			allow = ev.Allow;
			newLevel = ev.NewLvl;
		}

		public static event WarheadKeycardAccess WarheadKeycardAccessEvent;
		public delegate void WarheadKeycardAccess(WarheadKeycardAccessEvent ev);

		public static void InvokeWarheadKeycardAccess(GameObject player, ref bool allow, ref string requiredPermission)
		{
			if (WarheadKeycardAccessEvent == null)
				return;

			WarheadKeycardAccessEvent ev = new WarheadKeycardAccessEvent()
			{
				Player = player.GetPlayer(),
				Allow = allow,
				RequiredPerms = requiredPermission
			};

			WarheadKeycardAccessEvent.InvokeSafely(ev);

			allow = ev.Allow;
			requiredPermission = ev.RequiredPerms;
		}

		public static event UseMedicalItem UseMedicalItemEvent;
		public delegate void UseMedicalItem(MedicalItemEvent ev);

		public static void InvokeUseMedicalItem(GameObject player, ItemType itemType, ref float cooldown, ref bool allow)
		{
			if (UseMedicalItemEvent == null)
				return;

			MedicalItemEvent ev = new MedicalItemEvent()
			{
				Player = player.GetPlayer(),
				Item = itemType,
				Cooldown = cooldown,
				Allow = allow
			};

			UseMedicalItemEvent.InvokeSafely(ev);

			cooldown = ev.Cooldown;
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
				Player = script.gameObject.GetPlayer(),
				Script = script,
				Allow = allow
			};

			Scp096EnrageEvent.InvokeSafely(ev);

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
				Player = script.gameObject.GetPlayer(),
				Script = script,
				Allow = allow
			};

			Scp096CalmEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event OnPlayerJoin PlayerJoinEvent;
		public delegate void OnPlayerJoin(PlayerJoinEvent ev);

		public static void InvokePlayerJoin(ReferenceHub player) => PlayerJoinEvent.InvokeSafely(new PlayerJoinEvent() { Player = player });

		public static event OnPlayerLeave PlayerLeaveEvent;
		public delegate void OnPlayerLeave(PlayerLeaveEvent ev);

		public static void InvokePlayerLeave(ReferenceHub player) => PlayerLeaveEvent.InvokeSafely(new PlayerLeaveEvent() { Player = player });

		public static event OnConsoleCommand ConsoleCommandEvent;
		public delegate void OnConsoleCommand(ConsoleCommandEvent ev);

		public static void InvokeConsoleCommand(GameObject player, string command, bool isEncrypted, out string returnMessage, out string color)
		{
			if (ConsoleCommandEvent == null)
			{
				returnMessage = "Command not found.";
				color = "red";
				return;
			}

			ConsoleCommandEvent ev = new ConsoleCommandEvent(isEncrypted)
			{
				Command = command,
				Player = player.GetPlayer(),
				ReturnMessage = "Command not found.",
				Color = "red"
			};

			ConsoleCommandEvent.InvokeSafely(ev);

			returnMessage = ev.ReturnMessage;
			color = ev.Color;
		}

		public static event PlayerHurt PlayerHurtEvent;
		public delegate void PlayerHurt(ref PlayerHurtEvent ev);

		public static void InvokePlayerHurt(PlayerStats stats, ref PlayerStats.HitInfo hitInfo, GameObject player, int playerId = 0)
		{
			if (PlayerHurtEvent == null)
				return;

			PlayerHurtEvent ev = new PlayerHurtEvent
			{
				Attacker = playerId == 0 ? stats.gameObject.GetPlayer() : Player.GetPlayer(playerId),
				Player = player.GetPlayer(),
				Info = hitInfo
			};

			if (string.IsNullOrEmpty(ev.Player.GetUserId()))
				return;

			PlayerHurtEvent.InvokeSafely(ev);

			hitInfo = ev.Info;
		}

		public static event PlayerDeath PlayerDeathEvent;
		public delegate void PlayerDeath(ref PlayerDeathEvent ev);

		public static void InvokePlayerDeath(PlayerStats stats, ref PlayerStats.HitInfo hitInfo, GameObject player, int playerId = 0)
		{
			if (PlayerDeathEvent == null)
				return;

			PlayerDeathEvent ev = new PlayerDeathEvent()
			{
				Killer = playerId == 0 ? stats.gameObject.GetPlayer() : Player.GetPlayer(playerId),
				Player = player.GetPlayer(),
				Info = hitInfo
			};

			if (string.IsNullOrEmpty(ev.Player.GetUserId()) || ev.Player.GetRole() == RoleType.Spectator)
				return;

			PlayerDeathEvent.InvokeSafely(ev);

			hitInfo = ev.Info;
		}

		public static event SetClass SetClassEvent;
		public delegate void SetClass(SetClassEvent ev);

		public static void InvokeSetClass(CharacterClassManager characterClassManager, RoleType roleType)
		{
			if (SetClassEvent == null)
				return;

			SetClassEvent ev = new SetClassEvent()
			{
				Player = characterClassManager.gameObject.GetPlayer(),
				Role = roleType
			};

			SetClassEvent.InvokeSafely(ev);
		}

		public static event StartItems StartItemsEvent;
		public delegate void StartItems(StartItemsEvent ev);

		public static void InvokeStartItems(GameObject player, RoleType roleType, ref List<ItemType> startingItems)
		{
			if (StartItemsEvent == null)
				return;

			StartItemsEvent ev = new StartItemsEvent()
			{
				Player = player.GetPlayer(),
				Role = roleType,
				StartItems = startingItems
			};

			StartItemsEvent.InvokeSafely(ev);

			startingItems = ev.StartItems;
		}

		public static event GrenadeThrown GrenadeThrownEvent;
		public delegate void GrenadeThrown(ref GrenadeThrownEvent ev);

		public static void InvokeGrenadeThrown(ref GrenadeManager grenadeManager, ref int grenadeId, ref bool slow, ref double fuse, ref bool allow)
		{
			if (GrenadeThrownEvent == null)
				return;

			GrenadeThrownEvent ev = new GrenadeThrownEvent()
			{
				Player = grenadeManager.gameObject.GetPlayer(),
				Gm = grenadeManager,
				Id = grenadeId,
				Allow = allow,
				Slow = slow,
				Fuse = fuse
			};

			GrenadeThrownEvent.InvokeSafely(ev);

			allow = ev.Allow;
			grenadeId = ev.Id;
			grenadeManager = ev.Gm;
			slow = ev.Slow;
			fuse = ev.Fuse;
		}

		public static event DropItem DropItemEvent;
		public delegate void DropItem(ref DropItemEvent ev);

		public static void InvokeDropItem(GameObject player, ref Inventory.SyncItemInfo item, ref bool allow)
		{
			if (DropItemEvent == null)
				return;

			DropItemEvent ev = new DropItemEvent()
			{
				Player = player.GetPlayer(),
				Item = item,
				Allow = allow
			};

			DropItemEvent.InvokeSafely(ev);

			allow = ev.Allow;
			item = ev.Item;
		}

		public static event ItemDropped ItemDroppedEvent;
		public delegate void ItemDropped(ItemDroppedEvent ev);

		public static void InvokeItemDropped(GameObject player, Pickup pickup, Inventory.SyncItemInfo syncItemInfo)
		{
			if (ItemDroppedEvent == null)
				return;

			ItemDroppedEvent ev = new ItemDroppedEvent
			{
				Player = player.GetPlayer(),
				Item = pickup,
				SyncItemInfo = syncItemInfo
			};

			ItemDroppedEvent.InvokeSafely(ev);
		}

		public static event PickupItem PickupItemEvent;
		public delegate void PickupItem(ref PickupItemEvent ev);

		public static void InvokePickupItem(GameObject player, ref Pickup pickup, ref bool allow)
		{
			if (PickupItemEvent == null)
				return;

			PickupItemEvent ev = new PickupItemEvent()
			{
				Player = player.GetPlayer(),
				Item = pickup,
				Allow = allow
			};

			PickupItemEvent.InvokeSafely(ev);
			allow = ev.Allow;
			pickup = ev.Item;
		}

		public static event HandcuffCuffed PlayerHandcuffedEvent;
		public delegate void HandcuffCuffed(ref HandcuffEvent ev);

		public static void InvokePlayerHandcuff(GameObject player, GameObject target, ref bool allow)
		{
			if (PlayerHandcuffedEvent == null)
				return;

			HandcuffEvent ev = new HandcuffEvent()
			{
				Player = player.GetPlayer(),
				Target = target.GetPlayer(),
				Allow = allow
			};

			PlayerHandcuffedEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event HandcuffFreed PlayerHandcuffFreedEvent;
		public delegate void HandcuffFreed(ref HandcuffEvent ev);

		public static void InvokePlayerHandcuffFree(GameObject player, GameObject target, ref bool allow)
		{
			if (PlayerHandcuffFreedEvent == null)
				return;

			HandcuffEvent ev = new HandcuffEvent()
			{
				Player = player.GetPlayer(),
				Target = target.GetPlayer(),
				Allow = allow
			};

			PlayerHandcuffFreedEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event Scp079TriggerTesla Scp079TriggerTeslaEvent;
		public delegate void Scp079TriggerTesla(ref Scp079TriggerTeslaEvent ev);

		public static void InvokeScp079TriggerTesla(GameObject player, ref bool allow)
		{
			if (Scp079TriggerTeslaEvent == null)
				return;

			Scp079TriggerTeslaEvent ev = new Scp079TriggerTeslaEvent
			{
				Player = player.GetPlayer(),
				Allow = allow
			};

			Scp079TriggerTeslaEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event CheckEscape CheckEscapeEvent;
		public delegate void CheckEscape(ref CheckEscapeEvent ev);

		public static void InvokeCheckEscape(GameObject player, ref bool allow)
		{
			if (CheckEscapeEvent == null)
				return;

			CheckEscapeEvent ev = new CheckEscapeEvent
			{
				Player = player.GetPlayer(),
				Allow = allow
			};

			CheckEscapeEvent.InvokeSafely(ev);
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
				Player = player.GetPlayer(),
				Allow = allow
			};

			IntercomSpeakEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event OnLateShoot LateShootEvent;
		public delegate void OnLateShoot(ref LateShootEvent ev);

		public static void InvokeOnLateShoot(ReferenceHub shooter, GameObject target, ref float damage, float distance, string hitboxType, ref bool allow)
		{
			if (LateShootEvent == null)
				return;

			LateShootEvent ev = new LateShootEvent()
			{
				Shooter = shooter,
				Target = target,
				Damage = damage,
				Distance = distance,
				HitboxType = hitboxType,
				Allow = allow
			};

			LateShootEvent.InvokeSafely(ev);

			allow = ev.Allow;
			damage = ev.Damage;
		}

		public static event OnShoot ShootEvent;
		public delegate void OnShoot(ref ShootEvent ev);

		public static void InvokeOnShoot(ReferenceHub shooter, GameObject target, ref bool allow, ref Vector3 targetPosition)
		{
			if (ShootEvent == null)
				return;

			ShootEvent ev = new ShootEvent
			{
				Shooter = shooter,
				Allow = allow,
				Target = target,
				TargetPos = targetPosition
			};

			ShootEvent.InvokeSafely(ev);

			allow = ev.Allow;
			targetPosition = ev.TargetPos;
		}

		public static event Scp106Teleport Scp106TeleportEvent;
		public delegate void Scp106Teleport(Scp106TeleportEvent ev);

		public static void InvokeScp106Teleport(GameObject player, Vector3 portalPosition, ref bool allow)
		{
			if (Scp106TeleportEvent == null)
				return;

			Scp106TeleportEvent ev = new Scp106TeleportEvent()
			{
				Player = player.GetPlayer(),
				PortalPosition = portalPosition,
				Allow = allow
			};

			Scp106TeleportEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event PocketDimDamage PocketDimDamageEvent;
		public delegate void PocketDimDamage(PocketDimDamageEvent ev);

		public static void InvokePocketDimDamage(GameObject player, ref bool allow)
		{
			if (PocketDimDamageEvent == null)
				return;

			PocketDimDamageEvent ev = new PocketDimDamageEvent()
			{
				Player = player.GetPlayer(),
				Allow = allow
			};

			PocketDimDamageEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event PocketDimEnter PocketDimEnterEvent;
		public delegate void PocketDimEnter(PocketDimEnterEvent ev);

		public static void InvokePocketDimEnter(GameObject player, ref bool allow)
		{
			if (PocketDimEnterEvent == null)
				return;

			PocketDimEnterEvent ev = new PocketDimEnterEvent()
			{
				Player = player.GetPlayer(),
				Allow = allow
			};

			PocketDimEnterEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event PocketDimEscaped PocketDimEscapedEvent;
		public delegate void PocketDimEscaped(PocketDimEscapedEvent ev);

		public static void InvokePocketDimEscaped(GameObject player, ref bool allow)
		{
			if (PocketDimEscapedEvent == null)
				return;

			PocketDimEscapedEvent ev = new PocketDimEscapedEvent()
			{
				Player = player.GetPlayer(),
				Allow = allow
			};

			PocketDimEscapedEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event PocketDimDeath PocketDimDeathEvent;
		public delegate void PocketDimDeath(PocketDimDeathEvent ev);

		public static void InvokePocketDimDeath(GameObject player, ref bool allow)
		{
			if (PocketDimDeathEvent == null)
				return;

			PocketDimDeathEvent ev = new PocketDimDeathEvent()
			{
				Player = player.GetPlayer(),
				Allow = allow
			};

			PocketDimDeathEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event PlayerReload PlayerReloadEvent;
		public delegate void PlayerReload(ref PlayerReloadEvent ev);

		public static void InvokePlayerReload(GameObject player, ref bool allow, bool animationOnly)
		{
			if (PlayerReloadEvent == null)
				return;

			PlayerReloadEvent ev = new PlayerReloadEvent
			{
				Player = player.GetPlayer(),
				Allow = allow,
				AnimationOnly = animationOnly
			};

			PlayerReloadEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event PlayerSpawn PlayerSpawnEvent;
		public delegate void PlayerSpawn(PlayerSpawnEvent ev);

		public static void InvokePlayerSpawn(CharacterClassManager characterClassManager, RoleType roleType, ref Vector3 spawnPoint, ref float rotationY)
		{
			if (PlayerSpawnEvent == null)
				return;

			PlayerSpawnEvent ev = new PlayerSpawnEvent
			{
				Player = characterClassManager.gameObject.GetPlayer(),
				Role = roleType,
				Spawnpoint = spawnPoint,
				RotationY = rotationY
			};

			PlayerSpawnEvent.InvokeSafely(ev);

			spawnPoint = ev.Spawnpoint;
			rotationY = ev.RotationY;
		}

		public static event Scp106Contain Scp106ContainEvent;
		public delegate void Scp106Contain(Scp106ContainEvent ev);

		public static void InvokeScp106ContainEvent(GameObject player, ref bool allow)
		{
			if (Scp106ContainEvent == null)
				return;

			Scp106ContainEvent ev = new Scp106ContainEvent
			{
				Player = player.GetPlayer(),
				Allow = allow
			};

			Scp106ContainEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event Scp914Activation Scp914ActivationEvent;
		public delegate void Scp914Activation(ref Scp914ActivationEvent ev);

		public static void InvokeScp914Activation(GameObject player, ref bool allow, ref double time)
		{
			if (Scp914ActivationEvent == null)
				return;

			Scp914ActivationEvent ev = new Scp914ActivationEvent
			{
				Player = player.GetPlayer(),
				Allow = allow,
				Time = time
			};

			Scp914ActivationEvent.InvokeSafely(ev);

			allow = ev.Allow;
			time = ev.Time;
		}

		public static event Scp914KnobChange Scp914KnobChangeEvent;
		public delegate void Scp914KnobChange(ref Scp914KnobChangeEvent ev);

		public static void InvokeScp914KnobChange(GameObject player, ref bool allow, ref Scp914Knob knobSetting)
		{
			if (Scp914KnobChangeEvent == null)
				return;

			Scp914KnobChangeEvent ev = new Scp914KnobChangeEvent
			{
				Player = player.GetPlayer(),
				Allow = allow,
				KnobSetting = knobSetting
			};

			Scp914KnobChangeEvent.InvokeSafely(ev);

			allow = ev.Allow;
			knobSetting = ev.KnobSetting;
		}

		public static event FemurEnter FemurEnterEvent;
		public delegate void FemurEnter(FemurEnterEvent ev);

		public static void InvokeFemurEnterEvent(GameObject player, ref bool allow)
		{
			if (FemurEnterEvent == null)
				return;

			FemurEnterEvent ev = new FemurEnterEvent
			{
				Player = player.GetPlayer(),
				Allow = allow
			};

			FemurEnterEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event SyncData SyncDataEvent;
		public delegate void SyncData(ref SyncDataEvent ev);

		public static void InvokeSyncData(GameObject player, ref int currentAnimation, ref Vector2 speed, ref bool allow)
		{
			if (SyncDataEvent == null)
				return;

			SyncDataEvent ev = new SyncDataEvent
			{
				Player = player.GetPlayer(),
				State = currentAnimation,
				v2 = speed,
				Allow = allow
			};

			SyncDataEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}
	}
}