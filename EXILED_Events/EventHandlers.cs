using System;
using System.Collections.Generic;
using System.Linq;
using EXILED.Extensions;
using MEC;

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
		}

		public IEnumerator<float> ResetRoundTime()
		{
			for (;;)
			{
				EventPlugin.RoundTime = DateTime.Now;
				yield return Timing.WaitForSeconds(1f);
			}
		}

		public void OnPlayerLeave(EXILED.PlayerLeaveEvent ev)
		{
			if (Player.IdHubs.ContainsKey(ev.Player.queryProcessor.PlayerId))
				Player.IdHubs.Remove(ev.Player.queryProcessor.PlayerId);

			foreach (KeyValuePair<string, ReferenceHub> entry in Player.StrHubs.Where(s => s.Value == ev.Player).ToList())
				Player.StrHubs.Remove(entry.Key);

			if (EventPlugin.DeadPlayers.Contains(ev.Player.gameObject))
				EventPlugin.DeadPlayers.Remove(ev.Player.gameObject);
		}

		public void OnPlayerDeath(ref PlayerDeathEvent ev)
		{
			if (ev.Player == null || ev.Player.characterClassManager.IsHost ||
			    string.IsNullOrEmpty(ev.Player.characterClassManager.UserId))
				return;
			
			if (!EventPlugin.DeadPlayers.Contains(ev.Player.gameObject))
				EventPlugin.DeadPlayers.Add(ev.Player.gameObject);
		}

		public void OnPlayerJoin(EXILED.PlayerJoinEvent ev)
		{
			if (ev.Player == null || ev.Player.characterClassManager.IsHost ||
			    string.IsNullOrEmpty(ev.Player.characterClassManager.UserId) || !RoundStarted)
				return;
			
			if (!EventPlugin.DeadPlayers.Contains(ev.Player.gameObject)) 
				EventPlugin.DeadPlayers.Add(ev.Player.gameObject);
		}

        public void OnSetClass(EXILED.SetClassEvent ev)
        {
            if (ev.Role != RoleType.Spectator)
            {
                if (EventPlugin.DeadPlayers.Contains(ev.Player.gameObject))
                    EventPlugin.DeadPlayers.Remove(ev.Player.gameObject);
            }
        }
    }
}