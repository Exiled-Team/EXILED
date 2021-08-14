// -----------------------------------------------------------------------
// <copyright file="NPClient.cs" company="Exiled Team">
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
    using Exiled.Events.EventArgs;
    using Exiled.Network.API;
    using Exiled.Network.API.Attributes;
    using Exiled.Network.API.Interfaces;
    using Exiled.Network.API.Models;
    using Exiled.Network.API.Packets;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using MEC;
    using Mirror;

    using static Broadcast;

    /// <summary>
    /// Network client.
    /// </summary>
    public class NPClient : NPManager, INetEventListener
    {
        private MainClass plugin;

        private NetDataWriter defaultdata;

        /// <summary>
        /// Initializes a new instance of the <see cref="NPClient"/> class.
        /// </summary>
        /// <param name="plugin">Plugin class.</param>
        public NPClient(MainClass plugin)
        {
            this.plugin = plugin;
            defaultdata = new NetDataWriter();
            defaultdata.Put(plugin.Config.HostConnectionKey);
            defaultdata.Put(Server.Port);
            defaultdata.Put(CustomNetworkManager.slots);
            Logger = new PluginLogger();
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pluginDir = Path.Combine(folderPath, "EXILED", "Plugins", "NetworkedPlugins");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);
            if (!Directory.Exists(Path.Combine(pluginDir, "addons-" + Server.Port)))
                Directory.CreateDirectory(Path.Combine(pluginDir, "addons-" + Server.Port));
            string[] addonsFiles = Directory.GetFiles(Path.Combine(pluginDir, "addons-" + Server.Port), "*.dll");
            Log.Info($"Loading {addonsFiles.Length} addons.");
            foreach (var file in addonsFiles)
            {
                Assembly a = Assembly.LoadFrom(file);
                try
                {
                    foreach (Type t in a.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
                    {
                        if (!t.BaseType.IsGenericType || t.BaseType.GetGenericTypeDefinition() != typeof(NPAddonClient<>))
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
                        {
                            continue;
                        }

                        NPAddonInfo addonInfo = (NPAddonInfo)Attribute.GetCustomAttribute(t, typeof(NPAddonInfo));
                        addon.Manager = this;
                        addon.Logger = Logger;
                        addon.AddonId = addonInfo.AddonID;
                        addon.DefaultPath = Path.Combine(pluginDir, $"addons-{Server.Port}");
                        addon.AddonPath = Path.Combine(addon.DefaultPath, addonInfo.AddonName);
                        if (Addons.ContainsKey(addonInfo.AddonID))
                        {
                            Logger.Error($"Addon {Addons[addonInfo.AddonID].Info.AddonName} already have id {addonInfo.AddonName}.");
                            break;
                        }

                        Addons.Add(addonInfo.AddonID, new NPAddonItem() { Addon = addon, Info = addonInfo });
                        LoadAddonConfig(addon.AddonId);
                        Logger.Info($"Loading addon {addonInfo.AddonName}.");
                        addon.OnEnable();
                        Logger.Info($"Waiting to client connections..");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed loading addon {Path.GetFileNameWithoutExtension(file)}. {ex.ToString()}");
                }
            }

            Logger.Info($"Starting CLIENT network...");
            Exiled.Events.Handlers.Server.SendingRemoteAdminCommand += Server_SendingRemoteAdminCommand;
            Exiled.Events.Handlers.Server.SendingConsoleCommand += Server_SendingConsoleCommand;
            Exiled.Events.Handlers.Player.Destroying += Player_Destroying;
            Exiled.Events.Handlers.Player.Verified += Player_Verified;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            StartNetworkClient();
        }

        private void Server_WaitingForPlayers()
        {
            UpdatePlayers();
        }

        private Dictionary<string, NPPlayer> players = new Dictionary<string, NPPlayer>();

        private void Player_Verified(VerifiedEventArgs ev)
        {
            if (!players.ContainsKey(ev.Player.UserId))
            {
                players.Add(ev.Player.UserId, new NPPlayer(null, ev.Player.UserId));
            }
        }

        private void Player_Destroying(DestroyingEventArgs ev)
        {
            if (players.ContainsKey(ev.Player.UserId))
            {
                players.Remove(ev.Player.UserId);
            }
        }

        private IEnumerator<float> DataCheckers()
        {
            players = new Dictionary<string, NPPlayer>();
            foreach (var plr in Player.List)
            {
                players.Add(plr.UserId, new NPPlayer(null, plr.UserId));
            }

            UpdatePlayers();
            while (true)
            {
                yield return Timing.WaitForSeconds(0.1f);

                try
                {
                    foreach (var plr in players)
                    {
                        if (!CanSendData)
                            continue;
                        var realPlayer = Player.Get(plr.Key);
                        var player = plr.Value;
                        if (realPlayer != null)
                        {
                            NetDataWriter writer = new NetDataWriter();
                            if (player.UserName != realPlayer.Nickname)
                            {
                                player.UserName = realPlayer.Nickname;
                                writer = new NetDataWriter();
                                writer.Put(player.UserName);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)0, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.Role != (int)realPlayer.Role)
                            {
                                player.Role = (int)realPlayer.Role;
                                writer = new NetDataWriter();
                                writer.Put(player.Role);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)1, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.DoNotTrack != realPlayer.DoNotTrack)
                            {
                                player.DoNotTrack = realPlayer.DoNotTrack;
                                writer = new NetDataWriter();
                                writer.Put(player.DoNotTrack);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)2, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.RemoteAdminAccess != realPlayer.RemoteAdminAccess)
                            {
                                player.RemoteAdminAccess = realPlayer.RemoteAdminAccess;
                                writer = new NetDataWriter();
                                writer.Put(player.RemoteAdminAccess);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)3, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.IsOverwatchEnabled != realPlayer.IsOverwatchEnabled)
                            {
                                player.IsOverwatchEnabled = realPlayer.IsOverwatchEnabled;
                                writer = new NetDataWriter();
                                writer.Put(player.IsOverwatchEnabled);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)4, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.IPAddress != realPlayer.IPAddress)
                            {
                                player.IPAddress = realPlayer.IPAddress;
                                writer = new NetDataWriter();
                                writer.Put(player.IPAddress);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)5, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.IsMuted != realPlayer.IsMuted)
                            {
                                player.IsMuted = realPlayer.IsMuted;
                                writer = new NetDataWriter();
                                writer.Put(player.IsMuted);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)6, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.IsIntercomMuted != realPlayer.IsIntercomMuted)
                            {
                                player.IsIntercomMuted = realPlayer.IsIntercomMuted;
                                writer = new NetDataWriter();
                                writer.Put(player.IsIntercomMuted);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)7, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.IsGodModeEnabled != realPlayer.IsGodModeEnabled)
                            {
                                player.IsGodModeEnabled = realPlayer.IsGodModeEnabled;
                                writer = new NetDataWriter();
                                writer.Put(player.IsGodModeEnabled);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)8, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.Health != realPlayer.Health)
                            {
                                player.Health = realPlayer.Health;
                                writer = new NetDataWriter();
                                writer.Put(player.Health);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)9, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.MaxHealth != realPlayer.MaxHealth)
                            {
                                player.MaxHealth = realPlayer.MaxHealth;
                                writer = new NetDataWriter();
                                writer.Put(player.MaxHealth);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)10, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.GroupName != realPlayer.GroupName)
                            {
                                player.GroupName = realPlayer.GroupName;
                                writer = new NetDataWriter();
                                writer.Put(player.GroupName);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)11, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.RankColor != realPlayer.RankColor)
                            {
                                player.RankColor = realPlayer.RankColor;
                                writer = new NetDataWriter();
                                writer.Put(player.RankColor);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)12, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                            }

                            if (player.RankName != realPlayer.RankName)
                            {
                                player.RankName = realPlayer.RankName;
                                writer = new NetDataWriter();
                                writer.Put(player.RankName);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)13,  Data = writer.Data, }, DeliveryMethod.ReliableOrdered);
                            }

                            if (realPlayer.SessionVariables.TryGetValue("SP", out object sendP))
                            {
                                if ((bool)sendP && (player.Position.X != realPlayer.Position.x || player.Position.Y != realPlayer.Position.y || player.Position.Z != realPlayer.Position.z))
                                {
                                    writer = new NetDataWriter();
                                    player.Position = new Position()
                                    {
                                        X = realPlayer.Position.x,
                                        Y = realPlayer.Position.y,
                                        Z = realPlayer.Position.z,
                                    };
                                    writer.Put<Position>(player.Position);
                                    PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)14, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                                }
                            }

                            if (realPlayer.SessionVariables.TryGetValue("SP", out object sendR))
                            {
                                if ((bool)sendR && (player.Rotation.X != realPlayer.Rotation.x || player.Rotation.Y != realPlayer.Rotation.y || player.Rotation.Z != realPlayer.Rotation.z))
                                {
                                    writer = new NetDataWriter();
                                    player.Rotation = new Rotation()
                                    {
                                        X = realPlayer.Rotation.x,
                                        Y = realPlayer.Rotation.y,
                                        Z = realPlayer.Rotation.z,
                                    };
                                    writer.Put<Rotation>(player.Rotation);
                                    PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)15, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                                }
                            }

                            if (player.PlayerID != realPlayer.Id)
                            {
                                player.PlayerID = realPlayer.Id;
                                writer = new NetDataWriter();
                                writer.Put(player.PlayerID);
                                PacketProcessor.Send<UpdatePlayerInfoPacket>(NetworkListener, new UpdatePlayerInfoPacket() { UserID = player.UserID, Type = (byte)16, Data = writer.Data, }, DeliveryMethod.ReliableOrdered);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }
        }

        private bool CanSendData { get; set; } = false;

        private void Server_SendingConsoleCommand(Exiled.Events.EventArgs.SendingConsoleCommandEventArgs ev)
        {
            foreach (var command in registerdCommands)
            {
                if (!string.IsNullOrEmpty(command.CommandName))
                {
                    if (!command.IsRaCommand && command.CommandName == ev.Name.ToUpper())
                    {
                        ev.IsAllowed = false;
                        PacketProcessor.Send<ExecuteCommandPacket>(NetworkListener, new ExecuteCommandPacket() { UserID = ev.Player.UserId, AddonID = command.AddonID, CommandName = command.CommandName, Arguments = ev.Arguments.ToArray() }, DeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }

        private void Server_SendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            foreach (var command in registerdCommands)
            {
                if (!string.IsNullOrEmpty(command.CommandName))
                {
                    if (command.IsRaCommand && command.CommandName == ev.Name.ToUpper())
                    {
                        ev.IsAllowed = false;
                        PacketProcessor.Send<ExecuteCommandPacket>(NetworkListener, new ExecuteCommandPacket() { UserID = ev.Sender.UserId, AddonID = command.AddonID, CommandName = command.CommandName, Arguments = ev.Arguments.ToArray() }, DeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }

        private void StartNetworkClient()
        {
            PacketProcessor.RegisterNestedType<CommandInfoPacket>();
            PacketProcessor.RegisterNestedType<PlayerInfoPacket>();
            PacketProcessor.RegisterNestedType<Position>();
            PacketProcessor.RegisterNestedType<Rotation>();
            PacketProcessor.SubscribeReusable<ReceiveAddonDataPacket, NetPeer>(OnReceiveAddonsData);
            PacketProcessor.SubscribeReusable<ReceiveAddonsPacket, NetPeer>(OnReceiveAddons);
            PacketProcessor.SubscribeReusable<PlayerInteractPacket, NetPeer>(OnPlayerInteract);
            PacketProcessor.SubscribeReusable<RoundRestartPacket, NetPeer>(OnRoundRestart);
            PacketProcessor.SubscribeReusable<SendBroadcastPacket, NetPeer>(OnSendBroadcast);
            PacketProcessor.SubscribeReusable<SendHintPacket, NetPeer>(OnSendHint);
            PacketProcessor.SubscribeReusable<ClearBroadcastsPacket, NetPeer>(OnClearBroadcast);
            PacketProcessor.SubscribeReusable<ReceiveCommandsPacket, NetPeer>(OnReceiveCommandsData);
            PacketProcessor.SubscribeReusable<ExecuteConsoleCommandPacket, NetPeer>(OnExecuteConsoleCommand);
            NetworkListener = new NetManager(this);
            NetworkListener.Start();
            NetworkListener.Connect(plugin.Config.HostAddress, plugin.Config.HostPort, defaultdata);
            Timing.RunCoroutine(RefreshPolls());
            Timing.RunCoroutine(SendPlayersInfo());
        }

        private void OnExecuteConsoleCommand(ExecuteConsoleCommandPacket packet, NetPeer peer)
        {
            var sender = new CustomConsoleExecutor(this, packet.Command);
            GameCore.Console.singleton.TypeCommand(packet.Command, sender);
        }

        private void OnSendHint(SendHintPacket packet, NetPeer peer)
        {
            foreach (var plr in Player.List)
            {
                if (plr.ReferenceHub.serverRoles.LocalRemoteAdmin || !packet.IsAdminOnly)
                    plr.ShowHint(packet.Message, packet.Duration);
            }
        }

        private void OnRoundRestart(RoundRestartPacket packet, NetPeer peer)
        {
            if (packet.Port != 0)
                ReferenceHub.HostHub.playerStats.RpcRoundrestartRedirect(0f, packet.Port);
            else
                ReferenceHub.HostHub.playerStats.Roundrestart();
        }

        private void OnReceiveAddonsData(ReceiveAddonDataPacket packet, NetPeer peer)
        {
            var reader = new NetDataReader(packet.Data);
            foreach (var addon in Addons.Where(p5 => p5.Key == packet.AddonID))
            {
                try
                {
                    addon.Value.Addon.OnMessageReceived(null, reader);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error while invoking OnMessageReceived in addon {addon.Value.Info.AddonName} {ex.ToString()}");
                }
            }
        }

        private void OnClearBroadcast(ClearBroadcastsPacket packet, NetPeer peer)
        {
            Server.Broadcast.RpcClearElements();
        }

        private void OnSendBroadcast(SendBroadcastPacket packet, NetPeer peer)
        {
            if (packet.IsAdminOnly)
            {
                foreach (var plr in Player.List)
                {
                    if (plr.ReferenceHub.serverRoles.LocalRemoteAdmin)
                        plr.Broadcast(packet.Duration, packet.Message, BroadcastFlags.Normal, false);
                    else
                        Server.Broadcast.RpcAddElement(packet.Message, packet.Duration, BroadcastFlags.Normal);
                }
            }
        }

        private void OnPlayerInteract(PlayerInteractPacket packet, NetPeer peer)
        {
            NetDataReader reader = new NetDataReader(packet.Data);

            Player p = (packet.UserID == "SERVER CONSOLE" || packet.UserID == "GAME CONSOLE") ? Player.Get(PlayerManager.localPlayer) : Player.Get(packet.UserID);
            if (p == null)
            {
                Logger.Info($"Player not found {packet.UserID}, action: {packet.Type}.");
                return;
            }

            switch (packet.Type)
            {
                // Kill player
                case 0:
                    p.Kill();
                    break;

                // Report message
                case 1:
                    p.SendConsoleMessage("[REPORTING] " + reader.GetString(), "GREEN");
                    break;

                // Remoteadmin message
                case 2:
                    p.RemoteAdminMessage(reader.GetString(), true, "NP");
                    break;

                // Console message
                case 3:
                    p.SendConsoleMessage(reader.GetString(), reader.GetString());
                    break;

                // Redirect
                case 4:
                    SendClientToServer(p, reader.GetUShort());
                    break;

                // Disconnect
                case 5:
                    ServerConsole.Disconnect(p.GameObject, reader.GetString());
                    break;

                // Hint
                case 6:
                    p.ShowHint(reader.GetString(), reader.GetFloat());
                    break;

                // Send position to network
                case 7:
                    bool sendPosition = reader.GetBool();
                    if (!p.SessionVariables.ContainsKey("SP"))
                        p.SessionVariables.Add("SP", sendPosition);
                    p.SessionVariables["SP"] = sendPosition;
                    break;

                // Send rotation to network
                case 8:
                    bool sendRotation = reader.GetBool();
                    if (!p.SessionVariables.ContainsKey("SR"))
                        p.SessionVariables.Add("SR", sendRotation);
                    p.SessionVariables["SR"] = sendRotation;
                    break;

                // Teleport
                case 9:
                    p.Position = new UnityEngine.Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
                    break;

                // Godmode
                case 10:
                    p.IsGodModeEnabled = reader.GetBool();
                    break;

                // Noclip
                case 11:
                    p.NoClipEnabled = reader.GetBool();
                    break;

                // Clear Inv
                case 12:
                    p.ClearInventory();
                    break;
            }
        }

        private void OnReceiveAddons(ReceiveAddonsPacket packet, NetPeer peer)
        {
            foreach (var addon in Addons.Where(p => packet.AddonIds.Contains(p.Key)))
            {
                try
                {
                    addon.Value.Addon.OnReady(null);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error while invoking OnReady in addon {addon.Value.Info.AddonName} {ex.ToString()}");
                }
            }

            CanSendData = true;
        }

        private void OnReceiveCommandsData(ReceiveCommandsPacket packet, NetPeer peer)
        {
            registerdCommands = packet.Commands;
            foreach (var command in registerdCommands)
            {
                Logger.Info($"Command {command.CommandName} registered from addon {command.AddonID}, isRa?: {command.IsRaCommand}");
            }
        }

        private List<CommandInfoPacket> registerdCommands = new List<CommandInfoPacket>();

        /// <summary>
        /// Redirect client to other server.
        /// </summary>
        /// <param name="hub">Player.</param>
        /// <param name="port">Server port.</param>
        public void SendClientToServer(Player hub, ushort port)
        {
            var serverPS = hub.ReferenceHub.playerStats;
            NetworkWriter writer = NetworkWriterPool.GetWriter();
            writer.WriteSingle(1f);
            writer.WriteUInt16(port);
            RpcMessage msg = new RpcMessage
            {
                netId = serverPS.netId,
                componentIndex = serverPS.ComponentIndex,
                functionHash = GetMethodHash(typeof(PlayerStats), "RpcRoundrestartRedirect"),
                payload = writer.ToArraySegment(),
            };
            hub.Connection.Send<RpcMessage>(msg, 0);
            NetworkWriterPool.Recycle(writer);
        }

        private static int GetMethodHash(Type invokeClass, string methodName)
        {
            return (invokeClass.FullName.GetStableHashCode() * 503) + methodName.GetStableHashCode();
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

        private IEnumerator<float> SendPlayersInfo()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(5f);
                try
                {
                    UpdatePlayers();
                }
                catch (Exception)
                {
                }
            }
        }

        private void UpdatePlayers()
        {
            List<PlayerInfoPacket> players = new List<PlayerInfoPacket>();
            foreach (var plr in Player.List)
            {
                players.Add(new PlayerInfoPacket()
                {
                    UserID = plr.UserId,
                });
            }

            PacketProcessor.Send<ReceivePlayersDataPacket>(NetworkListener, new ReceivePlayersDataPacket() { Players = players }, DeliveryMethod.ReliableOrdered);
        }

        private CoroutineHandle dataChecker;

        private IEnumerator<float> Reconnect()
        {
            yield return Timing.WaitForSeconds(5f);
            Logger.Info($"Reconnecting to {plugin.Config.HostAddress}:{plugin.Config.HostPort}...");
            NetworkListener.Connect(plugin.Config.HostAddress, plugin.Config.HostPort, defaultdata);
        }

        /// <inheritdoc/>
        public void OnPeerConnected(NetPeer peer)
        {
            Logger.Info("Client connected to host.");
            List<string> addon = new List<string>();
            foreach (var addon2 in Addons)
                addon.Add(addon2.Key);
            PacketProcessor.Send<ReceiveAddonsPacket>(peer, new ReceiveAddonsPacket() { AddonIds = addon.ToArray() }, DeliveryMethod.ReliableOrdered);
            Logger.Info("Addons info sended to host, waiting to response...");
            dataChecker = Timing.RunCoroutine(DataCheckers());
        }

        /// <inheritdoc/>
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Logger.Info($"Client disconnected from host. (Info: {disconnectInfo.Reason.ToString()})");
            Timing.RunCoroutine(Reconnect());
            registerdCommands.Clear();
            CanSendData = false;
            if (dataChecker != null)
                Timing.KillCoroutines(dataChecker);
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
                Logger.Error($"Error while receiving data from server {ex}");
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
        }
    }
}
