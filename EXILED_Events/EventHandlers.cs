using System;
using System.Collections.Generic;
using System.Linq;
using EXILED.Extensions;
using Grenades;
using MEC;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EXILED.Patches
{
	public class EventHandlers
	{
		private readonly Plugin plugin;
		public EventHandlers(EventPlugin plugin) => this.plugin = plugin;
		
		public void OnWaitingForPlayers()
		{
			EventPlugin.GhostedIds.Clear();
			Plugin.IdHubs.Clear();
			Plugin.StrHubs.Clear();
			Timing.RunCoroutine(ResetRoundTime(), "resetroundtime");
		}

		public void OnRoundStart()
		{
			Timing.KillCoroutines("resetroundtime");
		}

		public IEnumerator<float> ResetRoundTime()
		{
			for (;;)
			{
				EventPlugin.RoundTime = DateTime.Now;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public void OnSetClass(EXILED.SetClassEvent ev)
		{
			Timing.RunCoroutine(InvokeSpawn(ev.Player, ev.Role));
		}

		public IEnumerator<float> InvokeSpawn(ReferenceHub hub, RoleType role)
		{
			yield return Timing.WaitForSeconds(0.05f);
			
			Events.InvokePlayerSpawn(hub, role);
		}

		public void OnPlayerLeave(EXILED.PlayerLeaveEvent ev)
		{
			if (Plugin.IdHubs.ContainsKey(ev.Player.queryProcessor.PlayerId))
				Plugin.IdHubs.Remove(ev.Player.queryProcessor.PlayerId);

			if (Plugin.StrHubs.ContainsValue(ev.Player))
				Plugin.StrHubs.Remove(Plugin.StrHubs.FirstOrDefault(s => s.Value == ev.Player).Key);
		}
	}
}