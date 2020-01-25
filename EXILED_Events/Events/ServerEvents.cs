using System.Collections.Generic;
using LiteNetLib;
using UnityEngine;

namespace EXILED
{
	public partial class Events
	{
		public static event OnRoundStart RoundStartEvent;
		public delegate void OnRoundStart();
		public static void InvokeRoundStart()
		{
			OnRoundStart roundStartEvent = RoundStartEvent;
			roundStartEvent?.Invoke();
		}

		public static event OnPreAuth PreAuthEvent;
		public delegate void OnPreAuth(ref PreauthEvent ev);
		public static void InvokePreAuth(ref string userid, ConnectionRequest request, ref bool allow)
		{
			OnPreAuth preAuthEvent = PreAuthEvent;
			if (preAuthEvent == null)
				return;
			
			PreauthEvent ev = new PreauthEvent()
			{
				Allow = allow,
				Request = request,
				UserId = userid
			};
			preAuthEvent?.Invoke(ref ev);
			allow = ev.Allow;
			userid = ev.UserId;
		}

		public static event OnRoundEnd RoundEndEvent;
		public delegate void OnRoundEnd();
		public static void InvokeRoundEnd()
		{
			OnRoundEnd roundEndEvent = RoundEndEvent;
			roundEndEvent?.Invoke();
		}

		public static event OnRoundRestart RoundRestartEvent;
		public delegate void OnRoundRestart();
		public static void InvokeRoundRestart()
		{
			OnRoundRestart onRoundRestart = RoundRestartEvent;
			onRoundRestart?.Invoke();
		}
		
		public static event OnCommand RemoteAdminCommandEvent;
		public delegate void OnCommand(ref RACommandEvent ev);
		public static void InvokeCommand(ref string query, ref CommandSender sender, ref bool allow)
		{
			OnCommand adminCommandEvent = RemoteAdminCommandEvent;
			if (adminCommandEvent == null)
				return;
			
			RACommandEvent ev = new RACommandEvent()
			{
				Allow = allow,
				Command = query,
				Sender = sender
			};
			adminCommandEvent?.Invoke(ref ev);
			query = ev.Command;
			sender = ev.Sender;
			allow = ev.Allow;
		}
		
		public static event OnCheaterReport CheaterReportEvent;
		public delegate void OnCheaterReport(ref CheaterReportEvent ev);

		public static void InvokeCheaterReport(string reporterId, string reportedId, string reportedIp, string reason, int serverId, ref bool allow)
		{
			OnCheaterReport onCheaterReport = CheaterReportEvent;
			if (onCheaterReport == null)
				return;
			
			CheaterReportEvent ev = new CheaterReportEvent()
			{
				Allow = allow,
				Report = reason,
				ReportedId = reportedId,
				ReportedIp = reportedIp,
				ReporterId = reporterId
			};
			onCheaterReport?.Invoke(ref ev);
			allow = ev.Allow;
		}
		
		public static event WaitingForPlayers WaitingForPlayersEvent;
		public delegate void WaitingForPlayers();
		public static void InvokeWaitingForPlayers()
		{
			WaitingForPlayers waitingForPlayers = WaitingForPlayersEvent;
			waitingForPlayers?.Invoke();
		}
		
		public static event TeamRespawn TeamRespawnEvent;
		public delegate void TeamRespawn(ref TeamRespawnEvent ev);

		public static void InvokeTeamRespawn(ref bool isChaos, ref int maxRespawn, ref List<GameObject> toRespawn)
		{
			TeamRespawn teamRespawn = TeamRespawnEvent;
			if (teamRespawn == null)
				return;
			
			List<ReferenceHub> respawn = new List<ReferenceHub>();
			foreach (GameObject obj in toRespawn)
				respawn.Add(Plugin.GetPlayer(obj));
			TeamRespawnEvent ev = new TeamRespawnEvent()
			{
				IsChaos = isChaos,
				MaxRespawnAmt = maxRespawn,
				ToRespawn = respawn
			};
			teamRespawn?.Invoke(ref ev);
			maxRespawn = ev.MaxRespawnAmt;
			toRespawn = new List<GameObject>();
			foreach (ReferenceHub hub in ev.ToRespawn)
				toRespawn.Add(hub.gameObject);
		}
	}
}