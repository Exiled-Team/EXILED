// -----------------------------------------------------------------------
// <copyright file="NPHost.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using Exiled.API.Features;
    using Exiled.Network.API;
    using Exiled.Network.API.Attributes;
    using Exiled.Network.API.Interfaces;
    using Exiled.Network.API.Models;
    using Exiled.Network.API.Packets;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using MEC;

    /// <summary>
    /// Network host.
    /// </summary>
    public class NPHost : NPManager, INetEventListener
    {
        private CoroutineHandle refreshPolls;
        private MainClass plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="NPHost"/> class.
        /// </summary>
        /// <param name="plugin">Plugin class.</param>
        public NPHost(MainClass plugin)
        {
            NPManager.Singleton = this;
            this.plugin = plugin;
            Logger = new PluginLogger();
            string pluginDir = Path.Combine(Paths.Plugins, "Exiled.Network");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            if (!Directory.Exists(Path.Combine(pluginDir, "addons-" + Server.Port)))
                Directory.CreateDirectory(Path.Combine(pluginDir, "addons-" + Server.Port));
            string[] addonsFiles = Directory.GetFiles(Path.Combine(pluginDir, "addons-" + Server.Port), "*.dll");
            Logger.Info($"Loading {addonsFiles.Length} addons.");
            foreach (var file in addonsFiles)
            {
                Assembly a = Assembly.LoadFrom(file);
                try
                {
                    foreach (Type t in a.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
                    {
                        if (!t.BaseType.IsGenericType || t.BaseType.GetGenericTypeDefinition() != typeof(NPAddonHost<>))
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

                        var addonType = addon.GetType();
                        var prop = addonType.GetProperty("DefaultPath", BindingFlags.Public | BindingFlags.Instance);
                        var field = prop.GetBackingField();
                        field.SetValue(addon, Path.Combine(pluginDir, $"addons-{Server.Port}"));

                        prop = addonType.GetProperty("AddonPath", BindingFlags.Public | BindingFlags.Instance);
                        field = prop.GetBackingField();
                        field.SetValue(addon, Path.Combine(addon.DefaultPath, addon.AddonName));

                        prop = addonType.GetProperty("Manager", BindingFlags.Public | BindingFlags.Instance);
                        field = prop.GetBackingField();
                        field.SetValue(addon, this);

                        prop = addonType.GetProperty("Logger", BindingFlags.Public | BindingFlags.Instance);
                        field = prop.GetBackingField();
                        field.SetValue(addon, Logger);

                        if (Addons.ContainsKey(addon.AddonId))
                        {
                            Logger.Error($"Addon {addon.AddonName} already already registered with id {addon.AddonId}.");
                            break;
                        }

                        LoadAddonConfig(addon.AddonId);
                        if (!addon.Config.IsEnabled)
                            return;

                        Addons.Add(addon.AddonId, addon);
                        Logger.Info($"Loading addon {addon.AddonVersion}.");
                        addon.OnEnable();
                        Logger.Info($"Waiting to client connections..");
                        foreach (var type in a.GetTypes())
                        {
                            if (typeof(ICommand).IsAssignableFrom(type))
                            {
                                ICommand cmd = (ICommand)Activator.CreateInstance(type);
                                RegisterCommand(addon, cmd);
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

        /// <summary>
        /// Unload network host.
        /// </summary>
        public void Unload()
        {
            if (refreshPolls != null)
                Timing.KillCoroutines(refreshPolls);
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
            PacketProcessor.ReadAllPackets(reader, peer);
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
                if (key == plugin.Config.HostConnectionKey)
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
            NetworkListener.Start(plugin.Config.HostPort);
            refreshPolls = Timing.RunCoroutine(RefreshPolls());
        }

        private void OnConsoleResponse(ConsoleResponsePacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            foreach (var addon in Addons)
            {
                addon.Value.OnConsoleResponse(server, packet.Command, packet.Response, packet.IsRemoteAdmin);
            }
        }

        private void OnUpdatePlayerInfo(UpdatePlayerInfoPacket packet, NetPeer peer)
        {
            if (!Servers.TryGetValue(peer, out NPServer server))
                return;

            if (!server.Players.ContainsKey(packet.UserID))
            {
                server.Players.Add(packet.UserID, new NPPlayer(server, packet.UserID));
                Logger.Info($"Add missing player {packet.UserID}.");
            }

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
                addon.Value.OnMessageReceived(server, reader);
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
                adds += $"{i.AddonName} - {i.AddonVersion}v made by {i.AddonAuthor}" + Environment.NewLine;
                i.OnReady(Servers[peer]);
                cmds.AddRange(GetCommands(i.AddonId));
                addonsId.Add(i.AddonId);
            }

            Logger.Info($"Received addons from server {server.FullAddress}, {adds}");
            PacketProcessor.Send<ReceiveCommandsPacket>(NetworkListener, new ReceiveCommandsPacket() { Commands = cmds }, DeliveryMethod.ReliableOrdered);
            PacketProcessor.Send<ReceiveAddonsPacket>(NetworkListener, new ReceiveAddonsPacket() { AddonIds = addonsId.ToArray() }, DeliveryMethod.ReliableOrdered);
        }

        private IEnumerator<float> RefreshPolls()
        {
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                if (NetworkListener != null)
                {
                    if (NetworkListener.IsRunning)
                        NetworkListener.PollEvents();
                }
            }
        }
    }
}
