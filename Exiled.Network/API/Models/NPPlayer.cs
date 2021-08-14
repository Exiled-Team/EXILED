// -----------------------------------------------------------------------
// <copyright file="NPPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Models
{
    using Exiled.Network.API.Interfaces;
    using Exiled.Network.API.Packets;

    using LiteNetLib;
    using LiteNetLib.Utils;

    /// <inheritdoc/>
    public class NPPlayer : PlayerFuncs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NPPlayer"/> class.
        /// </summary>
        /// <param name="server">Server where player is on.</param>
        /// <param name="userID">Player UserID.</param>
        public NPPlayer(NPServer server, string userID)
        {
            this.Server = server;
            this.UserID = userID;
        }

        /// <summary>
        /// Gets or sets erver where player is online.
        /// </summary>
        public NPServer Server { get; set; }

        /// <inheritdoc/>
        public override void Kill()
        {
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = 0, }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendReportMessage(string message)
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)1, Data = writer.Data }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendRAMessage(string message)
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)2, Data = writer.Data }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendConsoleMessage(string message, string color = "GREEN")
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            writer.Put(color);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)3, Data = writer.Data }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void Redirect(ushort port)
        {
            var writer = new NetDataWriter();
            writer.Put(port);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)4, Data = writer.Data }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void Disconnect(string reason)
        {
            var writer = new NetDataWriter();
            writer.Put(reason);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)5, Data = writer.Data }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendHint(string message, float duration)
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            writer.Put(duration);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)6, Data = writer.Data }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendPosition(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)7, Data = writer.Data, }, DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendRotation(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)8, Data = writer.Data }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void Teleport(float x, float y, float z)
        {
            var writer = new NetDataWriter();
            writer.Put(x);
            writer.Put(y);
            writer.Put(z);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)9, Data = writer.Data }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SetGodmode(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)10, Data = writer.Data }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SetNoclip(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)11, Data = writer.Data }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void ClearInventory()
        {
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() { UserID = UserID, Type = (byte)12, Data = new byte[0], }, DeliveryMethod.ReliableOrdered);
        }
    }
}
