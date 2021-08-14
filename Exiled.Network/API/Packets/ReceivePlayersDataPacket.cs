// -----------------------------------------------------------------------
// <copyright file="ReceivePlayersDataPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    using System.Collections.Generic;

    /// <summary>
    /// Receive players packet.
    /// </summary>
    public class ReceivePlayersDataPacket
    {
        /// <summary>
        /// Gets or sets players.
        /// </summary>
        public List<PlayerInfoPacket> Players { get; set; }
    }
}
