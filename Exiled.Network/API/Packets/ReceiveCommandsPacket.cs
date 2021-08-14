// -----------------------------------------------------------------------
// <copyright file="ReceiveCommandsPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    using System.Collections.Generic;

    /// <summary>
    /// Receive commands packet.
    /// </summary>
    public class ReceiveCommandsPacket
    {
        /// <summary>
        /// Gets or sets received commands.
        /// </summary>
        public List<CommandInfoPacket> Commands { get; set; }
    }
}
