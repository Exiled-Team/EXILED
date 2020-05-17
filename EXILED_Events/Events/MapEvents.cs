using EXILED.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXILED
{
	public partial class Events
	{
		[Obsolete("Use WarheadCancelEvent instead.", true)]
		public delegate void OnWarheadCommand(ref WarheadLeverEvent ev);

		public static event PlaceDecal PlaceDecalEvent;
		public delegate void PlaceDecal(PlaceDecalEvent ev);

		public static void InvokePlaceDecal(GameObject player, ref Vector3 position, ref Quaternion rotation, ref int type, ref bool allow)
		{
			if (PlaceDecalEvent == null)
				return;

			PlaceDecalEvent ev = new PlaceDecalEvent()
			{
				Player = player.GetPlayer(),
				Position = position,
				Rotation = rotation,
				Type = type,
				Allow = allow
			};

			PlaceDecalEvent.InvokeSafely(ev);

			position = ev.Position;
			rotation = ev.Rotation;
			type = ev.Type;
			allow = ev.Allow;
		}

		public static event PlaceBlood PlaceBloodEvent;
		public delegate void PlaceBlood(PlaceBloodEvent ev);

		public static void InvokePlaceBlood(GameObject player, ref Vector3 position, ref int bloodType, ref float multiplier, ref bool allow)
		{
			if (PlaceBloodEvent == null)
				return;

			PlaceBloodEvent ev = new PlaceBloodEvent()
			{
				Player = player.GetPlayer(),
				Position = position,
				BloodType = bloodType,
				Multiplier = multiplier,
				Allow = allow
			};

			PlaceBloodEvent.InvokeSafely(ev);

			position = ev.Position;
			bloodType = ev.BloodType;
			multiplier = ev.Multiplier;
			allow = ev.Allow;
		}

		public static event AnnounceDecontamination AnnounceDecontaminationEvent;
		public delegate void AnnounceDecontamination(AnnounceDecontaminationEvent ev);

		public static void InvokeAnnounceDecontamination(ref int announcementId, ref bool isAnnouncementGlobal, ref bool allow)
		{
			if (AnnounceDecontaminationEvent == null)
				return;

			AnnounceDecontaminationEvent ev = new AnnounceDecontaminationEvent()
			{
				AnnouncementId = announcementId,
				IsAnnouncementGlobal = isAnnouncementGlobal,
				Allow = allow
			};

			AnnounceDecontaminationEvent.InvokeSafely(ev);

			announcementId = ev.AnnouncementId;
			isAnnouncementGlobal = ev.IsAnnouncementGlobal;
			allow = ev.Allow;
		}

		public static event AnnounceScpTermination AnnounceScpTerminationEvent;
		public delegate void AnnounceScpTermination(AnnounceScpTerminationEvent ev);

		public static void InvokeAnnounceScpTermination(Role role, ref PlayerStats.HitInfo hitInfo, ref string terminationCause, ref bool allow)
		{
			if (AnnounceScpTerminationEvent == null)
				return;

			AnnounceScpTerminationEvent ev = new AnnounceScpTerminationEvent()
			{
				Killer = hitInfo.PlyId == 0 ? null : Player.GetPlayer(hitInfo.PlyId),
				Role = role,
				HitInfo = hitInfo,
				TerminationCause = terminationCause,
				Allow = allow
			};

			AnnounceScpTerminationEvent.InvokeSafely(ev);

			hitInfo = ev.HitInfo;
			terminationCause = ev.TerminationCause;
			allow = ev.Allow;
		}

		public static event AnnounceNtfEntrance AnnounceNtfEntranceEvent;
		public delegate void AnnounceNtfEntrance(AnnounceNtfEntranceEvent ev);

		public static void InvokeAnnounceNtfEntrance(ref int scpsLeft, ref int ntfNumber, ref char ntfLetter, ref bool allow)
		{
			if (AnnounceNtfEntranceEvent == null)
				return;

			AnnounceNtfEntranceEvent ev = new AnnounceNtfEntranceEvent()
			{
				ScpsLeft = scpsLeft,
				NtfNumber = ntfNumber,
				NtfLetter = ntfLetter,
				Allow = allow
			};

			AnnounceNtfEntranceEvent.InvokeSafely(ev);

			scpsLeft = ev.ScpsLeft;
			ntfNumber = ev.NtfNumber;
			ntfLetter = ev.NtfLetter;
			allow = ev.Allow;
		}

		public static event OnWarheadDetonation WarheadDetonationEvent;
		public delegate void OnWarheadDetonation();

		public static void InvokeWarheadDetonation() => WarheadDetonationEvent.InvokeSafely();

		public static event OnDoorInteract DoorInteractEvent;
		public delegate void OnDoorInteract(ref DoorInteractionEvent ev);

		public static void InvokeDoorInteract(GameObject player, Door door, ref bool allow)
		{
			if (DoorInteractEvent == null)
				return;

			DoorInteractionEvent ev = new DoorInteractionEvent()
			{
				Player = player.GetPlayer(),
				Allow = allow,
				Door = door
			};

			DoorInteractEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event OnElevatorInteract ElevatorInteractEvent;
		public delegate void OnElevatorInteract(ref ElevatorInteractionEvent ev);

		public static void InvokeElevatorInteract(GameObject player, Lift.Elevator elevator, ref bool allow)
		{
			if (ElevatorInteractEvent == null)
				return;

			ElevatorInteractionEvent ev = new ElevatorInteractionEvent()
			{
				Player = player.GetPlayer(),
				Elevator = elevator,
				Allow = allow
			};

			ElevatorInteractEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event WarheadCancelled WarheadCancelledEvent;
		public delegate void WarheadCancelled(WarheadCancelEvent ev);

		public static void InvokeWarheadCancel(GameObject player, ref bool allow)
		{
			if (WarheadCancelledEvent == null)
				return;

			WarheadCancelEvent ev = new WarheadCancelEvent
			{
				Allow = allow,
				Player = player ? player.GetPlayer() : null
			};

			WarheadCancelledEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event WarheadStart WarheadStartEvent;
		public delegate void WarheadStart(WarheadStartEvent ev);

		public static void InvokeWarheadStart(GameObject player, ref bool allow)
		{
			if (WarheadStartEvent == null)
				return;

			WarheadStartEvent ev = new WarheadStartEvent
			{
				Player = player == null ? null : player.GetPlayer(),
				Allow = allow
			};

			WarheadStartEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event OnLockerInteract LockerInteractEvent;
		public delegate void OnLockerInteract(LockerInteractionEvent ev);

		public static void InvokeLockerInteract(GameObject player, Locker locker, int lockerId, ref bool allow)
		{
			if (LockerInteractEvent == null)
				return;

			LockerInteractionEvent ev = new LockerInteractionEvent(player.GetPlayer(), locker, lockerId)
			{
				Allow = allow,
			};

			LockerInteractEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event TriggerTesla TriggerTeslaEvent;
		public delegate void TriggerTesla(ref TriggerTeslaEvent ev);

		public static void InvokeTriggerTesla(GameObject player, bool isInHurtingRange, ref bool isTriggerable)
		{
			if (TriggerTeslaEvent == null)
				return;

			TriggerTeslaEvent ev = new TriggerTeslaEvent()
			{
				Player = player.GetPlayer(),
				IsInHurtingRange = isInHurtingRange,
				Triggerable = isTriggerable
			};

			TriggerTeslaEvent.InvokeSafely(ev);

			isTriggerable = ev.Triggerable;
		}

		public static event Scp914Upgrade Scp914UpgradeEvent;
		public delegate void Scp914Upgrade(ref SCP914UpgradeEvent ev);

		public static void InvokeScp914Upgrade(Scp914.Scp914Machine machine, List<CharacterClassManager> characterClassManagers, ref List<Pickup> pickups, Scp914.Scp914Knob knobSetting, ref bool allow)
		{
			if (Scp914UpgradeEvent == null)
				return;

			List<ReferenceHub> players = new List<ReferenceHub>();

			foreach (CharacterClassManager characterClassManager in characterClassManagers)
				players.Add(characterClassManager.gameObject.GetPlayer());

			SCP914UpgradeEvent ev = new SCP914UpgradeEvent()
			{
				Allow = allow,
				Machine = machine,
				Players = players,
				Items = pickups,
				KnobSetting = knobSetting
			};

			Scp914UpgradeEvent.InvokeSafely(ev);

			pickups = ev.Items;
			allow = ev.Allow;
		}

		public static event GeneratorUnlock GeneratorUnlockEvent;
		public delegate void GeneratorUnlock(ref GeneratorUnlockEvent ev);

		public static void InvokeGeneratorUnlock(GameObject player, Generator079 generator, ref bool allow)
		{
			if (GeneratorUnlockEvent == null)
				return;

			GeneratorUnlockEvent ev = new GeneratorUnlockEvent()
			{
				Player = player.GetPlayer(),
				Generator = generator,
				Allow = allow
			};

			GeneratorUnlockEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event GeneratorOpen GeneratorOpenedEvent;
		public delegate void GeneratorOpen(ref GeneratorOpenEvent ev);

		public static void InvokeGeneratorOpen(GameObject player, Generator079 generator, ref bool allow)
		{
			if (GeneratorOpenedEvent == null)
				return;

			GeneratorOpenEvent ev = new GeneratorOpenEvent()
			{
				Player = player.GetPlayer(),
				Generator = generator,
				Allow = allow
			};

			GeneratorOpenedEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event GeneratorClose GeneratorClosedEvent;
		public delegate void GeneratorClose(ref GeneratorCloseEvent ev);

		public static void InvokeGeneratorClose(GameObject player, Generator079 generator, ref bool allow)
		{
			if (GeneratorClosedEvent == null)
				return;

			GeneratorCloseEvent ev = new GeneratorCloseEvent()
			{
				Player = player.GetPlayer(),
				Generator = generator,
				Allow = allow
			};

			GeneratorClosedEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event GeneratorInsert GeneratorInsertedEvent;
		public delegate void GeneratorInsert(ref GeneratorInsertTabletEvent ev);

		public static void InvokeGeneratorInsert(GameObject player, Generator079 generator, ref bool allow)
		{
			if (GeneratorInsertedEvent == null)
				return;

			GeneratorInsertTabletEvent ev = new GeneratorInsertTabletEvent()
			{
				Player = player.GetPlayer(),
				Generator = generator,
				Allow = allow
			};

			GeneratorInsertedEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event GeneratorEject GeneratorEjectedEvent;
		public delegate void GeneratorEject(ref GeneratorEjectTabletEvent ev);

		public static void InvokeGeneratorEject(GameObject player, Generator079 generator, ref bool allow)
		{
			if (GeneratorEjectedEvent == null)
				return;

			GeneratorEjectTabletEvent ev = new GeneratorEjectTabletEvent()
			{
				Player = player.GetPlayer(),
				Generator = generator,
				Allow = allow
			};

			GeneratorEjectedEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event GeneratorFinish GeneratorFinishedEvent;
		public delegate void GeneratorFinish(ref GeneratorFinishEvent ev);

		public static void InvokeGeneratorFinish(Generator079 generator)
		{
			if (GeneratorFinishedEvent == null)
				return;

			GeneratorFinishEvent ev = new GeneratorFinishEvent()
			{
				Generator = generator,
			};

			GeneratorFinishedEvent.InvokeSafely(ev);
		}

		public static event Decontamination DecontaminationEvent;
		public delegate void Decontamination(ref DecontaminationEvent ev);

		public static void InvokeDecontamination(ref bool allow)
		{
			if (DecontaminationEvent == null)
				return;

			DecontaminationEvent ev = new DecontaminationEvent
			{
				Allow = allow
			};

			DecontaminationEvent.InvokeSafely(ev);

			allow = ev.Allow;
		}

		public static event CheckRoundEnd CheckRoundEndEvent;
		public delegate void CheckRoundEnd(ref CheckRoundEndEvent ev);

		public static void InvokeCheckRoundEnd(ref bool forceEnd, ref bool allow, ref RoundSummary.LeadingTeam leadingTeam, ref bool teamChanged)
		{
			if (CheckRoundEndEvent == null)
				return;

			CheckRoundEndEvent ev = new CheckRoundEndEvent
			{
				LeadingTeam = leadingTeam,
				ForceEnd = forceEnd,
				Allow = allow
			};

			CheckRoundEndEvent.InvokeSafely(ev);

			teamChanged = leadingTeam != ev.LeadingTeam;
			leadingTeam = ev.LeadingTeam;
			allow = ev.Allow;
			forceEnd = ev.ForceEnd;
		}
	}
}
