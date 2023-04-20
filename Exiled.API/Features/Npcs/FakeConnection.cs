// -----------------------------------------------------------------------
// <copyright file="FakeConnection.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Npcs
{
    using System;

    using Mirror;

    /// <summary>
    /// The fake connection used by NPCs.
    /// </summary>
    public class FakeConnection : NetworkConnectionToClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeConnection"/> class.
        /// </summary>
        /// <param name="id">The ConnectionId of the FakeConnection.</param>
        public FakeConnection(int id)
            : base(id, false, 0f)
        {
        }

        /// <summary>
        /// Gets the address of the connection.
        /// </summary>
        public override string address => "npc";

        /// <summary>
        /// Sends a message to the connection.
        /// </summary>
        /// <param name="segment">The segment of the message.</param>
        /// <param name="channelId">The ChannelId.</param>
        public override void Send(ArraySegment<byte> segment, int channelId = 0)
        {
        }

        /// <summary>
        /// Disconnects the connection.
        /// </summary>
        public override void Disconnect()
        {
        }
    }
}