// -----------------------------------------------------------------------
// <copyright file="ReceiveAddonsPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    /// <summary>
    /// Receive addons packet.
    /// </summary>
    public class ReceiveAddonsPacket
    {
        /// <summary>
        /// Gets or sets addon ids.
        /// </summary>
        public string[] AddonIds { get; set; }
    }
}
