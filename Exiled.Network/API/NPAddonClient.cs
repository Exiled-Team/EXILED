// -----------------------------------------------------------------------
// <copyright file="NPAddonClient.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API
{
    using System.Collections.Generic;
    using Exiled.Network.API.Interfaces;
    using Exiled.Network.API.Models;
    using Exiled.Network.API.Packets;
    using LiteNetLib;
    using LiteNetLib.Utils;

    /// <summary>
    /// Network Client Addon.
    /// </summary>
    /// <param name="TConfig">Addon Config.</param>
    public abstract class NPAddonClient<TConfig> : IAddon<TConfig>
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
        /// Send data to server.
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

        /// <summary>
        /// Only works on server.
        /// </summary>
        /// <param name="cmd"> Dont use.</param>
        /// <param name="arguments"> Dont use that.</param>
        public void OnConsoleCommand(string cmd, List<string> arguments)
        {
        }

        /// <summary>
        /// Only works on server.
        /// </summary>
        /// <param name="server"> Dont use.</param>
        /// <param name="command"> Dont use that.</param>
        /// <param name="response"> Dont use that 2x.</param>
        /// <param name="isRa"> Dont use that 3x.</param>
        public void OnConsoleResponse(NPServer server, string command, string response, bool isRa)
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
