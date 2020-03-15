using System;
using System.Collections.Generic;
using System.Reflection;
using Grenades;
using LiteNetLib;
using Scp914;
using UnityEngine;
using static BanHandler;

namespace EXILED
{
	public class PlayerJoinEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
	}

	public class GrenadeThrownEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public GrenadeManager Gm { get; internal set; }
		public int Id { get; internal set; }
		public bool Allow { get; set; }
		public bool Slow { get; set; }
		public double Fuse { get; set; }
	}

    public class Scp914UpgradeEvent : EventArgs
    {
	    public bool Allow { get; set; }
        public Scp914Machine Machine { get; internal set; }
        public List<ReferenceHub> Players { get; set; }
        public List<Pickup> Items { get; set; }
        public Scp914Knob KnobSetting { get; set; }
    }

	public class SetClassEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public RoleType Role { get; set; }
	}

    public class StartItemsEvent : EventArgs
    {
        public ReferenceHub Player { get; internal set; }
        public RoleType Role { get; set; }
        public List<ItemType> StartItems { get; set; }
    }

    public class PlayerHurtEvent : EventArgs
    {
	    private PlayerStats.HitInfo info;
		public ReferenceHub Player { get; internal set; }
		public ReferenceHub Attacker { get; set; }
		private DamageTypes.DamageType damageType = DamageTypes.None;

		public int Time => info.Time;

		/// <summary>
		/// The DamageType as a <see cref="DamageTypes.DamageType"/>
		/// </summary>
		public DamageTypes.DamageType DamageType
		{
			get 
			{
				if (damageType == DamageTypes.None)
					damageType = DamageTypes.FromIndex(info.Tool);
				return damageType;
			}
		}
		/// <summary>
		/// The DamageType's <see cref="int"/> value
		/// </summary>
		public int Tool => info.Tool;

		/// <summary>
		/// The amount of damage to be dealt
		/// </summary>
		public float Amount
		{
			get => info.Amount;
			set => info.Amount = value;
		}

		public PlayerStats.HitInfo Info
		{ 
			get => info;
			set
			{
				damageType = DamageTypes.None;
				info = value;
			}
		}
	}

	public class PlayerDeathEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public ReferenceHub Killer { get; internal set; }
		public PlayerStats.HitInfo Info { get; set; }
	}

	public class TriggerTeslaEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Triggerable { get; set; }
	}

	public class TeamRespawnEvent : EventArgs
	{
		public bool IsChaos { get; set; }
		public int MaxRespawnAmt { get; set; }
		public List<ReferenceHub> ToRespawn { get; set; }
	}

	public class MedicalItemEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public ItemType Item { get; internal set; }
		public bool Allow { get; set; }
	}

	public class Scp096EnrageEvent : EventArgs
	{
		public Scp096PlayerScript Script { get; internal set; }
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class Scp096CalmEvent : EventArgs
	{
		public Scp096PlayerScript Script { get; internal set; }
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class PreAuthEvent : EventArgs
	{
		public string UserId { get; set; }
		public ConnectionRequest Request { get; internal set; }
		public bool Allow { get; set; }
	}

	public class RaCommandEvent : EventArgs
	{
		public string Command { get; internal set; }
		public CommandSender Sender { get; internal set; }
		public bool Allow { get; set; }
	}

	public class CheaterReportEvent : EventArgs
	{
		public string ReporterId { get; internal set; }
		public string ReportedId { get; internal set; }
		public string ReportedIp { get; internal set; }
		public string Report { get; internal set; }
		public int ServerId { get; internal set; }
		public bool Allow { get; set; }
	}

	public class DoorInteractionEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Door Door { get; internal set; }
		public bool Allow { get; set; }
	}
	public class LockerInteractionEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Locker Locker { get; internal set; }
		public int LockerId { get; internal set; }
		public bool Allow { get; set; }
	}

	public class PlayerLeaveEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
	}

	public class WarheadLeverEvent : EventArgs
	{
		public bool Allow { get; set; }
		public ReferenceHub Player { get; internal set; }
	}
	public class ConsoleCommandEvent : EventArgs
	{
		public ConsoleCommandEvent(bool encrypted) => Encrypted = encrypted;
		public ReferenceHub Player { get; internal set; }
		public string Command { get; internal set; }
		public string ReturnMessage { get; set; }
		public bool Encrypted { get; private set; }
		public string Color { get; set; }
	}

	public class DropItemEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Inventory.SyncItemInfo Item { get; set; }
		public bool Allow { get; set; }
	}

	public class ItemDroppedEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Pickup Item { get; set; }
	}

	public class PickupItemEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Pickup Item { get; set; }
		public bool Allow { get; set; }
	}

	public class HandcuffEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public ReferenceHub Target { get; internal set; }
		public bool Allow { get; set; }
	}

	public class GeneratorUnlockEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Generator079 Generator { get; internal set; }
		public bool Allow { get; set; }
	}
	public class GeneratorOpenEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Generator079 Generator { get; internal set; }
		public bool Allow { get; set; }
	}
	
	public class GeneratorCloseEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Generator079 Generator { get; internal set; }
		public bool Allow { get; set; }
	}
	
	public class GeneratorInsertTabletEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Generator079 Generator { get; internal set; }
		public bool Allow { get; set; }
	}
	
	public class GeneratorEjectTabletEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Generator079 Generator { get; internal set; }
		public bool Allow { get; set; }
	}
	
	public class GeneratorFinishEvent : EventArgs
	{
		public Generator079 Generator { get; internal set; }
	}

	public class Scp079TriggerTeslaEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class DecontaminationEvent : EventArgs
	{
		public bool Allow { get; set; }
	}

	public class CheckEscapeEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class IntercomSpeakEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class CheckRoundEndEvent : EventArgs
	{
		public bool Allow { get; set; }
		public bool ForceEnd { get; set; }
		public RoundSummary.LeadingTeam LeadingTeam { get; set; }
	}
  
  	public class LateShootEvent : EventArgs
	{
		public GameObject Target { get; internal set; }
		public ReferenceHub Shooter { get; internal set; }
		public float Damage { get; set; }
		public bool Allow { get; set; }
		public float Distance { get; set; }
	}

    public class ShootEvent : EventArgs
    {
	    public ReferenceHub Shooter { get; internal set; }
	    public GameObject Target { get; internal set; }
	    public bool Allow { get; set; }
	    public Vector3 TargetPos { get; set; }
    }

	public class Scp106TeleportEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Vector3 PortalPosition { get; set; }
		public bool Allow { get; set; }
	}

	public class PocketDimDamageEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class PocketDimEscapedEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class PlayerBannedEvent : EventArgs
	{
		public BanDetails Details { get; set; }
		public BanType Type { get; set; }
	}

	public class PlayerBanEvent : EventArgs
	{
		private readonly bool log;
		private string userId;
		private int duration;
		private ReferenceHub bannedPlayer;
		private bool allow = true;

		public string Reason;
		public string FullMessage;

		public PlayerBanEvent(bool log, ReferenceHub bannedPlayer, string reason, string userId, int duration)
		{
			this.log = log;
			this.userId = userId;
			this.duration = duration;
			this.bannedPlayer = bannedPlayer;
			Reason = reason;

			// Set to true in the constructor to avoid triggering the logs.
			allow = true;
		}

		public int Duration
		{
			set
			{
				if (duration == value) return;

				if (log)
					LogBanChange(Assembly.GetCallingAssembly().GetName().Name
					+ $" changed duration: {duration} to {value} for ID: {userId}");

				duration = value;
			}
			get => duration;
		}
		public string UserId
		{
			set
			{
				if (userId == value) return;

				if (userId == null) userId = "(null)";
				
				if(log)
					LogBanChange(Assembly.GetCallingAssembly().GetName().Name
					+ $" changed UserID from {userId} to {value}");

				userId = value;
			}
			get => userId;
		}
		public bool Allow
		{
			set
			{
				if (allow == value) return;
				
				if (log)
					LogBanChange(Assembly.GetCallingAssembly().GetName().Name
					+ $" {(value ? "allowed" : "denied")} banning user with ID: {UserId}");

				allow = value;
			}
			get => allow;
		}
		public ReferenceHub BannedPlayer
		{
			set
			{
				if (value == null || BannedPlayer == value) return;
				if (log)
					LogBanChange(Assembly.GetCallingAssembly().GetName().Name
					+ $" changed the banned player from user {bannedPlayer.nicknameSync.Network_myNickSync} ({bannedPlayer.characterClassManager.UserId}) to {value.nicknameSync.Network_myNickSync} ({value.characterClassManager.UserId})");
				bannedPlayer = value;
			}
			get => bannedPlayer;
		}
		
		private void LogBanChange(string msg)
		{
			string time = TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm:ss.fff zzz");
			object lockObject = ServerLogs.LockObject;
			lock (lockObject)
			{
				Log.Warn($"[ANTI-BACKDOOR]: {msg} - {time}");
			}
		}
	}

	public class PocketDimEnterEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class PocketDimDeathEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class PlayerReloadEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class PlayerSpawnEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public RoleType Role { get; set; }
        public Vector3 Spawnpoint { get; set; }
        public float RotationY { get; set; }
	}

	public class Scp106ContainEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

	public class Scp914ActivationEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
		public double Time { get; set; }
	}

	public class Scp914KnobChangeEvent : EventArgs
	{
		public bool Allow { get; set; }
		public Scp914Knob KnobSetting { get; set; }
		public ReferenceHub Player { get; internal set; }
	}

	public class SetGroupEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public UserGroup Group { get; set; }
		public bool Allow { get; set; }
	}

	public class FemurEnterEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
	}

    public class SyncDataEvent : EventArgs
    {
        public ReferenceHub Player { get; internal set; }
        public int State { get; set; }
        public Vector2 V2 { get; set; }
        public bool Allow { get; set; }
    }

    public class WarheadKeycardAccessEvent : EventArgs
    {
	    public ReferenceHub Player { get; internal set; }
	    public bool Allow { get; set; }
	    public string RequiredPerms { get; set; }
    }

    public class Scp079ExpGainEvent : EventArgs
    {
	    public ReferenceHub Player { get; internal set; }
	    public bool Allow { get; set; }
	    public ExpGainType GainType { get; set; }
	    public float Amount { get; set; }
    }

    public class Scp079LvlGainEvent : EventArgs
    {
	    public ReferenceHub Player { get; internal set; }
	    public bool Allow { get; set; }
	    public int OldLvl { get; internal set; }
	    public int NewLvl { get; set; }
    }

    public class WarheadCancelEvent : EventArgs
    {
	    public ReferenceHub Player { get; internal set; }
	    public bool Allow { get; set; }
    }

    public class WarheadStartEvent : EventArgs
    {
	    public bool Allow { get; set; }
    }

    public class ItemChangedEvent : EventArgs
    {
        public ReferenceHub Player { get; internal set; }
        public Inventory.SyncItemInfo OldItem { get; internal set; }
        public Inventory.SyncItemInfo NewItem { get; set; }
    }
}