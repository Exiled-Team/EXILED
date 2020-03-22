using GameCore;
using Harmony;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), new[] { typeof(GameObject), typeof(int), typeof(string), typeof(string), typeof(bool) })]
	public class PlayerBanEvent
	{
		public static bool Prefix(GameObject user, int duration, string reason, string issuer, bool isGlobalBan)
		{
			if (isGlobalBan && ConfigFile.ServerConfig.GetBool("gban_ban_ip", false))
			{
				duration = int.MaxValue;
			}
			string userId = null;
			string address = user.GetComponent<NetworkIdentity>().connectionToClient.address;
			CharacterClassManager characterClassManager = null;
			ReferenceHub userHub = Extensions.Player.GetPlayer(user);
			ReferenceHub issuerHub = Extensions.Player.GetPlayer(issuer);
			if (issuerHub == null) issuerHub = Extensions.Player.GetPlayer(PlayerManager.localPlayer);
			try
			{
				if (ConfigFile.ServerConfig.GetBool("online_mode", false))
				{
					characterClassManager = userHub.characterClassManager;
					userId = characterClassManager.UserId;
				}
			}
			catch
			{
				ServerConsole.AddLog("Failed during issue of User ID ban (1)!");
				return false;
			}
			string message = $"You have been {((duration > 0) ? "banned" : "kicked")}. ";
			if (!string.IsNullOrEmpty(reason))
			{
				message = message + "Reason: " + reason;
			}

			try
			{
				bool allow = true;

				Events.InvokePlayerBan(ref userHub, ref userId, ref duration, ref allow, ref message, ref reason, ref issuerHub);

				if (!allow)
					return false;
			}
			catch (Exception exception)
			{
				Log.Error($"BanUserEvent error: {exception}");
			}

			if (duration > 0 && (!ServerStatic.PermissionsHandler.IsVerified || !userHub.serverRoles.BypassStaff))
			{
				string originalName = string.IsNullOrEmpty(userHub.nicknameSync.MyNick) ? "(no nick)" : userHub.nicknameSync.MyNick;
				long issuanceTime = TimeBehaviour.CurrentTimestamp();
				long banExpieryTime = TimeBehaviour.GetBanExpieryTime((uint)duration);
				try
				{
					if (userId != null && !isGlobalBan)
					{
						BanHandler.IssueBan(new BanDetails
						{
							OriginalName = originalName,
							Id = userId,
							IssuanceTime = issuanceTime,
							Expires = banExpieryTime,
							Reason = reason,
							Issuer = issuer
						}, BanHandler.BanType.UserId);
						if (!string.IsNullOrEmpty(characterClassManager.UserId2))
						{
							BanHandler.IssueBan(new BanDetails
							{
								OriginalName = originalName,
								Id = characterClassManager.UserId2,
								IssuanceTime = issuanceTime,
								Expires = banExpieryTime,
								Reason = reason,
								Issuer = issuer
							}, BanHandler.BanType.UserId);
						}
					}
				}
				catch
				{
					ServerConsole.AddLog("Failed during issue of User ID ban (2)!");
					return false;
				}
				try
				{
					if (ConfigFile.ServerConfig.GetBool("ip_banning", false) || isGlobalBan)
					{
						BanHandler.IssueBan(new BanDetails
						{
							OriginalName = originalName,
							Id = address,
							IssuanceTime = issuanceTime,
							Expires = banExpieryTime,
							Reason = reason,
							Issuer = issuer
						}, BanHandler.BanType.IP);
					}
				}
				catch
				{
					ServerConsole.AddLog("Failed during issue of IP ban!");
					return false;
				}
			}
			List<GameObject> playersToBan = new List<GameObject>();
			foreach (GameObject gameObject in PlayerManager.players)
			{
				characterClassManager = gameObject.GetComponent<CharacterClassManager>();
				if ((userId != null && characterClassManager.UserId == userId) || (address != null && characterClassManager.connectionToClient.address == address))
				{
					playersToBan.Add(characterClassManager.gameObject);
				}
			}
			foreach (GameObject player in playersToBan)
			{
				ServerConsole.Disconnect(player, message);
			}
			return false;
		}
	}
}
