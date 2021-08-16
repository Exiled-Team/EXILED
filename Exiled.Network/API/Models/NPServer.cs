// -----------------------------------------------------------------------
// <copyright file="NPServer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Models
{
    using System.Collections.Generic;
    using Exiled.Network.API.Packets;
    using LiteNetLib;
    using LiteNetLib.Utils;

    /// <summary>
    /// Server.
    /// </summary>
    public class NPServer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NPServer"/> class.
        /// </summary>
        /// <param name="server">Server.</param>
        /// <param name="processor">Packet Processor.</param>
        /// <param name="serverAddress">Server Address.</param>
        /// <param name="port">Server Port.</param>
        /// <param name="maxPlayers">Max Players.</param>
        public NPServer(NetPeer server, NetPacketProcessor processor, string serverAddress, ushort port, int maxPlayers)
        {
            this.PacketProcessor = processor;
            this.Peer = server;
            this.ServerAddress = serverAddress;
            this.ServerPort = port;
            this.MaxPlayers = maxPlayers;
        }

        /// <summary>
        /// Gets or sets Packet processor.
        /// </summary>
        public NetPacketProcessor PacketProcessor { get; set; }

        /// <summary>
        /// Gets or sets Server peer.
        /// </summary>
        public NetPeer Peer { get; set; }

        /// <summary>
        /// Gets server address.
        /// </summary>
        public string ServerAddress { get; } = "localhost";

        /// <summary>
        /// Gets server port.
        /// </summary>
        public ushort ServerPort { get; } = 7777;

        /// <summary>
        /// Gets server max players.
        /// </summary>
        public int MaxPlayers { get; } = 25;

        /// <summary>
        /// Gets or sets current online players.
        /// </summary>
        public Dictionary<string, NPPlayer> Players { get; set; } = new Dictionary<string, NPPlayer>();

        /// <summary>
        /// Gets addons running on that server.
        /// </summary>
        public List<NPAddonItem> Addons { get; internal set; } = new List<NPAddonItem>();

        /// <summary>
        /// Gets server full address.
        /// </summary>
        public string FullAddress => $"{ServerAddress}:{ServerPort}";

        /// <summary>
        /// Gets player via userid.
        /// </summary>
        /// <param name="userId">Player UserID.</param>
        /// <returns>Player.</returns>
        public NPPlayer GetPlayer(string userId)
        {
            if (Players.ContainsKey(userId))
            {
                return Players[userId];
            }

            return null;
        }

        /// <summary>
        /// Execute command on server.
        /// </summary>
        /// <param name="command">Command name.</param>
        /// <param name="arguments">Command arguments.</param>
        public void ExecuteCommand(string command, List<string> arguments)
        {
            PacketProcessor.Send<ExecuteConsoleCommandPacket>(Peer, new ExecuteConsoleCommandPacket() { AddonID = string.Empty, Command = $"{command} {string.Join(" ", arguments)}" }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Display broadcast on server.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="duration">Duration of broadcast.</param>
        /// <param name="isAdminOnly">Is displayed only for admins.</param>
        public void SendBroadcast(string message, ushort duration, bool isAdminOnly = false)
        {
            PacketProcessor.Send<SendBroadcastPacket>(Peer, new SendBroadcastPacket() { Message = message, IsAdminOnly = isAdminOnly, Duration = duration }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Display hint on server.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="duration">Duration of hint.</param>
        /// <param name="isAdminOnly">Is displayed only for admins.</param>
        public void SendHint(string message, float duration, bool isAdminOnly = false)
        {
            PacketProcessor.Send<SendHintPacket>(Peer, new SendHintPacket() { Message = message, IsAdminOnly = isAdminOnly, Duration = duration }, DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Clear all broadcasts on server.
        /// </summary>
        public void ClearBroadcast()
        {
            PacketProcessor.Send<ClearBroadcastsPacket>(Peer, new ClearBroadcastsPacket(), DeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Roundrestart server.
        /// </summary>
        /// <param name="port">Optional for redirecting everyone to other server..</param>
        public void RoundRestart(ushort port = 0)
        {
            PacketProcessor.Send<RoundRestartPacket>(Peer, new RoundRestartPacket() { Port = port }, DeliveryMethod.ReliableOrdered);
        }
    }
}
