// -----------------------------------------------------------------------
// <copyright file="PreAuthenticating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cryptography;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using GameCore;
    using HarmonyLib;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using Mirror.LiteNetLib4Mirror;

    /// <summary>
    /// Patches <see cref="CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest(ConnectionRequest)"/>.
    /// Adds the <see cref="Player.PreAuthenticating"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest), typeof(ConnectionRequest))]
    internal class PreAuthenticating
    {
        private static bool Prefix(ref ConnectionRequest request)
        {
            HandleConnection(request);
            return false;
        }

        /// <summary>
        /// Handle the player connection.
        /// </summary>
        /// <param name="request">The <see cref="ConnectionRequest"/> instance.</param>
        private static void HandleConnection(ConnectionRequest request)
        {
            try
            {
                int position = request.Data.Position;
                if (!request.Data.TryGetByte(out byte result1) || !request.Data.TryGetByte(out byte result2) || !request.Data.TryGetByte(out byte result3) || result1 != CustomNetworkManager.Major || result2 != CustomNetworkManager.Minor || result3 != CustomNetworkManager.SyncedRevision)
                {
                    CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                    CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)3);
                    request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                }
                else
                {
                    bool flag = request.Data.TryGetInt(out int result4);
                    if (!request.Data.TryGetBytesWithLength(out byte[] result5))
                        flag = false;
                    if (!flag)
                    {
                        CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)15);
                        request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                    }
                    else if (CustomLiteNetLib4MirrorTransport.DelayConnections)
                    {
                        CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
                        CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)17);
                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put(CustomLiteNetLib4MirrorTransport.DelayTime);
                        if (CustomLiteNetLib4MirrorTransport.DelayVolume < byte.MaxValue)
                            ++CustomLiteNetLib4MirrorTransport.DelayVolume;
                        if (CustomLiteNetLib4MirrorTransport.DelayVolume < CustomLiteNetLib4MirrorTransport.DelayVolumeThreshold)
                        {
                            ServerConsole.AddLog(
                              $"Delayed connection incoming from endpoint {request.RemoteEndPoint} by {CustomLiteNetLib4MirrorTransport.DelayTime} seconds.");
                            request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
                        }
                        else
                        {
                            ServerConsole.AddLog(
                              $"Force delayed connection incoming from endpoint {request.RemoteEndPoint} by {CustomLiteNetLib4MirrorTransport.DelayTime} seconds.");
                            request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                        }
                    }
                    else
                    {
                        if (CustomLiteNetLib4MirrorTransport.UseChallenge)
                        {
                            if (result4 == 0 || result5 == null || result5.Length == 0)
                            {
                                if (!CustomLiteNetLib4MirrorTransport.CheckIpRateLimit(request))
                                    return;
                                int num = 0;
                                string key = string.Empty;
                                for (byte index = 0; index < 3; ++index)
                                {
                                    num = RandomGenerator.GetInt32();
                                    if (num == 0)
                                        num = 1;
                                    key = request.RemoteEndPoint.Address + "-" + num;
                                    if (CustomLiteNetLib4MirrorTransport.Challenges.ContainsKey(key))
                                    {
                                        if (index == 2)
                                        {
                                            CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                            CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)4);
                                            request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                            ServerConsole.AddLog(
                                              $"Failed to generate ID for challenge for incoming connection from endpoint {request.RemoteEndPoint}.");
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                byte[] bytes = RandomGenerator.GetBytes(CustomLiteNetLib4MirrorTransport.ChallengeInitLen + CustomLiteNetLib4MirrorTransport.ChallengeSecretLen, true);
                                ServerConsole.AddLog(
                                  $"Requested challenge for incoming connection from endpoint {request.RemoteEndPoint}.");
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)13);
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)CustomLiteNetLib4MirrorTransport.ChallengeMode);
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put(num);
                                switch (CustomLiteNetLib4MirrorTransport.ChallengeMode)
                                {
                                    case ChallengeType.MD5:
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(bytes, 0, CustomLiteNetLib4MirrorTransport.ChallengeInitLen);
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put(CustomLiteNetLib4MirrorTransport.ChallengeSecretLen);
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(Md.Md5(bytes));
                                        CustomLiteNetLib4MirrorTransport.Challenges.Add(key, new PreauthChallengeItem(new ArraySegment<byte>(bytes, CustomLiteNetLib4MirrorTransport.ChallengeInitLen, CustomLiteNetLib4MirrorTransport.ChallengeSecretLen)));
                                        break;
                                    case ChallengeType.SHA1:
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(bytes, 0, CustomLiteNetLib4MirrorTransport.ChallengeInitLen);
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put(CustomLiteNetLib4MirrorTransport.ChallengeSecretLen);
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(Sha.Sha1(bytes));
                                        CustomLiteNetLib4MirrorTransport.Challenges.Add(key, new PreauthChallengeItem(new ArraySegment<byte>(bytes, CustomLiteNetLib4MirrorTransport.ChallengeInitLen, CustomLiteNetLib4MirrorTransport.ChallengeSecretLen)));
                                        break;
                                    default:
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(bytes);
                                        CustomLiteNetLib4MirrorTransport.Challenges.Add(key, new PreauthChallengeItem(new ArraySegment<byte>(bytes)));
                                        break;
                                }

                                request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
                                return;
                            }

                            string key1 = request.RemoteEndPoint.Address + "-" + result4;
                            if (!CustomLiteNetLib4MirrorTransport.Challenges.ContainsKey(key1))
                            {
                                ServerConsole.AddLog(
                                  $"Security challenge response of incoming connection from endpoint {request.RemoteEndPoint} has been REJECTED (invalid Challenge ID).");
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)14);
                                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                return;
                            }

                            ArraySegment<byte> validResponse = CustomLiteNetLib4MirrorTransport.Challenges[key1].ValidResponse;
                            if (!result5.SequenceEqual(validResponse))
                            {
                                ServerConsole.AddLog(
                                  $"Security challenge response of incoming connection from endpoint {request.RemoteEndPoint} has been REJECTED (invalid response).");
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)15);
                                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                return;
                            }

                            CustomLiteNetLib4MirrorTransport.Challenges.Remove(key1);
                            CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
                            ServerConsole.AddLog(
                              $"Security challenge response of incoming connection from endpoint {request.RemoteEndPoint} has been accepted.");
                        }
                        else if (!CustomLiteNetLib4MirrorTransport.CheckIpRateLimit(request))
                        {
                            return;
                        }

                        if (!CharacterClassManager.OnlineMode)
                        {
                            KeyValuePair<BanDetails, BanDetails> keyValuePair = BanHandler.QueryBan(null, request.RemoteEndPoint.Address.ToString());
                            if (keyValuePair.Value != null)
                            {
                                ServerConsole.AddLog($"Player tried to connect from banned endpoint {request.RemoteEndPoint}.");
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)6);
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put(keyValuePair.Value.Expires);
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put(keyValuePair.Value?.Reason ?? string.Empty);
                                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                            }
                            else
                            {
                                request.Accept();
                                CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
                            }
                        }
                        else
                        {
                            if (!request.Data.TryGetString(out string result6) || result6 == string.Empty)
                            {
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)5);
                                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                            }
                            else
                            {
                                if (!request.Data.TryGetULong(out ulong result7) || !request.Data.TryGetByte(out byte result8) || !request.Data.TryGetString(out string result9) || !request.Data.TryGetBytesWithLength(out byte[] result10))
                                {
                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)4);
                                    request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                }
                                else
                                {
                                    CentralAuthPreauthFlags flags = (CentralAuthPreauthFlags)result8;
                                    try
                                    {
                                        if (!ECDSA.VerifyBytes(
                                          $"{result6};{result8};{result9};{result7}", result10, ServerConsole.PublicKey))
                                        {
                                            ServerConsole.AddLog(
                                              $"Player from endpoint {request.RemoteEndPoint} sent prea-uthentication token with invalid digital signature.");
                                            CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                            CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)2);
                                            request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                        }
                                        else if (TimeBehaviour.CurrentUnixTimestamp > result7)
                                        {
                                            ServerConsole.AddLog(
                                              $"Player from endpoint {request.RemoteEndPoint} sent expired pre-authentication token.");
                                            ServerConsole.AddLog("Make sure that time and timezone set on server is correct. We recommend synchronizing the time.");
                                            CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                            CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)11);
                                            request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                        }
                                        else
                                        {
                                            if (CustomLiteNetLib4MirrorTransport.UserRateLimiting)
                                            {
                                                if (CustomLiteNetLib4MirrorTransport.UserRateLimit.Contains(result6))
                                                {
                                                    ServerConsole.AddLog(
                                                      $"Incoming connection from {result6} ({request.RemoteEndPoint}) rejected due to exceeding the rate limit.");
                                                    ServerLogs.AddLog(ServerLogs.Modules.Networking, $"Incoming connection from endpoint {result6} ({request.RemoteEndPoint}) rejected due to exceeding the rate limit.", ServerLogs.ServerLogType.RateLimit);
                                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)12);
                                                    request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                                    return;
                                                }

                                                CustomLiteNetLib4MirrorTransport.UserRateLimit.Add(result6);
                                            }

                                            if (!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreBans) || !ServerStatic.GetPermissionsHandler().IsVerified)
                                            {
                                                KeyValuePair<BanDetails, BanDetails> keyValuePair = BanHandler.QueryBan(result6, request.RemoteEndPoint.Address.ToString());
                                                if (keyValuePair.Key != null || keyValuePair.Value != null)
                                                {
                                                    ServerConsole.AddLog(
                                                      $"{(keyValuePair.Key == null ? "Player" : (object)"Banned player")} {result6} tried to connect from {(keyValuePair.Value == null ? string.Empty : (object)"banned ")} endpoint {request.RemoteEndPoint}.");
                                                    ServerLogs.AddLog(ServerLogs.Modules.Networking, $"{(keyValuePair.Key == null ? "Player" : (object)"Banned player")} {result6} tried to connect from {(keyValuePair.Value == null ? string.Empty : (object)"banned ")} endpoint {request.RemoteEndPoint}.", ServerLogs.ServerLogType.ConnectionUpdate);
                                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)6);
                                                    NetDataWriter requestWriter = CustomLiteNetLib4MirrorTransport.RequestWriter;
                                                    BanDetails key = keyValuePair.Key;
                                                    long num = key?.Expires ?? keyValuePair.Value.Expires;
                                                    requestWriter.Put(num);
                                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Put(keyValuePair.Key?.Reason ?? keyValuePair.Value?.Reason ?? string.Empty);
                                                    request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                                    return;
                                                }
                                            }

                                            if (flags.HasFlagFast(CentralAuthPreauthFlags.GloballyBanned) && (ServerStatic.PermissionsHandler.IsVerified || CustomLiteNetLib4MirrorTransport.UseGlobalBans))
                                            {
                                                ServerConsole.AddLog(
                                                  $"Player {result6} ({request.RemoteEndPoint}) kicked due to an active global ban.");
                                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)8);
                                                request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                            }
                                            else if ((!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreWhitelist) || !ServerStatic.GetPermissionsHandler().IsVerified) && !WhiteList.IsWhitelisted(result6))
                                            {
                                                ServerConsole.AddLog(
                                                  $"Player {result6} tried joined from endpoint {request.RemoteEndPoint}, but is not whitelisted.");
                                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)7);
                                                request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                            }
                                            else if (CustomLiteNetLib4MirrorTransport.Geoblocking != GeoblockingMode.None && (!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreGeoblock) || !ServerStatic.PermissionsHandler.BanTeamBypassGeo) && (!CustomLiteNetLib4MirrorTransport.GeoblockIgnoreWhitelisted || !WhiteList.IsOnWhitelist(result6)) && ((CustomLiteNetLib4MirrorTransport.Geoblocking == GeoblockingMode.Whitelist && !CustomLiteNetLib4MirrorTransport.GeoblockingList.Contains(result9.ToUpper())) || (CustomLiteNetLib4MirrorTransport.Geoblocking == GeoblockingMode.Blacklist && CustomLiteNetLib4MirrorTransport.GeoblockingList.Contains(result9.ToUpper()))))
                                            {
                                                ServerConsole.AddLog(
                                                  $"Player {result6} ({request.RemoteEndPoint}) tried joined from blocked country {result9.ToUpper()}.");
                                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)9);
                                                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                            }
                                            else
                                            {
                                                int num = CustomNetworkManager.slots;
                                                if (flags.HasFlagFast(CentralAuthPreauthFlags.ReservedSlot) && ServerStatic.PermissionsHandler.BanTeamSlots)
                                                    num = LiteNetLib4MirrorNetworkManager.singleton.maxConnections;
                                                else if (ConfigFile.ServerConfig.GetBool("use_reserved_slots", true) && ReservedSlot.HasReservedSlot(result6))
                                                    num += CustomNetworkManager.reservedSlots;
                                                if (LiteNetLib4MirrorCore.Host.ConnectedPeersCount < num)
                                                {
                                                    if (CustomLiteNetLib4MirrorTransport.UserIds.ContainsKey(request.RemoteEndPoint))
                                                        CustomLiteNetLib4MirrorTransport.UserIds[request.RemoteEndPoint].SetUserId(result6);
                                                    else
                                                        CustomLiteNetLib4MirrorTransport.UserIds.Add(request.RemoteEndPoint, new PreauthItem(result6));

                                                    PreAuthenticatingEventArgs ev = new PreAuthenticatingEventArgs(result6, request, position, result8, result9);

                                                    Player.OnPreAuthenticating(ev);

                                                    if (ev.IsAllowed)
                                                    {
                                                        request.Accept();
                                                        CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
                                                        ServerConsole.AddLog(
                                                          $"Player {result3} pre-authenticated from endpoint {request.RemoteEndPoint}.");
                                                        ServerLogs.AddLog(
                                                          ServerLogs.Modules.Networking,
                                                          $"{result3} pre-authenticated from endpoint {request.RemoteEndPoint}.",
                                                          ServerLogs.ServerLogType.ConnectionUpdate);
                                                    }
                                                    else
                                                    {
                                                        ServerConsole.AddLog(
                                                          $"Player {result3} tried to pre-authenticate from endpoint {request.RemoteEndPoint}, but the request has been rejected by a plugin.");
                                                        ServerLogs.AddLog(
                                                          ServerLogs.Modules.Networking,
                                                          $"{result3} tried to pre-authenticate from endpoint {request.RemoteEndPoint}, but the request has been rejected by a plugin.",
                                                          ServerLogs.ServerLogType.ConnectionUpdate);
                                                    }
                                                }
                                                else
                                                {
                                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)1);
                                                    request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ServerConsole.AddLog(
                                          $"Player from endpoint {request.RemoteEndPoint} sent an invalid pre-authentication token. {ex.Message}");
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)2);
                                        request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServerConsole.AddLog(
                  $"Player from endpoint {request.RemoteEndPoint} failed to pre-authenticate: {ex.Message}");
                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)4);
                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
            }
        }
    }
}
