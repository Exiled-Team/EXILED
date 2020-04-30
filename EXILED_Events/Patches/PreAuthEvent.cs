using Cryptography;
using GameCore;
using LiteNetLib;
using LiteNetLib.Utils;
using Mirror.LiteNetLib4Mirror;
using System;
using System.Collections.Generic;
using System.Net;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CustomLiteNetLib4MirrorTransport), nameof(CustomLiteNetLib4MirrorTransport.ProcessConnectionRequest), typeof(ConnectionRequest))]
	public class PreAuthEvent
	{
		public static bool Prefix(ref ConnectionRequest request)
		{
			try
			{
				if (EventPlugin.PreAuthEventPatchDisable)
					return true;
				HandleConnection(request);
				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"PreAuthEvent error: {exception}");
				return true;
			}
		}

		private static void HandleConnection(ConnectionRequest request)
		{
			NetDataWriter rejectData = new NetDataWriter();
			try
			{
				byte result1;
				byte result2;
				int position = request.Data.Position;
				if (!request.Data.TryGetByte(out result1) || !request.Data.TryGetByte(out result2) || result1 != CustomNetworkManager.Major || result2 != CustomNetworkManager.Minor)
				{
					rejectData.Reset();
					rejectData.Put(3);
					request.RejectForce(rejectData);
				}
				else
				{
					if (CustomLiteNetLib4MirrorTransport.IpRateLimiting)
					{
						if (CustomLiteNetLib4MirrorTransport.IpRateLimit.Contains(request.RemoteEndPoint.Address.ToString()))
						{
							ServerConsole.AddLog(string.Format("Incoming connection from endpoint {0} rejected due to exceeding the rate limit.", request.RemoteEndPoint));
							ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Format("Incoming connection from endpoint {0} rejected due to exceeding the rate limit.", request.RemoteEndPoint), ServerLogs.ServerLogType.RateLimit);
							rejectData.Reset();
							rejectData.Put(12);
							request.RejectForce(rejectData);
							return;
						}
						CustomLiteNetLib4MirrorTransport.IpRateLimit.Add(request.RemoteEndPoint.Address.ToString());
					}
					if (!CharacterClassManager.OnlineMode)
					{
						KeyValuePair<BanDetails, BanDetails> keyValuePair = BanHandler.QueryBan(null, request.RemoteEndPoint.Address.ToString());
						if (keyValuePair.Value != null)
						{
							ServerConsole.AddLog(string.Format("Player tried to connect from banned endpoint {0}.", request.RemoteEndPoint));
							rejectData.Reset();
							rejectData.Put(6);
							rejectData.Put(keyValuePair.Value.Expires);
							rejectData.Put(keyValuePair.Value?.Reason ?? string.Empty);
							request.RejectForce(rejectData);
						}
						else
							request.Accept();
					}
					else
					{
						string result3;
						if (!request.Data.TryGetString(out result3) || result3 == string.Empty)
						{
							rejectData.Reset();
							rejectData.Put(5);
							request.RejectForce(rejectData);
						}
						else
						{
							ulong result4;
							byte result5;
							string result6;
							byte[] result7;
							if (!request.Data.TryGetULong(out result4) || !request.Data.TryGetByte(out result5) || !request.Data.TryGetString(out result6) || !request.Data.TryGetBytesWithLength(out result7))
							{
								rejectData.Reset();
								rejectData.Put(4);
								request.RejectForce(rejectData);
							}
							else
							{
								CentralAuthPreauthFlags flags = (CentralAuthPreauthFlags)result5;
								try
								{
									if (!ECDSA.VerifyBytes(string.Format("{0};{1};{2};{3}", result3, result5, result6, result4), result7, ServerConsole.PublicKey))
									{
										ServerConsole.AddLog(string.Format("Player from endpoint {0} sent preauthentication token with invalid digital signature.", request.RemoteEndPoint));
										rejectData.Reset();
										rejectData.Put(2);
										request.RejectForce(rejectData);
									}
									else if (TimeBehaviour.CurrentUnixTimestamp > result4)
									{
										ServerConsole.AddLog(string.Format("Player from endpoint {0} sent expired preauthentication token.", request.RemoteEndPoint));
										ServerConsole.AddLog("Make sure that time and timezone set on server is correct. We recommend synchronizing the time.");
										rejectData.Reset();
										rejectData.Put(11);
										request.RejectForce(rejectData);
									}
									else
									{
										if (CustomLiteNetLib4MirrorTransport.UserRateLimiting)
										{
											if (CustomLiteNetLib4MirrorTransport.UserRateLimit.Contains(result3))
											{
												ServerConsole.AddLog(string.Format("Incoming connection from {0} ({1}) rejected due to exceeding the rate limit.", result3, request.RemoteEndPoint));
												ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Format("Incoming connection from endpoint {0} ({1}) rejected due to exceeding the rate limit.", result3, request.RemoteEndPoint), ServerLogs.ServerLogType.RateLimit);
												rejectData.Reset();
												rejectData.Put(12);
												request.RejectForce(rejectData);
												return;
											}
											CustomLiteNetLib4MirrorTransport.UserRateLimit.Add(result3);
										}
										if (!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreBans) || !ServerStatic.GetPermissionsHandler().IsVerified)
										{
											KeyValuePair<BanDetails, BanDetails> keyValuePair = BanHandler.QueryBan(result3, request.RemoteEndPoint.Address.ToString());
											if (keyValuePair.Key != null || keyValuePair.Value != null)
											{
												ServerConsole.AddLog(string.Format("{0} {1} tried to connect from {2} endpoint {3}.", keyValuePair.Key == null ? "Player" : "Banned player", result3, keyValuePair.Value == null ? "" : "banned ", request.RemoteEndPoint));
												ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Format("{0} {1} tried to connect from {2} endpoint {3}.", keyValuePair.Key == null ? "Player" : "Banned player", result3, keyValuePair.Value == null ? "" : "banned ", request.RemoteEndPoint), ServerLogs.ServerLogType.ConnectionUpdate);
												rejectData.Reset();
												rejectData.Put(6);
												NetDataWriter netDataWriter1 = rejectData;
												BanDetails key = keyValuePair.Key;
												netDataWriter1.Put(key != null ? key.Expires : keyValuePair.Value.Expires);
												NetDataWriter netDataWriter2 = rejectData;
												string str;
												if ((str = keyValuePair.Key?.Reason) == null)
													str = keyValuePair.Value?.Reason ?? string.Empty;
												netDataWriter2.Put(str);
												request.Reject(rejectData);
												return;
											}
										}
										if (flags.HasFlagFast(CentralAuthPreauthFlags.GloballyBanned) && !ServerStatic.GetPermissionsHandler().IsVerified)
										{
											bool useGlobalBans = CustomLiteNetLib4MirrorTransport.UseGlobalBans;
										}
										if ((!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreWhitelist) || !ServerStatic.GetPermissionsHandler().IsVerified) && !WhiteList.IsWhitelisted(result3))
										{
											ServerConsole.AddLog(string.Format("Player {0} tried joined from endpoint {1}, but is not whitelisted.", result3, request.RemoteEndPoint));
											rejectData.Reset();
											rejectData.Put(7);
											request.Reject(rejectData);
										}
										else if (CustomLiteNetLib4MirrorTransport.Geoblocking != GeoblockingMode.None && (!flags.HasFlagFast(CentralAuthPreauthFlags.IgnoreGeoblock) || !ServerStatic.GetPermissionsHandler().BanTeamBypassGeo) && (!CustomLiteNetLib4MirrorTransport.GeoblockIgnoreWhitelisted || !WhiteList.IsOnWhitelist(result3)) && (CustomLiteNetLib4MirrorTransport.Geoblocking == GeoblockingMode.Whitelist && !CustomLiteNetLib4MirrorTransport.GeoblockingList.Contains(result6.ToUpper()) || CustomLiteNetLib4MirrorTransport.Geoblocking == GeoblockingMode.Blacklist && CustomLiteNetLib4MirrorTransport.GeoblockingList.Contains(result6.ToUpper())))
										{
											ServerConsole.AddLog(string.Format("Player {0} ({1}) tried joined from blocked country {2}.", result3, request.RemoteEndPoint, result6.ToUpper()));
											rejectData.Reset();
											rejectData.Put(9);
											request.RejectForce(rejectData);
										}
										else
										{
											int num = CustomNetworkManager.slots;
											if (flags.HasFlagFast(CentralAuthPreauthFlags.ReservedSlot) && ServerStatic.GetPermissionsHandler().BanTeamSlots)
												num = LiteNetLib4MirrorNetworkManager.singleton.maxConnections;
											else if (ConfigFile.ServerConfig.GetBool("use_reserved_slots", true) && ReservedSlot.HasReservedSlot(result3))
												num += CustomNetworkManager.reservedSlots;
											if (LiteNetLib4MirrorCore.Host.PeersCount < num)
											{
												if (CustomLiteNetLib4MirrorTransport.UserIds.ContainsKey(request.RemoteEndPoint))
													CustomLiteNetLib4MirrorTransport.UserIds[request.RemoteEndPoint].SetUserId(result3);
												else
													CustomLiteNetLib4MirrorTransport.UserIds.Add(request.RemoteEndPoint, new PreauthItem(result3));
												bool allow = true;
												Events.InvokePreAuth(result3, request, position, result5, result6, ref allow);
												if (allow)
												{
													request.Accept();
													ServerConsole.AddLog(string.Format("Player {0} preauthenticated from endpoint {1}.", result3, request.RemoteEndPoint));
													ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Format("{0} preauthenticated from endpoint {1}.", result3, request.RemoteEndPoint), ServerLogs.ServerLogType.ConnectionUpdate);
												}
												else
												{
													ServerConsole.AddLog(string.Format("Player {0} tried to preauthenticate from endpoint {1}, but the request has been rejected by a plugin.", result3, request.RemoteEndPoint));
													ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Format("{0} tried to preauthenticate from endpoint {1}, but the request has been rejected by a plugin.", result3, request.RemoteEndPoint), ServerLogs.ServerLogType.ConnectionUpdate);
												}
											}
											else
											{
												rejectData.Reset();
												rejectData.Put(1);
												request.Reject(rejectData);
											}
										}
									}
								}
								catch (Exception exception)
								{
									Log.Error(string.Format("Player from endpoint {0} sent an invalid preauthentication token. {1}", request.RemoteEndPoint, exception.Message));
									rejectData.Reset();
									rejectData.Put(2);
									request.RejectForce(rejectData);
								}
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error(string.Format("Player from endpoint {0} failed to preauthenticate: {1}", request.RemoteEndPoint, exception.Message));
				rejectData.Reset();
				rejectData.Put(4);
				request.RejectForce(rejectData);
			}
		}
	}
}