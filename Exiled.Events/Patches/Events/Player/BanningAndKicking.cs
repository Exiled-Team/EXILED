// -----------------------------------------------------------------------
// <copyright file="BanningAndKicking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using GameCore;

    using HarmonyLib;

    using Mirror;

    using UnityEngine;

    using Log = Exiled.API.Features.Log;

    /// <summary>
    ///     Patches <see cref="BanPlayer.BanUser(GameObject, long, string, string, bool)" />.
    ///     Adds the <see cref="Handlers.Player.Banning" /> and <see cref="Handlers.Player.Kicking" />events.
    /// </summary>
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), typeof(GameObject), typeof(long), typeof(string), typeof(string), typeof(bool))]
    internal static class BanningAndKicking
    {
        private static bool Prefix(GameObject user, long duration, string reason, string issuer, bool isGlobalBan)
        {
            try
            {
                if (isGlobalBan && ConfigFile.ServerConfig.GetBool("gban_ban_ip")) duration = int.MaxValue;

                string userId = null;
                string address = user.GetComponent<NetworkIdentity>().connectionToClient.address;

                Player targetPlayer = Player.Get(user);
                Player issuerPlayer;
                if (issuer.Contains("("))
                    issuerPlayer = Player.Get(issuer.Substring(issuer.LastIndexOf('(') + 1).TrimEnd(')')) ?? Server.Host;
                else
                    issuerPlayer = Server.Host;

                try
                {
                    if (ConfigFile.ServerConfig.GetBool("online_mode"))
                        userId = targetPlayer.UserId;
                }
                catch
                {
                    ServerConsole.AddLog("Failed during issue of User ID ban (1)!");
                    return false;
                }

                string message = $"You have been {(duration > 0 ? "banned" : "kicked")}. ";
                if (!string.IsNullOrEmpty(reason)) message = message + "Reason: " + reason;

                if (!ServerStatic.PermissionsHandler.IsVerified || !targetPlayer.IsStaffBypassEnabled)
                {
                    if (duration > 0)
                    {
                        BanningEventArgs ev = new(targetPlayer, issuerPlayer, duration, reason, message);
                        Handlers.Player.OnBanning(ev);

                        if (!ev.IsAllowed)
                            return false;

                        duration = ev.Duration;
                        reason = ev.Reason;
                        message = ev.FullMessage;

                        string originalName = string.IsNullOrEmpty(targetPlayer.Nickname)
                            ? "(no nick)"
                            : targetPlayer.Nickname;
                        long issuanceTime = TimeBehaviour.CurrentTimestamp();
                        long banExpieryTime = TimeBehaviour.GetBanExpirationTime((uint) duration);
                        try
                        {
                            if (userId is not null && !isGlobalBan)
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
                        catch
                        {
                            ServerConsole.AddLog("Failed during issue of User ID ban (2)!");
                            return false;
                        }

                        try
                        {
                            if (ConfigFile.ServerConfig.GetBool("ip_banning") || isGlobalBan)
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
                        catch
                        {
                            ServerConsole.AddLog("Failed during issue of IP ban!");
                            return false;
                        }
                    }
                    else if (duration == 0)
                    {
                        KickingEventArgs ev = new(targetPlayer, issuerPlayer, reason, message);
                        Handlers.Player.OnKicking(ev);

                        if (!ev.IsAllowed)
                            return false;

                        reason = ev.Reason;
                        message = ev.FullMessage;
                    }
                }

                ServerConsole.Disconnect(targetPlayer.ReferenceHub.gameObject, message);

                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Exiled.Events.Patches.Events.Player.BanningAndKicking: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
