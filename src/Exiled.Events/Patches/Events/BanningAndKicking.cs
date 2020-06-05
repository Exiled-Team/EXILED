﻿// -----------------------------------------------------------------------
// <copyright file="BanningAndKicking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
#pragma warning disable SA1313
    using System.Collections.Generic;

    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;

    using GameCore;

    using HarmonyLib;

    using Mirror;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="BanPlayer.BanUser(GameObject, int, string, string, bool)"/>.
    /// Adds the <see cref="Player.Banning"/> and <see cref="Player.Kicking"/>events.
    /// </summary>
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), new[] { typeof(GameObject), typeof(int), typeof(string), typeof(string), typeof(bool) })]
    public class BanningAndKicking
    {
        /// <summary>
        /// Prefix of <see cref="BanPlayer.BanUser(GameObject, int, string, string, bool)"/>.
        /// </summary>
        /// <param name="user"><inheritdoc cref="KickingEventArgs.Target"/></param>
        /// <param name="duration"><inheritdoc cref="BanningEventArgs.Duration"/></param>
        /// <param name="reason"><inheritdoc cref="KickingEventArgs.Reason"/></param>
        /// <param name="issuer"><inheritdoc cref="KickingEventArgs.Issuer"/></param>
        /// <param name="isGlobalBan">Indicates whether the ban is going to be global or not.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(GameObject user, int duration, string reason, string issuer, bool isGlobalBan)
        {
            if (isGlobalBan && ConfigFile.ServerConfig.GetBool("gban_ban_ip", false))
            {
                duration = int.MaxValue;
            }

            string userId = null;
            string address = user.GetComponent<NetworkIdentity>().connectionToClient.address;

            API.Features.Player targetPlayer = API.Features.Player.Get(user);
            API.Features.Player issuerPlayer = API.Features.Player.Get(issuer) ?? API.Features.Server.Host;

            try
            {
                if (ConfigFile.ServerConfig.GetBool("online_mode", false))
                    userId = targetPlayer.UserId;
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

            if (!ServerStatic.PermissionsHandler.IsVerified || !targetPlayer.IsStaffBypassEnabled)
            {
                if (duration > 0)
                {
                    var ev = new BanningEventArgs(targetPlayer, issuerPlayer, duration, reason, message);

                    Player.OnBanning(ev);

                    duration = ev.Duration;
                    reason = ev.Reason;
                    message = ev.FullMessage;

                    if (!ev.IsAllowed)
                        return false;

                    string originalName = string.IsNullOrEmpty(targetPlayer.Nickname) ? "(no nick)" : targetPlayer.Nickname;
                    long issuanceTime = TimeBehaviour.CurrentTimestamp();
                    long banExpieryTime = TimeBehaviour.GetBanExpieryTime((uint)duration);
                    try
                    {
                        if (userId != null && !isGlobalBan)
                        {
                            BanHandler.IssueBan(
                                new BanDetails
                                {
                                    OriginalName = originalName,
                                    Id = userId,
                                    IssuanceTime = issuanceTime,
                                    Expires = banExpieryTime,
                                    Reason = reason,
                                    Issuer = issuer,
                                }, BanHandler.BanType.UserId);

                            if (!string.IsNullOrEmpty(targetPlayer.CustomUserId))
                            {
                                BanHandler.IssueBan(
                                    new BanDetails
                                    {
                                        OriginalName = originalName,
                                        Id = targetPlayer.CustomUserId,
                                        IssuanceTime = issuanceTime,
                                        Expires = banExpieryTime,
                                        Reason = reason,
                                        Issuer = issuer,
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
                            BanHandler.IssueBan(
                                new BanDetails
                                {
                                    OriginalName = originalName,
                                    Id = address,
                                    IssuanceTime = issuanceTime,
                                    Expires = banExpieryTime,
                                    Reason = reason,
                                    Issuer = issuer,
                                }, BanHandler.BanType.IP);
                        }
                    }
                    catch
                    {
                        ServerConsole.AddLog("Failed during issue of IP ban!");
                        return false;
                    }
                }
                else if (duration == 0)
                {
                    var ev = new KickingEventArgs(targetPlayer, issuerPlayer, reason, message);

                    Player.OnKicking(ev);

                    reason = ev.Reason;
                    message = ev.FullMessage;

                    if (!ev.IsAllowed)
                        return false;
                }
            }

            List<GameObject> playersToBan = new List<GameObject>();
            foreach (GameObject gameObject in PlayerManager.players)
            {
                CharacterClassManager characterClassManager = gameObject.GetComponent<CharacterClassManager>();
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
