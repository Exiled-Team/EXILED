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
		public static void InvokeWarheadDetonation()
		{
			OnWarheadDetonation warheadDetonationEvent = WarheadDetonationEvent;
			warheadDetonationEvent?.Invoke();
		}

		public static event OnDoorInteract DoorInteractEvent;
		public delegate void OnDoorInteract(ref DoorInteractionEvent ev);

		public static void InvokeDoorInteract(GameObject player, Door door, ref bool allow)
		{
			OnDoorInteract onDoorInteract = DoorInteractEvent;
			if (onDoorInteract == null)
				return;
			
			DoorInteractionEvent ev = new DoorInteractionEvent()
			{
				Player = player.GetPlayer(),
				Allow = allow,
				Door = door
			};
			onDoorInteract?.Invoke(ref ev);
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
			OnLockerInteract onLockerInteract = LockerInteractEvent;
			if (onLockerInteract == null)
				return;
			LockerInteractionEvent ev = new LockerInteractionEvent(gameObject.GetPlayer(), locker, lockerid)
			{
				Allow = allow,
			};
			onLockerInteract?.Invoke(ev);
			allow = ev.Allow;
		}

		public static event TriggerTesla TriggerTeslaEvent;
		public delegate void TriggerTesla(ref TriggerTeslaEvent ev);
		public static void InvokeTriggerTesla(GameObject obj, bool hurtRange, ref bool triggerable)
		{
			TriggerTesla triggerTesla = TriggerTeslaEvent;
			if (triggerTesla == null)
				return;
			
			TriggerTeslaEvent ev = new TriggerTeslaEvent()
			{
				Player = obj.GetPlayer(),
				Triggerable = triggerable
			};
			triggerTesla?.Invoke(ref ev);
			triggerable = ev.Triggerable;
		}
		
		public static event Scp914Upgrade Scp914UpgradeEvent;
		public delegate void Scp914Upgrade(ref SCP914UpgradeEvent ev);
		public static void InvokeScp914Upgrade(Scp914.Scp914Machine machine, List<CharacterClassManager> ccms, ref List<Pickup> pickups, Scp914.Scp914Knob knobSetting, ref bool allow)
		{
			Scp914Upgrade activated = Scp914UpgradeEvent;
			if (activated == null)
				return;
			List<ReferenceHub> players = new List<ReferenceHub>();
			foreach (CharacterClassManager ccm in ccms)
			{
				players.Add(ccm.gameObject.GetPlayer());
			}

			SCP914UpgradeEvent ev = new SCP914UpgradeEvent()
			{
				Allow = allow,
				Machine = machine,
				Players = players,
				Items = pickups,
				KnobSetting = knobSetting
			};
			activated?.Invoke(ref ev);
			pickups = ev.Items;
			allow = ev.Allow;
		}

		public static event GeneratorUnlock GeneratorUnlockEvent;
		public delegate void GeneratorUnlock(ref GeneratorUnlockEvent ev);
		internal static void InvokeGeneratorUnlock(GameObject person, Generator079 generator, ref bool allow)
		{
			GeneratorUnlock generatorUnlock = GeneratorUnlockEvent;
			if (generatorUnlock == null)
				return;
			GeneratorUnlockEvent ev = new GeneratorUnlockEvent()
			{
				Allow = allow,
				Generator = generator,
				Player = person.GetPlayer()
			};
			generatorUnlock.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event GeneratorOpen GeneratorOpenedEvent;
		public delegate void GeneratorOpen(ref GeneratorOpenEvent ev);
		public static void InvokeGeneratorOpen(GameObject player, Generator079 generator, ref bool allow)
		{
			GeneratorOpen generatorOpen = GeneratorOpenedEvent;
			if (generatorOpen == null)
				return;
			GeneratorOpenEvent ev = new GeneratorOpenEvent()
			{
				Player = player.GetPlayer(),
				Generator = generator,
				Allow = allow
			};
			generatorOpen.Invoke(ref ev);
			allow = ev.Allow;
		}
		
		public static event GeneratorClose GeneratorClosedEvent;
		public delegate void GeneratorClose(ref GeneratorCloseEvent ev);
		public static void InvokeGeneratorClose(GameObject player, Generator079 generator, ref bool allow)
		{
			GeneratorClose generatorClose = GeneratorClosedEvent;
			if (generatorClose == null)
				return;
			GeneratorCloseEvent ev = new GeneratorCloseEvent()
			{
				Player = player.GetPlayer(),
				Generator = generator,
				Allow = allow
			};
			generatorClose.Invoke(ref ev);
			allow = ev.Allow;
		}
		
		public static event GeneratorInsert GeneratorInsertedEvent;
		public delegate void GeneratorInsert(ref GeneratorInsertTabletEvent ev);
		public static void InvokeGeneratorInsert(GameObject player, Generator079 generator, ref bool allow)
		{
			GeneratorInsert generatorInsert = GeneratorInsertedEvent;
			if (generatorInsert == null)
				return;
			GeneratorInsertTabletEvent ev = new GeneratorInsertTabletEvent()
			{
				Player = player.GetPlayer(),
				Generator = generator,
				Allow = allow
			};
			generatorInsert.Invoke(ref ev);
			allow = ev.Allow;
		}
		
		public static event GeneratorEject GeneratorEjectedEvent;
		public delegate void GeneratorEject(ref GeneratorEjectTabletEvent ev);
		public static void InvokeGeneratorEject(GameObject player, Generator079 generator, ref bool allow)
		{
			GeneratorEject generatorEject = GeneratorEjectedEvent;
			if (generatorEject == null)
				return;
			GeneratorEjectTabletEvent ev = new GeneratorEjectTabletEvent()
			{
				Player = player.GetPlayer(),
				Generator = generator,
				Allow = allow
			};
			generatorEject.Invoke(ref ev);
			allow = ev.Allow;
		}
		
		public static event GeneratorFinish GeneratorFinishedEvent;
		public delegate void GeneratorFinish(ref GeneratorFinishEvent ev);
		public static void InvokeGeneratorFinish(Generator079 generator)
		{
			GeneratorFinish generatorFinish = GeneratorFinishedEvent;
			if (generatorFinish == null)
				return;
			GeneratorFinishEvent ev = new GeneratorFinishEvent()
			{
				Generator = generator,
			};
			generatorFinish.Invoke(ref ev);
		}

		public static event Decontamination DecontaminationEvent;

		public delegate void Decontamination(ref DecontaminationEvent ev);

		public static void InvokeDecontamination(ref bool allow)
		{
			Decontamination decontamination = DecontaminationEvent;
			if (decontamination == null)
				return;
			DecontaminationEvent ev = new DecontaminationEvent
			{
				Allow = allow
			};
			
			decontamination.Invoke(ref ev);
			allow = ev.Allow;
		}

		public static event CheckRoundEnd CheckRoundEndEvent;

		public delegate void CheckRoundEnd(ref CheckRoundEndEvent ev);

		public static void InvokeCheckRoundEnd(ref bool force, ref bool allow, ref RoundSummary.LeadingTeam team, ref bool teamChanged)
		{
			CheckRoundEnd checkRoundEnd = CheckRoundEndEvent;
			if (checkRoundEnd == null)
				return;
			
			CheckRoundEndEvent ev = new CheckRoundEndEvent
			{
				LeadingTeam = team,
				ForceEnd = force,
				Allow = allow
			};
			
			checkRoundEnd.Invoke(ref ev);
			if (team != ev.LeadingTeam)
				teamChanged = true;
			team = ev.LeadingTeam;
			allow = ev.Allow;
			force = ev.ForceEnd;
		}
	}
}