// -----------------------------------------------------------------------
// <copyright file="PreAuthenticating.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cryptography;
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
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
    public class PreAuthenticating
    {
        /// <summary>
        /// Prefix of <see cref="CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest(ConnectionRequest)"/>.
        /// </summary>
        /// <param name="request">The <see cref="ConnectionRequest"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(ref ConnectionRequest request)
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
                byte result1;
                byte result2;
                byte result3;
                int position = request.Data.Position;
                if (!request.Data.TryGetByte(out result1) || !request.Data.TryGetByte(out result2) || (!request.Data.TryGetByte(out result3) || (int)result1 != (int)CustomNetworkManager.Major) || ((int)result2 != (int)CustomNetworkManager.Minor || (int)result3 != (int)CustomNetworkManager.SyncedRevision))
                {
                    CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                    CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)3);
                    request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                }
                else
                {
                    int result4;
                    bool flag = request.Data.TryGetInt(out result4);
                    byte[] result5;
                    if (!request.Data.TryGetBytesWithLength(out result5))
                        flag = false;
                    if (!flag)
                    {
                        CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)15);
                        request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                    }
                    else if (CustomLiteNetLib4MirrorTransport.DelayConnections)
                    {
                        CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)17);
                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put(CustomLiteNetLib4MirrorTransport.DelayTime);
                        if (CustomLiteNetLib4MirrorTransport.DelayVolume < byte.MaxValue)
                            ++CustomLiteNetLib4MirrorTransport.DelayVolume;
                        if ((int)CustomLiteNetLib4MirrorTransport.DelayVolume < (int)CustomLiteNetLib4MirrorTransport.DelayVolumeThreshold)
                        {
                            ServerConsole.AddLog(string.Format("Delayed connection incoming from endpoint {0} by {1} seconds.", (object)request.RemoteEndPoint, (object)CustomLiteNetLib4MirrorTransport.DelayTime), ConsoleColor.Gray);
                            request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
                        }
                        else
                        {
                            ServerConsole.AddLog(string.Format("Force delayed connection incoming from endpoint {0} by {1} seconds.", (object)request.RemoteEndPoint, (object)CustomLiteNetLib4MirrorTransport.DelayTime), ConsoleColor.Gray);
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
                                for (byte index = 0; index < (byte)3; ++index)
                                {
                                    num = RandomGenerator.GetInt32(false);
                                    if (num == 0)
                                        num = 1;
                                    key = request.RemoteEndPoint.Address.ToString() + "-" + (object)num;
                                    if (CustomLiteNetLib4MirrorTransport.Challenges.ContainsKey(key))
                                    {
                                        if (index == (byte)2)
                                        {
                                            CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                            CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)4);
                                            request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                            ServerConsole.AddLog(string.Format("Failed to generate ID for challenge for incoming connection from endpoint {0}.", (object)request.RemoteEndPoint), ConsoleColor.Gray);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                byte[] bytes = RandomGenerator.GetBytes((int)CustomLiteNetLib4MirrorTransport.ChallengeInitLen + (int)CustomLiteNetLib4MirrorTransport.ChallengeSecretLen, true);
                                ServerConsole.AddLog(string.Format("Requested challenge for incoming connection from endpoint {0}.", (object)request.RemoteEndPoint), ConsoleColor.Gray);
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)13);
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)CustomLiteNetLib4MirrorTransport.ChallengeMode);
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put(num);
                                switch (CustomLiteNetLib4MirrorTransport.ChallengeMode)
                                {
                                    case ChallengeType.MD5:
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(bytes, 0, (int)CustomLiteNetLib4MirrorTransport.ChallengeInitLen);
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put(CustomLiteNetLib4MirrorTransport.ChallengeSecretLen);
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(Md.Md5(bytes));
                                        CustomLiteNetLib4MirrorTransport.Challenges.Add(key, new PreauthChallengeItem(new ArraySegment<byte>(bytes, (int)CustomLiteNetLib4MirrorTransport.ChallengeInitLen, (int)CustomLiteNetLib4MirrorTransport.ChallengeSecretLen)));
                                        break;
                                    case ChallengeType.SHA1:
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(bytes, 0, (int)CustomLiteNetLib4MirrorTransport.ChallengeInitLen);
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.Put(CustomLiteNetLib4MirrorTransport.ChallengeSecretLen);
                                        CustomLiteNetLib4MirrorTransport.RequestWriter.PutBytesWithLength(Sha.Sha1(bytes));
                                        CustomLiteNetLib4MirrorTransport.Challenges.Add(key, new PreauthChallengeItem(new ArraySegment<byte>(bytes, (int)CustomLiteNetLib4MirrorTransport.ChallengeInitLen, (int)CustomLiteNetLib4MirrorTransport.ChallengeSecretLen)));
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

                            string key1 = request.RemoteEndPoint.Address.ToString() + "-" + (object)result4;
                            if (!CustomLiteNetLib4MirrorTransport.Challenges.ContainsKey(key1))
                            {
                                ServerConsole.AddLog(string.Format("Security challenge response of incoming connection from endpoint {0} has been REJECTED (invalid Challenge ID).", (object)request.RemoteEndPoint), ConsoleColor.Gray);
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)14);
                                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                return;
                            }

                            ArraySegment<byte> validResponse = CustomLiteNetLib4MirrorTransport.Challenges[key1].ValidResponse;
                            if (!((IEnumerable<byte>)result5).SequenceEqual<byte>((IEnumerable<byte>)validResponse))
                            {
                                ServerConsole.AddLog(string.Format("Security challenge response of incoming connection from endpoint {0} has been REJECTED (invalid response).", (object)request.RemoteEndPoint), ConsoleColor.Gray);
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)15);
                                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                return;
                            }

                            CustomLiteNetLib4MirrorTransport.Challenges.Remove(key1);
                            CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
                            ServerConsole.AddLog(string.Format("Security challenge response of incoming connection from endpoint {0} has been accepted.", (object)request.RemoteEndPoint), ConsoleColor.Gray);
                        }
                        else if (!CustomLiteNetLib4MirrorTransport.CheckIpRateLimit(request))
                        {
                            return;
                        }

                        if (!CharacterClassManager.OnlineMode)
                        {
                            KeyValuePair<BanDetails, BanDetails> keyValuePair = BanHandler.QueryBan((string)null, request.RemoteEndPoint.Address.ToString());
                            if (keyValuePair.Value != null)
                            {
                                ServerConsole.AddLog(string.Format("Player tried to connect from banned endpoint {0}.", (object)request.RemoteEndPoint), ConsoleColor.Gray);
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
                                ServerConsole.AddLog(
                                  $"Player {result3} preauthenticated from endpoint {request.RemoteEndPoint}.");
                                ServerLogs.AddLog(
                                    ServerLogs.Modules.Networking,
                                    $"{result3} preauthenticated from endpoint {request.RemoteEndPoint}.",
                                    ServerLogs.ServerLogType.ConnectionUpdate);
                            }
                        }
                        else
                        {
                            string result6;
                            if (!request.Data.TryGetString(out result6) || result6 == string.Empty)
                            {
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)5);
                                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                            }
                            else
                            {
                                ulong result7;
                                byte result8;
                                string result9;
                                byte[] result10;
                                if (!request.Data.TryGetULong(out result7) || !request.Data.TryGetByte(out result8) || (!request.Data.TryGetString(out result9) || !request.Data.TryGetBytesWithLength(out result10)))
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
                                        if (!ECDSA.VerifyBytes(string.Format("{0};{1};{2};{3}", (object)result6, (object)result8, (object)result9, (object)result7), result10, ServerConsole.PublicKey))
                                        {
                                            ServerConsole.AddLog(string.Format("Player from endpoint {0} sent preauthentication token with invalid digital signature.", request.RemoteEndPoint), ConsoleColor.Gray);
                                            CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                            CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)2);
                                            request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                        }
                                        else if (TimeBehaviour.CurrentUnixTimestamp > result7)
                                        {
                                            ServerConsole.AddLog(string.Format("Player from endpoint {0} sent expired preauthentication token.", request.RemoteEndPoint), ConsoleColor.Gray);
                                            ServerConsole.AddLog("Make sure that time and timezone set on server is correct. We recommend synchronizing the time.", ConsoleColor.Gray);
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
                                                    ServerConsole.AddLog(string.Format("Incoming connection from {0} ({1}) rejected due to exceeding the rate limit.", result6, request.RemoteEndPoint), ConsoleColor.Gray);
                                                    ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Format("Incoming connection from endpoint {0} ({1}) rejected due to exceeding the rate limit.", result6, request.RemoteEndPoint), ServerLogs.ServerLogType.RateLimit);
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
                                                    ServerConsole.AddLog(string.Format("{0} {1} tried to connect from {2} endpoint {3}.", keyValuePair.Key == null ? (object)"Player" : (object)"Banned player", (object)result6, keyValuePair.Value == null ? string.Empty : "banned ", request.RemoteEndPoint), ConsoleColor.Gray);
                                                    ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Format("{0} {1} tried to connect from {2} endpoint {3}.", keyValuePair.Key == null ? (object)"Player" : (object)"Banned player", (object)result6, keyValuePair.Value == null ? string.Empty : "banned ", request.RemoteEndPoint), ServerLogs.ServerLogType.ConnectionUpdate);
                                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)6);
                                                    NetDataWriter requestWriter = CustomLiteNetLib4MirrorTransport.RequestWriter;
                                                    BanDetails key = keyValuePair.Key;
                                                    long num = key != null ? key.Expires : keyValuePair.Value.Expires;
                                                    requestWriter.Put(num);
                                                    CustomLiteNetLib4MirrorTransport.RequestWriter.Put(keyValuePair.Key?.Reason ?? keyValuePair.Value?.Reason ?? string.Empty);
                                                    request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                                    return;
                                                }
                                            }

                                            if (flags.HasFlagFast(CentralAuthPreauthFlags.GloballyBanned) && (ServerStatic.PermissionsHandler.IsVerified || CustomLiteNetLib4MirrorTransport.UseGlobalBans))
                                            {
                                                ServerConsole.AddLog(string.Format("Player {0} ({1}) kicked due to an active global ban.", (object)result6, (object)request.RemoteEndPoint), ConsoleColor.Gray);
                                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)8);
                                                request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                            }
                                            else if ((!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreWhitelist) || !ServerStatic.GetPermissionsHandler().IsVerified) && !WhiteList.IsWhitelisted(result6))
                                            {
                                                ServerConsole.AddLog(string.Format("Player {0} tried joined from endpoint {1}, but is not whitelisted.", (object)result6, (object)request.RemoteEndPoint), ConsoleColor.Gray);
                                                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                                                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)7);
                                                request.Reject(CustomLiteNetLib4MirrorTransport.RequestWriter);
                                            }
                                            else if (CustomLiteNetLib4MirrorTransport.Geoblocking != GeoblockingMode.None && (!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreGeoblock) || !ServerStatic.PermissionsHandler.BanTeamBypassGeo) && (!CustomLiteNetLib4MirrorTransport.GeoblockIgnoreWhitelisted || !WhiteList.IsOnWhitelist(result6)) && ((CustomLiteNetLib4MirrorTransport.Geoblocking == GeoblockingMode.Whitelist && !CustomLiteNetLib4MirrorTransport.GeoblockingList.Contains(result9.ToUpper())) || (CustomLiteNetLib4MirrorTransport.Geoblocking == GeoblockingMode.Blacklist && CustomLiteNetLib4MirrorTransport.GeoblockingList.Contains(result9.ToUpper()))))
                                            {
                                                ServerConsole.AddLog(string.Format("Player {0} ({1}) tried joined from blocked country {2}.", (object)result6, (object)request.RemoteEndPoint, (object)result9.ToUpper()), ConsoleColor.Gray);
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

                                                    var ev = new PreAuthenticatingEventArgs(result6, request, position, result8, result9);

                                                    Player.OnPreAuthenticating(ev);

                                                    if (ev.IsAllowed)
                                                    {
                                                        request.Accept();
                                                        CustomLiteNetLib4MirrorTransport.PreauthDisableIdleMode();
                                                        ServerConsole.AddLog(
                                                          $"Player {result3} preauthenticated from endpoint {request.RemoteEndPoint}.");
                                                        ServerLogs.AddLog(
                                                            ServerLogs.Modules.Networking,
                                                            $"{result3} preauthenticated from endpoint {request.RemoteEndPoint}.",
                                                            ServerLogs.ServerLogType.ConnectionUpdate);
                                                    }
                                                    else
                                                    {
                                                        ServerConsole.AddLog(
                                                          $"Player {result3} tried to preauthenticate from endpoint {request.RemoteEndPoint}, but the request has been rejected by a plugin.");
                                                        ServerLogs.AddLog(
                                                            ServerLogs.Modules.Networking,
                                                            $"{result3} tried to preauthenticate from endpoint {request.RemoteEndPoint}, but the request has been rejected by a plugin.",
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
                                        ServerConsole.AddLog(string.Format("Player from endpoint {0} sent an invalid preauthentication token. {1}", (object)request.RemoteEndPoint, (object)ex.Message), ConsoleColor.Gray);
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
                ServerConsole.AddLog(string.Format("Player from endpoint {0} failed to preauthenticate: {1}", (object)request.RemoteEndPoint, (object)ex.Message), ConsoleColor.Gray);
                CustomLiteNetLib4MirrorTransport.RequestWriter.Reset();
                CustomLiteNetLib4MirrorTransport.RequestWriter.Put((byte)4);
                request.RejectForce(CustomLiteNetLib4MirrorTransport.RequestWriter);
            }
        }
    }
}