// -----------------------------------------------------------------------
// <copyright file="NPAddonHost.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.Network.API.Interfaces;
    using Exiled.Network.API.Models;
    using Exiled.Network.API.Packets;

    using LiteNetLib;
    using LiteNetLib.Utils;

    /// <summary>
    /// Network Host Addon.
    /// </summary>
    /// <param name="TConfig">Addon Config.</param>
    public abstract class NPAddonHost<TConfig> : IAddon<TConfig>
        where TConfig : IConfig, new()
    {
        /// <inheritdoc/>
        public NPManager Manager { get; set; }

        /// <inheritdoc/>
        public NPLogger Logger { get; set; }

        /// <inheritdoc/>
        public string AddonId { get; set; }

        /// <inheritdoc/>
        public string DefaultPath { get; set; }

        /// <inheritdoc/>
        public string AddonPath { get; set; }

        /// <summary>
        /// Send data to client.
        /// </summary>
        /// <param name="serverPort">Server port.</param>
        /// <param name="writer">Data writer.</param>
        public void SendData(ushort serverPort, NetDataWriter writer)
        {
            foreach (var obj in Manager.Servers)
            {
                if (obj.Value.ServerPort == serverPort)
                {
                    Manager.PacketProcessor.Send<ReceiveAddonDataPacket>(obj.Key, new ReceiveAddonDataPacket() { AddonID = AddonId, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        /// <summary>
        /// Send data to client.
        /// </summary>
        /// <param name="server">Server.</param>
        /// <param name="writer">Data writer.</param>
        public void SendData(NPServer server, NetDataWriter writer)
        {
            foreach (var obj in Manager.Servers)
            {
                if (obj.Value.FullAddress == server.FullAddress)
                {
                    Manager.PacketProcessor.Send<ReceiveAddonDataPacket>(obj.Key, new ReceiveAddonDataPacket() { AddonID = AddonId, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        /// <summary>
        /// Send data to client.
        /// </summary>
        /// <param name="serverAddress">Server address.</param>
        /// <param name="writer">Data writer.</param>
        public void SendData(string serverAddress, NetDataWriter writer)
        {
            foreach (var obj in Manager.Servers)
            {
                if (obj.Key.EndPoint.Address.ToString() == serverAddress)
                {
                    Manager.PacketProcessor.Send<ReceiveAddonDataPacket>(obj.Key, new ReceiveAddonDataPacket() { AddonID = AddonId, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        /// <summary>
        /// Send data to client.
        /// </summary>
        /// <param name="serverAddress">Server address.</param>
        /// <param name="serverPort">Server port.</param>
        /// <param name="writer">Data writer.</param>
        public void SendData(string serverAddress, ushort serverPort, NetDataWriter writer)
        {
            foreach (var obj in Manager.Servers)
            {
                if (obj.Key.EndPoint.Address.ToString() == serverAddress)
                {
                    if (obj.Value.ServerPort == serverPort)
                    {
                        Manager.PacketProcessor.Send<ReceiveAddonDataPacket>(obj.Key, new ReceiveAddonDataPacket() { AddonID = AddonId, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all online servers.
        /// </summary>
        /// <returns>List of NPServer.</returns>
        public List<NPServer> GetServers()
        {
            return Manager.Servers.Values.ToList();
        }

        /// <summary>
        /// Check if server running is online.
        /// </summary>
        /// <param name="port">Server Port.</param>
        /// <returns>Is Online.</returns>
        public bool IsServerOnline(ushort port)
        {
            return Manager.Servers.Any(p => p.Value.ServerPort == port);
        }

        /// <summary>
        /// Send data to all servers.
        /// </summary>
        /// <param name="writer">Data writer.</param>
        public void SendData(NetDataWriter writer)
        {
            Manager.PacketProcessor.Send<ReceiveAddonDataPacket>(Manager.NetworkListener, new ReceiveAddonDataPacket() { AddonID = AddonId, Data = writer.Data }, DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public virtual void OnMessageReceived(NPServer server, NetDataReader reader)
        {
        }

        /// <inheritdoc/>
        public virtual void OnEnable()
        {
        }

        /// <inheritdoc/>
        public virtual void OnReady(NPServer server)
        {
        }

        /// <inheritdoc/>
        public virtual void OnConsoleCommand(string cmd, List<string> arguments)
        {
        }

        /// <inheritdoc/>
        public virtual void OnConsoleResponse(NPServer server, string command, string response, bool isRa)
        {
        }

        /// <inheritdoc/>
        public TConfig Config { get; } = new TConfig();

        /// <summary>
        /// Compare configs.
        /// </summary>
        /// <param name="other"> Config.</param>
        /// <returns>Int.</returns>
        public int CompareTo(IAddon<IConfig> other) => 0;
    }
}
