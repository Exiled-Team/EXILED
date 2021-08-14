// -----------------------------------------------------------------------
// <copyright file="SendBroadcastPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    /// <summary>
    /// Send broadcast packet.
    /// </summary>
    public class SendBroadcastPacket
    {
        /// <summary>
        /// Gets or sets a value indicating whether is admin only.
        /// </summary>
        public bool IsAdminOnly { get; set; }

        /// <summary>
        /// Gets or sets broadcast message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets duration of broadcast.
        /// </summary>
        public ushort Duration { get; set; }
    }
}
