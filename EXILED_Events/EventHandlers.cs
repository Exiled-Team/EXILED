using System;
using System.Collections.Generic;
using MEC;

namespace EXILED.Patches
{
	public class EventHandlers
	{
		private readonly Plugin plugin;
		public EventHandlers(EventPlugin plugin) => this.plugin = plugin;
		
		public void OnWaitingForPlayers()
		{
			EventPlugin.GhostedIds.Clear();
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
	}
}