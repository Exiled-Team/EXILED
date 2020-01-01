using System;
using System.Collections.Generic;
using MEC;

namespace EXILED.Patches
{
	public class EventHandlers
	{
		private readonly Plugin plugin;
		public EventHandlers(plugin plugin) => this.plugin = plugin;
		
		public void OnWaitingForPlayers()
		{
			EXILED.plugin.GhostedIds.Clear();
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
				EXILED.plugin.RoundTime = DateTime.Now;
				yield return Timing.WaitForSeconds(1f);
			}
		}
	}
}