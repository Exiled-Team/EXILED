using System;
using System.Collections.Generic;
using EXILED.Extensions;
using EXILED.Patches;
using UnityEngine;

namespace EXILED
{
	public partial class Events
	{
		[Obsolete("Use WarheadCancelEvent instead.", true)]
		public delegate void OnWarheadCommand(ref WarheadLeverEvent ev);

		public static event OnWarheadDetonation WarheadDetonationEvent;
		public delegate void OnWarheadDetonation();

		public static void InvokeWarheadDetonation() => WarheadDetonationEvent?.Invoke();

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

			DoorInteractEvent.Invoke(ref ev);

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

			ElevatorInteractEvent.Invoke(ref ev);

			allow = ev.Allow;
		}

		public static event WarheadCancelled WarheadCancelledEvent;
		public delegate void WarheadCancelled(WarheadCancelEvent ev);

		public static void InvokeWarheadCancel(GameObject obj, ref bool allow)
		{
			if (WarheadCancelledEvent == null)
				return;
			
			WarheadCancelEvent ev = new WarheadCancelEvent
			{
				Allow = allow,
				Player = obj ? obj.GetPlayer() : null
			};

			WarheadCancelledEvent.Invoke(ev);

			allow = ev.Allow;
		}

		public static event WarheadStart WarheadStartEvent;
		public delegate void WarheadStart(WarheadStartEvent ev);

		public static void InvokeWarheadStart(ref bool allow)
		{
			if (WarheadStartEvent == null)
				return;
			
			WarheadStartEvent ev = new WarheadStartEvent
			{
				Allow = allow
			};
			
			WarheadStartEvent.Invoke(ev);

			allow = ev.Allow;
		}

		public static event OnLockerInteract LockerInteractEvent;
		public delegate void OnLockerInteract(LockerInteractionEvent ev);

		internal static void InvokeLockerInteract(GameObject gameObject, Locker locker, int lockerid, ref bool allow)
		{
			if (LockerInteractEvent == null)
				return;

			LockerInteractionEvent ev = new LockerInteractionEvent(gameObject.GetPlayer(), locker, lockerid)
			{
				Allow = allow,
			};

			LockerInteractEvent.Invoke(ev);

			allow = ev.Allow;
		}

		public static event TriggerTesla TriggerTeslaEvent;
		public delegate void TriggerTesla(ref TriggerTeslaEvent ev);

		public static void InvokeTriggerTesla(GameObject obj, bool hurtRange, ref bool triggerable)
		{
			if (TriggerTeslaEvent == null)
				return;
			
			TriggerTeslaEvent ev = new TriggerTeslaEvent()
			{
				Player = obj.GetPlayer(),
				Triggerable = triggerable
			};

			TriggerTeslaEvent.Invoke(ref ev);

			triggerable = ev.Triggerable;
		}
		
		public static event Scp914Upgrade Scp914UpgradeEvent;
		public delegate void Scp914Upgrade(ref SCP914UpgradeEvent ev);

		public static void InvokeScp914Upgrade(Scp914.Scp914Machine machine, List<CharacterClassManager> ccms, ref List<Pickup> pickups, Scp914.Scp914Knob knobSetting, ref bool allow)
		{
			if (Scp914UpgradeEvent == null)
				return;

			List<ReferenceHub> players = new List<ReferenceHub>();

			foreach (CharacterClassManager ccm in ccms)
				players.Add(ccm.gameObject.GetPlayer());

			SCP914UpgradeEvent ev = new SCP914UpgradeEvent()
			{
				Allow = allow,
				Machine = machine,
				Players = players,
				Items = pickups,
				KnobSetting = knobSetting
			};
			
			Scp914UpgradeEvent.Invoke(ref ev);

			pickups = ev.Items;
			allow = ev.Allow;
		}

		public static event GeneratorUnlock GeneratorUnlockEvent;
		public delegate void GeneratorUnlock(ref GeneratorUnlockEvent ev);

		internal static void InvokeGeneratorUnlock(GameObject person, Generator079 generator, ref bool allow)
		{
			if (GeneratorUnlockEvent == null)
				return;

			GeneratorUnlockEvent ev = new GeneratorUnlockEvent()
			{
                Player = person.GetPlayer(),
                Generator = generator,
                Allow = allow				
			};

			GeneratorUnlockEvent.Invoke(ref ev);

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

			GeneratorOpenedEvent.Invoke(ref ev);

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

			GeneratorClosedEvent.Invoke(ref ev);

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

			GeneratorInsertedEvent.Invoke(ref ev);

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

			GeneratorEjectedEvent.Invoke(ref ev);

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

			GeneratorFinishedEvent.Invoke(ref ev);
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
			
			DecontaminationEvent.Invoke(ref ev);

			allow = ev.Allow;
		}

		public static event CheckRoundEnd CheckRoundEndEvent;
		public delegate void CheckRoundEnd(ref CheckRoundEndEvent ev);

		public static void InvokeCheckRoundEnd(ref bool force, ref bool allow, ref RoundSummary.LeadingTeam team, ref bool teamChanged)
		{
			if (CheckRoundEndEvent == null)
				return;
			
			CheckRoundEndEvent ev = new CheckRoundEndEvent
			{
				LeadingTeam = team,
				ForceEnd = force,
				Allow = allow
			};
			
			CheckRoundEndEvent.Invoke(ref ev);

			teamChanged = team != ev.LeadingTeam;
			team = ev.LeadingTeam;
			allow = ev.Allow;
			force = ev.ForceEnd;
		}
	}
}