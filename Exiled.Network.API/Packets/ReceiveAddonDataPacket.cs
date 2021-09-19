// -----------------------------------------------------------------------
// <copyright file="ReceiveAddonDataPacket.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network.API.Packets
{
    /// <summary>
    /// Receive addons packet.
    /// </summary>
    public class ReceiveAddonDataPacket
    {
        /// <summary>
        /// Gets or sets addon id.
        /// </summary>
        public string AddonID { get; set; }

        /// <summary>
        /// Gets or sets addon data.
        /// </summary>
        public byte[] Data { get; set; }
    }
}
