using Grenades;
using LiteNetLib;
using Scp914;
using System;
using System.Collections.Generic;
using System.Reflection;
using LiteNetLib.Utils;
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
		public ReferenceHub Player { get; set; }
		public GrenadeManager Gm { get; set; }
		public int Id { get; set; }
		public bool Allow { get; set; }
		public bool Slow { get; set; }
		public double Fuse { get; set; }
	}

	public class SCP914UpgradeEvent : EventArgs
	{
		public bool Allow;
		public Scp914.Scp914Machine Machine;
		public List<ReferenceHub> Players;
		public List<Pickup> Items;
		public Scp914.Scp914Knob KnobSetting;
	}

	public class SetClassEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public RoleType Role { get; set; }
	}

	public class StartItemsEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public RoleType Role { get; set; }
		public List<ItemType> StartItems { get; set; }
	}

	public class PlayerHurtEvent : EventArgs
	{
		private PlayerStats.HitInfo info;
		public ReferenceHub Player { get; set; }
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
		public ReferenceHub Player { get; set; }
		public ReferenceHub Killer { get; set; }
		public PlayerStats.HitInfo Info { get; set; }
	}

	public class TriggerTeslaEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public bool IsInHurtingRange { get; internal set; }
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
		public ReferenceHub Player { get; set; }
		public ItemType Item { get; set; }
		public float Cooldown { get; set; }
		public bool Allow { get; set; }
	}

	public class Scp096EnrageEvent : EventArgs
	{
		public Scp096PlayerScript Script { get; set; }
		public ReferenceHub Player { get; set; }
		public bool Allow { get; set; }
	}

	public class Scp096CalmEvent : EventArgs
	{
		public Scp096PlayerScript Script { get; set; }
		public ReferenceHub Player { get; set; }
		public bool Allow { get; set; }
	}

	public class PreauthEvent : EventArgs
	{
		public PreauthEvent(string UserId, ConnectionRequest request, int position, byte flags, string country)
		{
			this.UserId = UserId;
			Request = request;
			ReaderStartPosition = position;
			Flags = flags;
			Country = country;
		}

		public string UserId { get; internal set; }
		public readonly int ReaderStartPosition;
		public readonly byte Flags;
		public readonly string Country;
		public ConnectionRequest Request { get; internal set; }

		private bool _allow = true;
		
		public bool Allow
		{
			get => _allow;
			set
			{
				if (value)
				{
					Log.Warn("Setting ev.Allow to true inside of the PreauthEvent is no longer allowed. This call will be ignored. Note: This should have no affect on how the plugin making this call functions. This is NOT AN ERROR!");
					return;
				}
				_allow = false;
			}
		}

		public void Disallow() => _allow = false;

		public void Reject(RejectionReason reason) => InternalReject(reason);

		public void Reject(string reason)
		{
			if (reason != null && reason.Length > 400) throw new Exception("Reason can't be longer than 400 characters.");
			InternalReject(RejectionReason.Custom, reason);
		}

        public void RejectBanned(string reason)
        {
            if (reason != null && reason.Length > 400) throw new Exception("Reason can't be longer than 400 characters.");
            InternalReject(RejectionReason.Banned, reason);
        }

		public void RejectBanned(string reason, long expiration)
        {
            if (reason != null && reason.Length > 400) throw new Exception("Reason can't be longer than 400 characters.");
            InternalReject(RejectionReason.Banned, reason, false, expiration);
        }

        public void RejectBanned(string reason, DateTime expiration)
        {
            if (reason != null && reason.Length > 400) throw new Exception("Reason can't be longer than 400 characters.");
            InternalReject(RejectionReason.Banned, reason, false, expiration.Ticks);
        }

		public void Reject(NetDataWriter writer)
		{
			if (!Allow) return;
			Allow = false;
			Request.Reject(writer);
		}
		
		public void RejectForce(RejectionReason reason) => InternalReject(reason, null, true);

		public void RejectForce(string reason)
		{
			if (reason != null && reason.Length > 400) throw new Exception("Reason can't be longer than 400 characters.");
			InternalReject(RejectionReason.Custom, reason, true);
		}

        public void RejectBannedForce(string reason)
        {
            if (reason != null && reason.Length > 400) throw new Exception("Reason can't be longer than 400 characters.");
            InternalReject(RejectionReason.Banned, reason, true);
        }

		public void RejectBannedForce(string reason, long expiration)
        {
            if (reason != null && reason.Length > 400) throw new Exception("Reason can't be longer than 400 characters.");
            InternalReject(RejectionReason.Banned, reason, true, expiration);
        }

        public void RejectBannedForce(string reason, DateTime expiration)
        {
            if (reason != null && reason.Length > 400) throw new Exception("Reason can't be longer than 400 characters.");
            InternalReject(RejectionReason.Banned, reason, true, expiration.Ticks);
        }

		public void RejectForce(NetDataWriter writer)
		{
			if (!Allow) return;
			Allow = false;
			Request.RejectForce(writer);
		}

		private void InternalReject(RejectionReason reason, string customReason = null, bool force = false, long expiration = 0)
		{
			if (!Allow) return;
			Allow = false;
			NetDataWriter rejectData = new NetDataWriter();
			rejectData.Put((byte)reason);

			switch (reason)
            {
                case RejectionReason.Banned:
                    rejectData.Put(expiration);
                    rejectData.Put(customReason);
					break;

                case RejectionReason.Custom:
                    rejectData.Put(customReason);
                    break;
            }

            if (force)
				Request.RejectForce(rejectData);
			else Request.Reject(rejectData);
		}
	}

	public class RACommandEvent : EventArgs
	{
		public string Command { get; set; }
		public CommandSender Sender { get; set; }
		public bool Allow { get; set; }
	}

	public class CheaterReportEvent : EventArgs
	{
		public string ReporterId { get; set; }
		public string ReportedId { get; set; }
		public string ReportedIp { get; set; }
		public string Report { get; set; }
		public int ServerId { get; set; }
		public bool Allow { get; set; }
	}

	public class DoorInteractionEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public Door Door { get; set; }
		public bool Allow { get; set; }
	}

	public class ElevatorInteractionEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Lift.Elevator Elevator { get; internal set; }
		public bool Allow { get; set; }
	}

	public class LockerInteractionEvent : EventArgs
	{
		public readonly ReferenceHub Player;
		public readonly Locker Locker;
		public readonly int LockerId;
		//Shoud be put as a properties and not in the constructor
		public LockerInteractionEvent(ReferenceHub player, Locker locker, int lockerId)
		{
			Player = player;
			Locker = locker;
			LockerId = lockerId;
		}
		public bool Allow { get; set; }
	}

	public class PlayerLeaveEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
	}

	public class WarheadLeverEvent : EventArgs
	{
		public bool Allow { get; set; }
		public ReferenceHub Player { get; set; }
	}

	public class ConsoleCommandEvent : EventArgs
	{
		public ConsoleCommandEvent(bool encrypted)
		{
			Encrypted = encrypted;
		}

		public ReferenceHub Player { get; set; }
		public string Command { get; set; }
		public string ReturnMessage { get; set; }
		public bool Encrypted { get; private set; }
		public string Color { get; set; }
	}

	public class DropItemEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public Inventory.SyncItemInfo Item { get; set; }
		public bool Allow { get; set; }
	}

	public class ItemDroppedEvent : EventArgs
	{
		public ReferenceHub Player;
		public Pickup Item;
		public Inventory.SyncItemInfo SyncItemInfo { get; set; }
	}

	public class PickupItemEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public Pickup Item { get; set; }
		public bool Allow { get; set; }
	}

	public class HandcuffEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public ReferenceHub Target { get; set; }
		public bool Allow { get; set; }
	}

	public class GeneratorUnlockEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public Generator079 Generator { get; set; }
		public bool Allow { get; set; }
	}
	public class GeneratorOpenEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public Generator079 Generator { get; set; }
		public bool Allow { get; set; }
	}

	public class GeneratorCloseEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public Generator079 Generator { get; set; }
		public bool Allow { get; set; }
	}

	public class GeneratorInsertTabletEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public Generator079 Generator { get; set; }
		public bool Allow { get; set; }
	}

	public class GeneratorEjectTabletEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public Generator079 Generator { get; set; }
		public bool Allow { get; set; }
	}

	public class GeneratorFinishEvent : EventArgs
	{
		public Generator079 Generator { get; set; }
	}

	public class Scp079TriggerTeslaEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public bool Allow { get; set; }
	}

	public class DecontaminationEvent : EventArgs
	{
		public bool Allow { get; set; }
	}

	public class CheckEscapeEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public bool Allow { get; set; }
	}

	public class IntercomSpeakEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
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
		public GameObject Target;
		public ReferenceHub Shooter;
		public float Damage;
		public bool Allow;
		public string HitboxType { get; internal set; }
		public float Distance;
	}

	public class ShootEvent : EventArgs
	{
		public ReferenceHub Shooter { get; set; }
		public GameObject Target { get; set; }
		public bool Allow { get; set; }
		public Vector3 TargetPos;
	}

	public class Scp106TeleportEvent : EventArgs
	{
		public ReferenceHub Player;
		public Vector3 PortalPosition;
		public bool Allow;
	}

	public class PocketDimDamageEvent : EventArgs
	{
		public ReferenceHub Player;
		public bool Allow;
	}

	public class PocketDimEscapedEvent : EventArgs
	{
		public ReferenceHub Player;
		public bool Allow;
	}

	public class PlayerBannedEvent : EventArgs
	{
		public BanDetails Details;
		public BanType Type;
	}

	public class PlayerBanEvent : EventArgs
	{
		private readonly bool log;
		private string userId;
		private int duration;
		private ReferenceHub bannedPlayer;
		private ReferenceHub issuer;
		private bool allow = true;

		public string Reason;
		public string FullMessage;

		public PlayerBanEvent(bool log, ReferenceHub bannedPlayer, string reason, string userId, int duration, ReferenceHub issuer)
		{
			this.log = log;
			this.userId = userId;
			this.duration = duration;
			this.bannedPlayer = bannedPlayer;
			this.issuer = issuer;
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

				if (log)
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
		public ReferenceHub Issuer
		{
			set
			{
				if (value == null || issuer == value) return;
				if (log)
					LogBanChange(Assembly.GetCallingAssembly().GetName().Name
								 + $" changed the ban issuer from user {issuer.nicknameSync.Network_myNickSync} ({issuer.characterClassManager.UserId}) to {value.nicknameSync.Network_myNickSync} ({value.characterClassManager.UserId})");
				issuer = value;
			}
			get => issuer;
		}
		private void LogBanChange(string msg)
		{
			string time = TimeBehaviour.FormatTime("yyyy-MM-dd HH:mm:ss.fff zzz");
			object lockObject = ServerLogs.LockObject;

			lock (lockObject)
			{
				ServerLogs.Queue.Enqueue(new ServerLogs.ServerLog(msg, "AntiBackdoor", "EXILED-Ban", time));
			}

			ServerLogs._write = true;
		}
	}

	public class PocketDimEnterEvent : EventArgs
	{
		public ReferenceHub Player;
		public bool Allow;
	}

	public class PocketDimDeathEvent : EventArgs
	{
		public ReferenceHub Player;
		public bool Allow;
	}

	public class PlayerReloadEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public bool Allow { get; set; }
		public bool AnimationOnly { get; internal set; }
	}

	public class PlayerSpawnEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public RoleType Role { get; set; }
		public Vector3 Spawnpoint { get; set; }
		public float RotationY { get; set; }
	}

	public class Scp106ContainEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public bool Allow { get; set; }
	}

	public class Scp914ActivationEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public bool Allow { get; set; }
		public double Time { get; set; }
	}

	public class Scp914KnobChangeEvent : EventArgs
	{
		public bool Allow { get; set; }
		public Scp914Knob KnobSetting { get; set; }
		public ReferenceHub Player { get; set; }
	}

	public class SetGroupEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public UserGroup Group { get; set; }
		public bool Allow { get; set; }
	}

	public class FemurEnterEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public bool Allow { get; set; }
	}

	public class SyncDataEvent : EventArgs
	{
		public ReferenceHub Player;
		public int State;
		public Vector2 v2;
		public bool Allow { get; set; }
	}

	public class WarheadKeycardAccessEvent : EventArgs
	{
		public ReferenceHub Player;
		public bool Allow;
		public string RequiredPerms;
	}

	public class Scp079ExpGainEvent : EventArgs
	{
		public ReferenceHub Player;
		public bool Allow;
		public ExpGainType GainType;
		public float Amount;
	}

	public class Scp079LvlGainEvent : EventArgs
	{
		public ReferenceHub Player;
		public bool Allow;
		public int OldLvl;
		public int NewLvl;
	}

	public class WarheadCancelEvent : EventArgs
	{
		public ReferenceHub Player;
		public bool Allow;
	}

	public class WarheadStartEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow;
	}

	public class ItemChangedEvent : EventArgs
	{
		public ReferenceHub Player;
		public Inventory.SyncItemInfo OldItem;
		public Inventory.SyncItemInfo NewItem;
	}

	public class Scp106CreatedPortalEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public bool Allow { get; set; }
		public Vector3 PortalPosition { get; set; }
	}

	public class AnnounceNtfEntranceEvent : EventArgs
	{
		public int ScpsLeft { get; set; }
		public int NtfNumber { get; set; }
		public char NtfLetter { get; set; }
		public bool Allow { get; set; }
	}

	public class AnnounceScpTerminationEvent : EventArgs
	{
		public ReferenceHub Killer { get; internal set; }
		public Role Role { get; internal set; }
		public PlayerStats.HitInfo HitInfo { get; set; }
		public string TerminationCause { get; set; }
		public bool Allow { get; set; }
	}

	public class AnnounceDecontaminationEvent : EventArgs
	{
		private int announcementId;

		public int AnnouncementId
		{
			get => announcementId;
			set => announcementId = Mathf.Clamp(value, 0, 5);
		}
		public bool IsAnnouncementGlobal { get; set; }
		public bool Allow { get; set; }
	}

	public class SpawnRagdollEvent : EventArgs
	{
		private int ragdollPlayerId;

		public ReferenceHub Killer { get; internal set; }
		public ReferenceHub Player { get; internal set; }
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
		public RoleType RoleType { get; set; }
		public PlayerStats.HitInfo HitInfo { get; set; }
		public bool AllowRecall { get; set; }
		public string RagdollDissonanceId { get; set; }
		public string RagdollPlayerName { get; set; }
		public int RagdollPlayerId
		{
			get => ragdollPlayerId;
			set
			{
				if (Extensions.Player.GetPlayer(value) == null)
					return;

				ragdollPlayerId = value;
			}
		}
		public bool Allow { get; set; }
	}

	public class PlayerInteractEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
	}

	public class PlaceBloodEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Vector3 Position { get; set; }
		public int BloodType { get; set; }
		public float Multiplier { get; set; }
		public bool Allow { get; set; }
	}

	public class UsedMedicalItemEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public ItemType ItemType { get; internal set; }
	}

	public class PlaceDecalEvent : EventArgs
	{
		public ReferenceHub Player { get; internal set; }
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
		public int Type { get; set; }
		public bool Allow { get; set; }
	}
}