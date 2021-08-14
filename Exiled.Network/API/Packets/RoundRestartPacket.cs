// -----------------------------------------------------------------------
// <copyright file="RoundRestartPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    /// <summary>
    /// Round restart packet.
    /// </summary>
    public class RoundRestartPacket
    {
        /// <summary>
        /// Gets or sets redirect port.
        /// </summary>
        public ushort Port { get; set; }
    }
}
