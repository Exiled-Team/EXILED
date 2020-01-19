using System;
using System.Collections.Generic;
using Grenades;
using LiteNetLib;
using UnityEngine;

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
}