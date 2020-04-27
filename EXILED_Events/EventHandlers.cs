using EXILED.Extensions;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EXILED.Patches
{
	public class EventHandlers
	{
		private readonly Plugin plugin;
		public EventHandlers(EventPlugin plugin) => this.plugin = plugin;

		public bool RoundStarted;

		public void OnWaitingForPlayers()
		{
			EventPlugin.GhostedIds.Clear();
			Map.Rooms.Clear();
			Map.Doors.Clear();
			Map.Lifts.Clear();
			Map.TeslaGates.Clear();
			Player.IdHubs.Clear();
			Player.StrHubs.Clear();
			Timing.RunCoroutine(ResetRoundTime(), "resetroundtime");
			EventPlugin.DeadPlayers.Clear();

			RoundStarted = false;
		}

		public void OnRoundStart()
		{
			Timing.KillCoroutines("resetroundtime");
			RoundStarted = true;

			foreach (ReferenceHub hub in Player.GetHubs())
			{
				if (hub.GetOverwatch())
				{
					hub.SetOverwatch(false);
					Timing.CallDelayed(2f, () => hub.SetOverwatch(true));
				}
			}
		}

		public IEnumerator<float> ResetRoundTime()
		{
			for (; ; )
			{
				EventPlugin.RoundTime = DateTime.Now;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public void OnPlayerLeave(EXILED.PlayerLeaveEvent ev)
		{
			if (Player.IdHubs.ContainsKey(ev.Player.GetPlayerId()))
				Player.IdHubs.Remove(ev.Player.GetPlayerId());

			foreach (KeyValuePair<string, ReferenceHub> entry in Player.StrHubs.Where(s => s.Value == ev.Player).ToList())
				Player.StrHubs.Remove(entry.Key);

			if (EventPlugin.DeadPlayers.Contains(ev.Player))
				EventPlugin.DeadPlayers.Remove(ev.Player);
		}

		public void OnPlayerDeath(ref PlayerDeathEvent ev)
		{
			if (ev.Player == null || ev.Player.IsHost() || string.IsNullOrEmpty(ev.Player.GetUserId()))
				return;

			if (!EventPlugin.DeadPlayers.Contains(ev.Player))
				EventPlugin.DeadPlayers.Add(ev.Player);
		}

		public void OnPlayerJoin(EXILED.PlayerJoinEvent ev)
		{
			if (ev.Player == null || ev.Player.IsHost() || string.IsNullOrEmpty(ev.Player.GetUserId()))
				return;

			if (!RoundStarted)
				return;

			if (!EventPlugin.DeadPlayers.Contains(ev.Player))
				EventPlugin.DeadPlayers.Add(ev.Player);
		}

		public void OnSetClass(EXILED.SetClassEvent ev)
		{
			if (ev.Player == null || ev.Player.IsHost() || string.IsNullOrEmpty(ev.Player.GetUserId()))
				return;

			if (ev.Role == RoleType.Spectator)
			{
				if (!EventPlugin.DeadPlayers.Contains(ev.Player))
					EventPlugin.DeadPlayers.Add(ev.Player);

				if (EventPlugin.DropInventory)
					ev.Player.inventory.ServerDropAll();
			}
			else
			{
				if (EventPlugin.DeadPlayers.Contains(ev.Player))
					EventPlugin.DeadPlayers.Remove(ev.Player);
			}
		}
	}
}
