using System;
using System.Collections.Generic;
using Grenades;
using LiteNetLib;
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

	public class PlayerHurtEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public ReferenceHub Attacker { get; set; }
		public PlayerStats.HitInfo Info { get; set; }
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
		public string UserId { get; set; }
		public ConnectionRequest Request { get; set; }
		public bool Allow { get; set; }
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
  
  	public class OnShootEvent : EventArgs
	{
		public GameObject Target;
		public ReferenceHub Shooter;
		public float Damage;
		public bool Allow;
		public float Distance;
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
	}

	public class PlayerSpawnEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public RoleType Role { get; set; }
	}

	public class Scp106ContainEvent : EventArgs
	{
		public ReferenceHub Player { get; set; }
		public bool Allow { get; set; }
	}
}