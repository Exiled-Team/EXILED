using EXILED.Extensions;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static BanHandler;

namespace EXILED
{
	public partial class Events
	{
		public static event OnRoundStart RoundStartEvent;
		public delegate void OnRoundStart();

		public static void InvokeRoundStart() => RoundStartEvent?.Invoke();

		public static event OnPreAuth PreAuthEvent;
		public delegate void OnPreAuth(ref PreauthEvent ev);

		public static void InvokePreAuth(ref string userId, ConnectionRequest request, ref bool allow)
		{
			if (PreAuthEvent == null)
				return;

			PreauthEvent ev = new PreauthEvent()
			{
				Allow = allow,
				Request = request,
				UserId = userId
			};

			PreAuthEvent.Invoke(ref ev);

			allow = ev.Allow;
			userId = ev.UserId;
		}

		public static event OnRoundEnd RoundEndEvent;
		public delegate void OnRoundEnd();

		public static void InvokeRoundEnd() => RoundEndEvent?.Invoke();

		public static event OnRoundRestart RoundRestartEvent;
		public delegate void OnRoundRestart();

		public static void InvokeRoundRestart() => RoundRestartEvent?.Invoke();

		public static event OnCommand RemoteAdminCommandEvent;
		public delegate void OnCommand(ref RACommandEvent ev);

		public static void InvokeCommand(ref string query, ref CommandSender sender, ref bool allow)
		{
			if (RemoteAdminCommandEvent == null)
				return;

			RACommandEvent ev = new RACommandEvent()
			{
				Allow = allow,
				Command = query,
				Sender = sender
			};

			RemoteAdminCommandEvent.Invoke(ref ev);

			query = ev.Command;
			sender = ev.Sender;
			allow = ev.Allow;

			string fileName = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED"), $"{ServerConsole.Port}-RA_log.txt");

			if (!File.Exists(fileName))
				File.Create(fileName).Close();

			File.AppendAllText(fileName, $"{sender.Nickname} ({sender.SenderId}) ran command: {query}. Command Permitted: {allow}" + Environment.NewLine);
		}

		public static event OnCheaterReport CheaterReportEvent;
		public delegate void OnCheaterReport(ref CheaterReportEvent ev);

		public static void InvokeCheaterReport(string reporterId, string reportedId, string reportedIp, string reason, int serverId, ref bool allow)
		{
			if (CheaterReportEvent == null)
				return;

			CheaterReportEvent ev = new CheaterReportEvent()
			{
				Allow = allow,
				Report = reason,
				ReportedId = reportedId,
				ReportedIp = reportedIp,
				ReporterId = reporterId
			};

			CheaterReportEvent.Invoke(ref ev);

			allow = ev.Allow;
		}

		public static event WaitingForPlayers WaitingForPlayersEvent;
		public delegate void WaitingForPlayers();

		public static void InvokeWaitingForPlayers() => WaitingForPlayersEvent?.Invoke();

		public static event TeamRespawn TeamRespawnEvent;
		public delegate void TeamRespawn(ref TeamRespawnEvent ev);

		public static void InvokeTeamRespawn(ref bool isChaos, ref int maxRespawn, ref List<ReferenceHub> playersToRespawn)
		{
			if (TeamRespawnEvent == null)
				return;

			TeamRespawnEvent ev = new TeamRespawnEvent()
			{
				IsChaos = isChaos,
				MaxRespawnAmt = maxRespawn,
				ToRespawn = playersToRespawn
			};

			TeamRespawnEvent.Invoke(ref ev);

			maxRespawn = ev.MaxRespawnAmt;
			playersToRespawn = ev.ToRespawn;
		}


		public static event PlayerBanned PlayerBannedEvent;
		public delegate void PlayerBanned(PlayerBannedEvent ev);

		public static void InvokePlayerBanned(BanDetails banDetails, BanType banType)
		{
			if (PlayerBannedEvent == null)
				return;

			PlayerBannedEvent ev = new PlayerBannedEvent()
			{
				Details = banDetails,
				Type = banType
			};

			PlayerBannedEvent.Invoke(ev);
		}

		public static event PlayerBan PlayerBanEvent;
		public delegate void PlayerBan(PlayerBanEvent ev);

		public static void InvokePlayerBan(ref ReferenceHub player, ref string userId, ref int duration, ref bool allow, ref string message, ref string reason, ref ReferenceHub issuer)
		{
			if (PlayerBanEvent == null)
				return;
			// ev.Allow is already set to true in the constructor
			// Private field values set in the constructor to avoid triggering the logs
			PlayerBanEvent ev = new PlayerBanEvent(Plugin.Config.GetBool("exiled_log_ban_event", true), player, reason, userId, duration, issuer)
			{
				FullMessage = message,
				Reason = reason
			};

			PlayerBanEvent.Invoke(ev);
			allow = ev.Allow;
			userId = ev.UserId;
			duration = ev.Duration;
			message = ev.FullMessage;
			reason = ev.Reason;
			player = ev.BannedPlayer;
			issuer = ev.Issuer;
		}

		public static event SetGroup SetGroupEvent;
		public delegate void SetGroup(SetGroupEvent ev);

		public static void InvokeSetGroup(GameObject player, ref UserGroup group, ref bool allow)
		{
			if (SetGroupEvent == null)
				return;

			SetGroupEvent ev = new SetGroupEvent()
			{
				Player = player.GetPlayer(),
				Group = group,
				Allow = allow
			};

			SetGroupEvent.Invoke(ev);
			allow = ev.Allow;
			group = ev.Group;
		}
	}
}
