// -----------------------------------------------------------------------
// <copyright file="Host.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.DedicatedNetwork
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Threading.Tasks;

    using Exiled.Network;
    using Exiled.Network.API;
    using Exiled.Network.API.Attributes;
    using Exiled.Network.API.Interfaces;
    using Exiled.Network.API.Models;
    using Exiled.Network.API.Packets;

    using LiteNetLib;
    using LiteNetLib.Utils;

    /// <summary>
    /// Dedicated host.
    /// </summary>
    public class Host : NPManager, INetEventListener
    {
        private DedicatedConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        public Host()
        {
            Logger = new ConsoleLogger();

            if (!File.Exists("./config.yml"))
                File.WriteAllText("./config.yml", YamlDS.Serializer.Serialize(new DedicatedConfig()));

            config = YamlDS.Deserializer.Deserialize<DedicatedConfig>(File.ReadAllText("./config.yml"));
            if (!Directory.Exists("./addons"))
                Directory.CreateDirectory("./addons");

            string[] addonsFiles = Directory.GetFiles("./addons", "*.dll");
            Logger.Info($"Loading {addonsFiles.Length} addons.");
            foreach (var file in addonsFiles)
            {
                Assembly a = Assembly.LoadFrom(file);
                try
                {
                    string addonID = string.Empty;
                    foreach (Type t in a.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
                    {
                        if (!t.BaseType.IsGenericType || t.BaseType.GetGenericTypeDefinition() != typeof(NPAddonDedicated<>))
                        {
                            continue;
                        }

                        IAddon<IConfig> addon = null;

                        var constructor = t.GetConstructor(Type.EmptyTypes);
                        if (constructor != null)
                        {
                            addon = constructor.Invoke(null) as IAddon<IConfig>;
                        }
                        else
                        {
                            var value = Array.Find(t.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), property => property.PropertyType == t)?.GetValue(null);

                            if (value != null)
                                addon = value as IAddon<IConfig>;
                        }

                        if (addon == null)
                            continue;

                        NPAddonInfo addonInfo = (NPAddonInfo)Attribute.GetCustomAttribute(t, typeof(NPAddonInfo));
                        addon.Manager = this;
                        addon.Logger = Logger;
                        addon.AddonId = addonInfo.AddonID;
                        addonID = addonInfo.AddonID;
                        addon.DefaultPath = Path.Combine("addons");
                        addon.AddonPath = Path.Combine(addon.DefaultPath, addonInfo.AddonName);
                        if (Addons.TryGetValue(addonInfo.AddonID, out NPAddonItem addonExists))
                        {
                            Logger.Error($"Addon {addonExists.Info.AddonName} already have id {addonInfo.AddonID}.");
                            break;
                        }

                        Addons.Add(addonInfo.AddonID, new NPAddonItem() { Addon = addon, Info = addonInfo });
                        LoadAddonConfig(addon.AddonId);
                        Logger.Info($"Loading addon {addonInfo.AddonVersion}.");
                        addon.OnEnable();
                        Logger.Info($"Waiting to client connections..");
                        foreach (var type in a.GetTypes())
                        {
                            if (typeof(ICommand).IsAssignableFrom(type))
                            {
                                ICommand cmd = (ICommand)Activator.CreateInstance(type);
                                RegisterCommand(addon.AddonId, cmd);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed loading addon {Path.GetFileNameWithoutExtension(file)}. {ex.ToString()}");
                }
            }

            Logger.Info($"Starting HOST network...");
            StartNetworkHost();
        }

        private void StartNetworkHost()
        {
            PacketProcessor.RegisterNestedType<CommandInfoPacket>();
            PacketProcessor.RegisterNestedType<PlayerInfoPacket>();
            PacketProcessor.RegisterNestedType<Position>();
            PacketProcessor.RegisterNestedType<Rotation>();
            PacketProcessor.SubscribeReusable<ReceiveAddonsPacket, NetPeer>(OnReceiveAddons);
            PacketProcessor.SubscribeReusable<ReceiveAddonDataPacket, NetPeer>(OnReceiveAddonData);
            PacketProcessor.SubscribeReusable<ReceivePlayersDataPacket, NetPeer>(OnReceivePlayersData);
            PacketProcessor.SubscribeReusable<ExecuteCommandPacket, NetPeer>(OnExecuteCommand);
            PacketProcessor.SubscribeReusable<UpdatePlayerInfoPacket, NetPeer>(OnUpdatePlayerInfo);
            PacketProcessor.SubscribeReusable<ConsoleResponsePacket, NetPeer>(OnConsoleResponse);
            NetworkListener = new NetManager(this);
            Logger.Info($"IP: {config.HostAddress}");
            Logger.Info($"Port: {config.HostPort}");
            NetworkListener.Start(config.HostPort);
            Task.Factory.StartNew(async () =>
            {
                await RefreshPolls();
            });
        }

        private void OnConsoleResponse(ConsoleResponsePacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            foreach (var addon in Addons)
            {
                addon.Value.Addon.OnConsoleResponse(server, packet.Command, packet.Response, packet.isRemoteAdmin);
            }
        }

        private void OnUpdatePlayerInfo(UpdatePlayerInfoPacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            if (!server.Players.ContainsKey(packet.UserID))
                server.Players.Add(packet.UserID, new NPPlayer(server, packet.UserID));

            if (!server.Players.TryGetValue(packet.UserID, out NPPlayer player))
                return;

            NetDataReader reader = new NetDataReader(packet.Data);

            switch (packet.Type)
            {
                case 0:
                    player.UserName = reader.GetString();
                    break;
                case 1:
                    player.Role = reader.GetInt();
                    break;
                case 2:
                    player.DoNotTrack = reader.GetBool();
                    break;
                case 3:
                    player.RemoteAdminAccess = reader.GetBool();
                    break;
                case 4:
                    player.IsOverwatchEnabled = reader.GetBool();
                    break;
                case 5:
                    player.IPAddress = reader.GetString();
                    break;
                case 6:
                    player.IsMuted = reader.GetBool();
                    break;
                case 7:
                    player.IsIntercomMuted = reader.GetBool();
                    break;
                case 8:
                    player.IsGodModeEnabled = reader.GetBool();
                    break;
                case 9:
                    player.Health = reader.GetFloat();
                    break;
                case 10:
                    player.MaxHealth = reader.GetInt();
                    break;
                case 11:
                    player.GroupName = reader.GetString();
                    break;
                case 12:
                    player.RankColor = reader.GetString();
                    break;
                case 13:
                    player.RankName = reader.GetString();
                    break;
                case 14:
                    player.Position = reader.Get<Position>();
                    break;
                case 15:
                    player.Rotation = reader.Get<Rotation>();
                    break;
                case 16:
                    player.PlayerID = reader.GetInt();
                    break;
            }
        }

        private void OnExecuteCommand(ExecuteCommandPacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;
            if (!server.Players.TryGetValue(packet.UserID, out NPPlayer player))
                return;
            ExecuteCommand(player, packet.AddonID, packet.CommandName, packet.Arguments.ToList());
        }

        private void OnReceivePlayersData(ReceivePlayersDataPacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            List<string> onlinePlayers = new List<string>();
            foreach (var plr in packet.Players)
            {
                if (!server.Players.TryGetValue(plr.UserID, out NPPlayer player))
                {
                    server.Players.Add(plr.UserID, new NPPlayer(server, plr.UserID));
                }

                onlinePlayers.Add(plr.UserID);
            }

            foreach (var offlinePlayer in server.Players.Where(p => !onlinePlayers.Contains(p.Key)).Select(p => p.Key))
            {
                server.Players.Remove(offlinePlayer);
            }
        }

        private void OnReceiveAddonData(ReceiveAddonDataPacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;
            NetDataReader reader = new NetDataReader(packet.Data);
            foreach (var addon in Addons.Where(pp => pp.Key == packet.AddonID))
            {
                addon.Value.Addon.OnMessageReceived(server, reader);
            }
        }

        private void OnReceiveAddons(ReceiveAddonsPacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            string adds = string.Empty;
            List<CommandInfoPacket> cmds = new List<CommandInfoPacket>();
            List<string> addonsId = new List<string>();
            foreach (var i in packet.AddonIds.Where(p => Addons.ContainsKey(p)).Select(s => Addons[s]))
            {
                Servers[peer].Addons.Add(i);
                adds += $"{i.Info.AddonName} - {i.Info.AddonVersion}v made by {i.Info.AddonAuthor}" + Environment.NewLine;
                i.Addon.OnReady(Servers[peer]);
                cmds.AddRange(GetCommands(i.Info.AddonID));
                addonsId.Add(i.Info.AddonID);
            }

            Logger.Info($"Received addons from server {server.FullAddress}, {adds}");
            PacketProcessor.Send<ReceiveCommandsPacket>(peer, new ReceiveCommandsPacket() { Commands = cmds }, DeliveryMethod.ReliableOrdered);
            PacketProcessor.Send<ReceiveAddonsPacket>(peer, new ReceiveAddonsPacket() { AddonIds = addonsId.ToArray() }, DeliveryMethod.ReliableOrdered);
        }

        private async Task RefreshPolls()
        {
            while (true)
            {
                await Task.Delay(15);
                if (NetworkListener != null)
                {
                    if (NetworkListener.IsRunning)
                        NetworkListener.PollEvents();
                }
            }
        }

        /// <inheritdoc/>
        public void OnPeerConnected(NetPeer peer)
        {
            Logger.Info($"Client {peer.EndPoint.Address.ToString()} connected to host.");
        }

        /// <inheritdoc/>
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (Servers.TryGetValue(peer, out NPServer server))
            {
                Logger.Info($"Client {server.FullAddress} disconnected from host. (Info: {disconnectInfo.Reason.ToString()})");
                Servers.Remove(peer);
            }
        }

        /// <inheritdoc/>
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Logger.Error($"Network error from endpoint {endPoint.Address}, {socketError}");
        }

        /// <inheritdoc/>
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            try
            {
                PacketProcessor.ReadAllPackets(reader, peer);
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed in {peer.EndPoint.Address}, {ex}");
            }
        }

        /// <inheritdoc/>
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }

        /// <inheritdoc/>
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        /// <inheritdoc/>
        public void OnConnectionRequest(ConnectionRequest request)
        {
            if (request.Data.TryGetString(out string key))
            {
                if (key == config.HostConnectionKey)
                {
                    if (request.Data.TryGetUShort(out ushort port))
                    {
                        if (request.Data.TryGetInt(out int maxplayers))
                        {
                            var peer = request.Accept();
                            if (!Servers.ContainsKey(peer))
                                Servers.Add(peer, new NPServer(peer, PacketProcessor, peer.EndPoint.Address.ToString(), port, maxplayers));
                            else
                                Servers[peer] = new NPServer(peer, PacketProcessor, peer.EndPoint.Address.ToString(), port, maxplayers);
                            Logger.Info($"New server added {peer.EndPoint.Address.ToString()}, port: {port}");
                            return;
                        }
                    }
                }
            }
        }
    }
}
